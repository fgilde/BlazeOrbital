namespace Cherry.Data;

public partial class Product
{
    public bool NeedsUpdate(Product other)
    {
        var clonedSelf = Clone();
        var clonedOther = other.Clone();
        clonedOther.DateUpdated = clonedOther.DateCreated = clonedSelf.DateCreated = clonedSelf.DateUpdated = default;
        return !clonedSelf.Equals(clonedOther);
    }

    public DateTime Created
    {
        get => new(DateCreated);
        set => DateCreated = value.Ticks;
    }

    public DateTime Updated
    {
        get => new(DateUpdated);
        set => DateUpdated = value.Ticks;
    }
}