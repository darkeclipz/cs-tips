# Properties vs. fields

 1. The question.
 2. What is a field.
 3. What is a property.
 4. Object oriented programming principles
 5. 

When do you use a field versus a property, and what are the arguments for doing so?

> Fields should be kept private (or protected) to a class and accessed via get and set properties.

Suppose we have the following class:

```cs
public class MyClass
{
    private string _myField;

    public string MyProperty
    {
        get => _myField;
        set => _myField = value;
    }

    // C# 3 Auto-property syntax automatically generates a private field.
    public string AnotherProperty { get; set; }
}
```

**Object orientated programming principles**

Object oriented programming principles say that the internal workings of a class should be hidden from the outside world.

There is an unspoken contract between a class-creator and the consumer. Fields hold state, properties expose state using one or more fields, voids change state (heavy lifting), and functions perform queries (heavy lifting).

We can see that the idea of properties being public to a class in the following two examples:

 1. Entity Framework will, by default, only include public properties with a getter and a setter.
 2. Fields are ignored by default during serialization to JSON.

Properties provide a level of abstraction allowing you to change the fields while not affecting the external way they are accessed by the things that use your class, thus breaking the code.

Although exposing a field is not the only use case of a property. It is also possible to create a property that does calculations on another field. In this case the property encapsulates how it got the value.

```cs
class Order 
{
    public Money Price { get; set; }
    public uint Quantity { get; set; }
    public Money Total => Price * Quantity;
}
```

Note that it is expected from the consumer that a property is only exposing state, thus the consumer does not expect a heavy calculation behind a property. If the calculation is expensive, this should be done in a method instead.

Another—arguably bad—use case is to have validations within a property.

```cs
class Person
{
    private int _age;

    public int Age
    {
        get => _age;
        set => _age = value >= 0 : value 
            ? throw new ArgumentException("Age must be positive");
    }
}
```

An argument against having validations within a property is that a property should never do any heavy lifting or validations. If a username, age, or password should have validations it should change from a `string` or `int` into a value type that contains the validations.

Another application of a property is the implementation of lazy initialization. If you have a property of an object that is expensive to load, but isn't accessed all the time in the application, you can only load this once at the moment it is called by a module.

```cs
class DataRepository
{
    private ICollection<Data>? _data;
    private abstract ICollection<Data> QueryData();
    public ICollection<Data> Data => _data ??= QueryData();
}
```

Although in this specific scenario the property is doing a heavy calculation, and it better to convert this into a method instead.

Finally, properties have more fine-grained access control compared to fields. It property could have a public getter but a protected setter. This is not possible with a field.

Changing a field into a property is a breaking change. If you are delivering an assembly that is used by another assembly, the consumer would have to recompile their assembly as a result of this change.

Long story short, if any private fields have to be exposed outside of a class, it should almost always be done so through properties. 

## Sources

 1. https://stackoverflow.com/questions/295104/what-is-the-difference-between-a-field-and-a-property
 2. https://csharpindepth.com/articles/PropertiesMatter