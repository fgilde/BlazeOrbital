using Cherry.Data;
using Microsoft.AspNetCore.Components;

namespace Cherry.Client.Layout;

public partial class App
{
    public Product? SelectedProduct { get; set; }

    public static App Instance { get; private set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Instance = this;
    }
}