using System.Globalization;

namespace Cherry.ManufacturingHub.Extensions;

public static class StringExtensions
{
    public static decimal ParsePrice(this string priceStr)
    {
        return decimal.Parse(priceStr, NumberStyles.Currency);
    }
}