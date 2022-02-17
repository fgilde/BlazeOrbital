using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace QuickGrid.EF;

public static class Extensions
{
    public static string GetTableName<T>(this DbContext context) where T : class
    {
        return context.Model.FindEntityType(typeof(T))?.GetTableName() ?? string.Empty;
    }
}