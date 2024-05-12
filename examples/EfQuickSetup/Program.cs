using Microsoft.EntityFrameworkCore;

var category = Category.For("Nature", "Fluffy articles about nature.");
var author = Author.For("John", "Doe");
var article = Article.For("Hello world", "This is my first article!", author, category);

using(var insertContext = new BlogContext())
{
    insertContext.Articles.Add(article);
    insertContext.SaveChanges();
}

using (var readContext = new BlogContext())
{
    var existingArticle = readContext.Articles
                                     .Include(a => a.Author)
                                     .Include(a => a.Category)
                                     .First(a => a.Id == article.Id);

    Console.WriteLine(existingArticle.Title);
    Console.WriteLine($"Published by {existingArticle.Author.FirstName} {existingArticle.Author.LastName}.");
    Console.WriteLine();
    Console.WriteLine(existingArticle.Content);
    Console.WriteLine($"Published on {existingArticle.DatePublished}");
    Console.WriteLine($"Category: {existingArticle.Category.Name}");
}

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