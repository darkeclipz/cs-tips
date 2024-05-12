# Hooking getters and setters

**Break into the debugger when a value changes**

```cs
class Person
{
    private string _password;

    public string Password
    {
        get => _password;
        set
        {
            ; // Add a breakpoint on this line                      
            _password = value;
        }
    }
}
```

**Add a property changing and changed event**

```cs
class Book
{
    private Money price;

    public event EventHandler PriceChanging;
    public event EventHandler PriceChanged;

    public Money Price
    {
        get
        {
            return price;
        }

        set
        {
            OnPriceChanging();
            price = value;
            OnPriceChanged();
        }
    }

    private void OnPriceChanging()
    {
        PriceChanging?.Invoke(this, EventArgs.Empty);
    }

    private void OnPriceChanged()
    {
        PriceChanged?.Invoke(this, EventArgs.Empty);
    }
}
```
