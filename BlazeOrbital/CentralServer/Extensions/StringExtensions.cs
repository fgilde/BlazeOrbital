using System.Globalization;

namespace Cherry.Data.Extensions;

public static class StringExtensions
{
    public static decimal ParsePrice(this string priceStr)
    {
        return decimal.Parse(priceStr, NumberStyles.Currency);
    }
}