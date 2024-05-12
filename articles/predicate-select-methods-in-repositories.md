# Predicates in SELECT methods for repositories

Suppose one has a repository for a user entity:

```cs
interface IUserRepository 
{
    User FindUserByUsername(string username);
    User FindUserByEmail(string email);

    List<User> GetUsersByAge(int age);

    // And so on...
}
```

One might argue that it is much simpler to just pass in a predicate, like so:

```cs
interface IUserRepository
{
    User FindUser(Func<User, bool> predicate);
    List<User> FindUsers(Func<User, bool> predicate);
}
```

Easy right? Now the consumer can just match on whatever predicate they want.

Now, there are a few issue with this approach. 

The first issue is that when you use a `Func<User, bool>` instead of a `Expression<Func<User, bool>>` you are going to use the `IEnumerable.Where` instead of the `IQuerable.Where`, which loads in the entire table into memory.

Alright, so we just change it into an `Expresssion<Func<User, bool>>` and we are good right?

No. What if a user has an `IsDeleted` property on which we need to match as well? Suddenly the consumer of the repository needs to know all kinds of details about how a user is stored, and when a user is valid.

In the case of multi-tenancy within a single database, this can result in catastrophical bugs.

This can still be remedied by appending these additional checks to the end of a given `IQueryable` before passing it over to the ORM, however...

This simplification of the repository layer transfer the complexity to the business logic layer. In this case you can just throw the entire repository layer away and use `IQueryable` within the services directly.

The idea of a repository layer is to be able to replace a database completely as long as you can still get the same data structure out of it. If you use higher order functions to query the database in the repository layer, you have essentially created an ORM to query your ORM.
