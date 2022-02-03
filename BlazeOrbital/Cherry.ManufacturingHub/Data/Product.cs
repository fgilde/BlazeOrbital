using System.Web;

namespace Cherry.Data;

public partial class Product
{
    public string Url => $"https://letmebingthatforyou.com/?q={HttpUtility.UrlEncode(Name)}";
}
