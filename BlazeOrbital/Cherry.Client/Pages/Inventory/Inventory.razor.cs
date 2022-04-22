using System.Globalization;
using Cherry.Client.Data;
using Cherry.Client.Layout;
using Cherry.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace Cherry.Client.Pages.Inventory;

public partial class Inventory: IDisposable
{
    ClientSideDbContext? db;
    private string? _search;
    private bool blocked = true;

    [Parameter] public string? SearchName
    {
        get => _search;
        set
        {
            _search = value;
            Task.Delay(300).ContinueWith(t => navigationManager.NavigateTo(!string.IsNullOrWhiteSpace(_search) ? $"search/{_search}" : "/", false, true));
        }
    }
    [Inject] private DataSynchronizer DataSynchronizer { get; set; }
    [Inject] private NavigationManager navigationManager { get; set; }

    

    string[] categories = Array.Empty<string>();
    string[] subcategories = Array.Empty<string>();
    
    int minStock, maxStock = 50000;
    
    IQueryable<Product>? GetFilteredParts()
    {
        if (db is null)
        {
            return null;
        }

        var result = db.Products.AsNoTracking().AsQueryable();
        if (categories.Any())
        {
            result = result.Where(x => categories.Contains(x.Category));
        }
        if (subcategories.Any())
        {
            result = result.Where(x => subcategories.Contains(x.Subcategory));
        }
        if (!string.IsNullOrWhiteSpace(_search))
        {
            result = result.Where(x => EF.Functions.Like(x.Name, _search.Replace("%", "\\%") + "%", "\\"));
        }
        if (minStock > 0)
        {
            result = result.Where(x => Convert.ToDecimal(x.SalePrice, NumberFormatInfo.CurrentInfo) >= minStock);
        }
        if (maxStock < 50000)
        {
            // result = result.Where(x => x.SalePrice.ParsePrice() <= maxStock);
        }

        return result;
    }

    protected override async Task OnInitializedAsync()
    {
        db = await DataSynchronizer.GetPreparedDbContextAsync();
        DataSynchronizer.OnUpdate += StateHasChanged;
    }

    public void Dispose()
    {
        db?.Dispose();
        DataSynchronizer.OnUpdate -= StateHasChanged;
    }

    private void NavigateTo(string url)
    {
        navigationManager.NavigateTo(url);
    }

    void ToggleMode()
    {
        blocked = !blocked;
    }

    private void OnItemClick(Product? product)
    {
        App.Instance.SelectedProduct = product;
        if (product != null)
            navigationManager.NavigateTo(product.Url);
    }
}