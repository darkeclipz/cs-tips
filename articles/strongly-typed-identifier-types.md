# Strongly typed identifier types

Suppose we have the following entity:

```cs
class Article
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime DatePublished { get; set; }

    public Guid AuthorId { get; set; }
    public Guid CategoryId { get; set; }

    public virtual Author Author { get; set; }
    public virtual Category Category { get; set; }
}
```

This is probably what every post or documentation page will use as an example to implement such an entity.

Now suppose that we have a service that is responsible for retrieving articles based on a few criteria.

```cs
interface IArticleService
{
    Article GetArticleById(Guid id);

    IEnumerable<Article> GetArticlesByAuthorId(Guid id);
    IEnumerable<Article> GetArticlesByCategoryId(Guid id);
}
```

It has happened to me plenty of times before that I made a silly mistake and did something like this:

```cs
var articles = ArticleService.GetArticlesByCategoryId(article.AuthorId);
```

Now this is a rather extreme scenario because `AuthorId` and `CategoryId` are rather different concepts, but this example is used to exeggerate the bug.

Wouldn't it be awesome if the compiler in _our strongly typed language_ complains about using the wrong identifier here?

That is exactly the type of problem that a _strong typed identifier type_ is solving. Instead of using a `Guid` as the identifier for our entity, we create a specific identifier for the entity itself, like so:

```cs
public readonly record struct ArticleId(Guid Value);
```

> Using a `record struct` gives us the performance of a `struct`, but the concise declarativeness of a `record` and the equality comparisons of a `record` as well.

With this change in mind the `Article` class becomes as follows:

```cs
class Article
{
    public ArticleId Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime DatePublished { get; set; }

    public AuthorId AuthorId { get; set; }
    public CategoryId CategoryId { get; set; }

    ...
}
```

## Configuring EF

To enable EF to work with strongy typed IDs, we need to tell EF how to convert it. This is done by specifying the `HasConversion` parameter on a property, like so:

```cs
builder.Entity<Article>
    .Property(article => article.Id)
    .HasConversion(id => id.Value, value => new(value))
    .ValueGeneratedOnAdd();
```

> Add `ValueGeneratedOnAdd` to enable the primitive to be auto-generated when adding (`SaveChanges`).
