Me me = new ()
{
    FirstName = "Lars",
    LastName = "Test"
};

class PriceService(Book Book, ICollection<IDiscount> Discounts, SellingContext SellingContext)
{
    public IEnumerable<PriceLine> CalculatePriceLines(Money originalPrice)
    {
        List<PriceLine> priceLines = [];

        priceLines.Add(new("Original price", originalPrice));

    }
}

interface IDiscount
{
    IEnumerable<DiscountApplication> GetDiscountAmount(Money price);
    public IDiscount IsApplicable(SellingContext context) => this;
}

static class NoDiscount
{
    public static EmptyDiscount Instance = new();
}

class EmptyDiscount : IDiscount
{
    public IEnumerable<DiscountApplication> GetDiscountAmount(Money price) => [];
}

class FixedDiscount(Money discount) : IDiscount
{
    readonly Money Discount = discount;

    public IEnumerable<DiscountApplication> GetDiscountAmount(Money price)
    {
        return [new DiscountApplication($"{Discount.Value:N2} euro off.", Discount)];
    }
}

record DiscountApplication(string Label, Money Amount);
record SellingContext(User User, Country Country, Book Book);
record User(string Username, string Email, DateTime MemberSince);
record Country(string Name);
record Money(decimal Value, string Currency);
record PriceLine(string Labl, Money Price);
record Book(string Title, BookAuthor Author);
record BookAuthor(string Name);

class Me
{
    public required string FirstName { get; init; }

    string _lastName;
    public required string LastName
    {
        get => _lastName;
        set => _lastName = value;
    }
}