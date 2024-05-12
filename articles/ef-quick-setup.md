# Entity Framework quick setup

This document contains a quick example on setting up Entity Framework Core.

## Domain model

The example contains the following domain model.

```cs
readonly record struct ArticleId(Guid Value)
{
    public static ArticleId Empty => new(Guid.Empty);
}

readonly record struct CategoryId(Guid Value)
{
    public static CategoryId Empty => new(Guid.Empty);
}

readonly record struct AuthorId(Guid Value)
{
    public static AuthorId Empty => new(Guid.Empty);
}

class Author
{
    public AuthorId Id { get; init; } = AuthorId.Empty;
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    private ICollection<Article>? _articles;
    public ICollection<Article> Articles => _articles ??= [];

    public static Author For(string firstName, string lastName) => new ()
    {
        Id = AuthorId.Empty,
        FirstName = firstName,
        LastName = lastName
    };
}

class Category
{
    public CategoryId Id { get; init; } = CategoryId.Empty;
    public required string Name { get; set; }
    public required string Description { get; set; }

    private ICollection<Article>? _articles;
    public ICollection<Article> Articles => _articles ??= [];

    public static Category For(string categoryName, string categoryDescription) => new ()
    {
        Id = CategoryId.Empty,
        Name = categoryName,
        Description = categoryDescription
    };
}

class Article
{
    public ArticleId Id { get; init; } = ArticleId.Empty;
    public required string Title { get; set; }
    public required string Content { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime? DateModified { get; set; }
    public DateTime? DatePublished { get; set; }
    public required Author Author { get; set; }
    public required Category Category { get; set; }

    public static Article For(string title, string content, Author author, Category category) => new()
    {
        Id = ArticleId.Empty,
        Title = title,
        Content = content,
        DateCreated = DateTime.UtcNow,
        Author = author,
        Category = category
    };
}
```

## Install EF

First we install the required packages:

```powershell
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
```

If you do not have `dotnet ef` installed, do so with:

```powershell
dotnet tool install --global dotnet-ef
```

## Define DbContext

Then we define a `DbContext` for the domain model, like so:

```cs
class BlogContext : DbContext
{
    public DbSet<Article> Articles { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;Initial Catalog=example");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Set up conversions for strongly type IDs
        modelBuilder.Entity<Article>()
            .Property(article => article.Id)
            .HasConversion(id => id.Value, value => new(value))
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Author>()
            .Property(author => author.Id)
            .HasConversion(id => id.Value, value => new(value))
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Category>()
            .Property(category => category.Id)
            .HasConversion(id => id.Value, value => new(value))
            .ValueGeneratedOnAdd();

        // One-to-many relationships
        modelBuilder.Entity<Article>()
            .HasOne(article => article.Author)
            .WithMany(author => author.Articles);

        modelBuilder.Entity<Article>()
            .HasOne(article => article.Category)
            .WithMany(category => category.Articles);
    }
}
```

### Connection string: LocalDb

```
Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;Initial Catalog=<database-name>
```

### Connection string: Azure SQL

```
Server=<server_name>.database.windows.net;Database=<database_name>;User Id=<database_user>;Password=<database_password>;
```

Use the `RetryOnFailure` policy when using Azure SQL:

```cs
optionsBuilder
    .UseSqlServer(
        connectionString,
        providerOptions => { providerOptions.EnableRetryOnFailure(); });
```

## .NET dependency injection

Additionally, register the service in the .NET DI container:

```cs
builder.Services.AddDbContext<BlogContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnectionString")));
```

## DbFactory (Blazor)

Register the DB context factory instead of the DB context itself:

```cs
services.AddDbContextFactory<ApplicationDbContext>(
    options =>
        options.UseSqlServer(connectionString);
```

And then use it like so:

```cs
@inject IDbContextFactory<ApplicationDbContext> ContextFactory;

using (var context = ContextFactory.CreateDbContext())
{
    // ...
}
```

## Resources

    1. https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/
    2. https://learn.microsoft.com/en-us/ef/core/modeling/
    3. https://learn.microsoft.com/en-us/ef/core/