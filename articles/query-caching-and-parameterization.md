# Query caching and parameterization

When EF recieves a LINQ query tree, it will first compile it and produce SQL from it.

EF will cache these queries, so that executing the same LINQ statement multiple times will be very fast.

Suppose we have the following two queries:

```cs
var article1 = db.Articles.First(a => a.Title == "Article1");
var article2 = db.Articles.First(a => a.Title == "Article2");
```

Because the query tree contains a different constants, a slightly different SQL command is generated.

```sql
SELECT TOP(1) [a].[Id], [a].[Title]
FROM [Articles] AS [a]
WHERE [a].[Title] = N'Article1'

SELECT TOP(1) [a].[Id], [a].[Title]
FROM [Articles] AS [a]
WHERE [a].[Title] = N'Article2'
```

Because the SQL differs, the database will likely produce a query plan for both queries.

Parameterization of the query will solve this problem, like so:

```cs
var articleTitle = "Article1";
var article = db.Articles.First(a => a.Title == articleTitle);
```

Because this produces the following SQL query:

```sql
SELECT TOP(1) [a].[Id], [a].[Title]
FROM [Articles] AS [a]
WHERE [a].[Title] = @__articleTitle_0
```

Depending on how many times the parameter changes, there is no need to parameterize every query, like in this scenario:

```cs
var article1 = db.Articles.First(a => a.Title == articleTitle && a.Status == ArticleStatus.Published);
```

Even if there are five different `ArticleStatus` options, at some point there will be a cached query for all of them.

It is a good idea to check the EF event counters report for the Query Cache Hit Rate. This counter should reach 100% soon after the program starts up, and most queries have been executed once.