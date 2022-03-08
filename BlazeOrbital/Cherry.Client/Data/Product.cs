using System.Web;

// Dont change this namespace. 
namespace Cherry.Data;

public partial class Product
{
    public string Url => $"p/{Id}/{Name}";

    //public string Url => $"https://letmebingthatforyou.com/?q={HttpUtility.UrlEncode((string?) Name)}";
}
