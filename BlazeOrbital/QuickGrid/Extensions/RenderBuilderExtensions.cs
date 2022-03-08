using Microsoft.AspNetCore.Components.Rendering;

namespace QuickGrid.Extensions;

internal static class RenderBuilderExtensions
{
    public static RenderTreeBuilder OpenAnchorIf(this RenderTreeBuilder builder, string href)
    {
        if (!string.IsNullOrWhiteSpace(href))
        {
            builder.OpenElement(0, "a");
            builder.AddAttribute(1, "href", href);
        }
        return builder;
    }

    public static RenderTreeBuilder CloseAnchorIf(this RenderTreeBuilder builder, string href)
    {
        if (!string.IsNullOrWhiteSpace(href))
        {
            builder.CloseElement();
        }
        return builder;
    }
}