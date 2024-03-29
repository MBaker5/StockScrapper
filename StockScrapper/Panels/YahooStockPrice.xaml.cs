using StockScrapper.Models;
using StockScrapper_App.Core;
using StockScrapper_App.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace StockScrapper.Panels;

public partial class YahooStockPrice : ContentPage
{
    private readonly IHtmlScrappService _scrapp;

    public ObservableCollection<CurrencyData> CurrencyList { get; set; } = new ObservableCollection<CurrencyData>();
    public CompanyShortcutEnum SelectedCompany { get; set; }
    public CompanyShortcutEnum[] CompanyShortcuts => (CompanyShortcutEnum[])Enum.GetValues(typeof(CompanyShortcutEnum));

    public YahooStockPrice()
    {
        try
        {
            InitializeComponent();
            BindingContext = this;
            _scrapp = new HtmlScrappService();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    private void ScrapButton_Clicked(object sender, EventArgs e)
    {
        CurrencyList.Clear();
        string companyShortcut = SelectedCompany.ToString();
        var xd = _scrapp.ScrapYahoo(companyShortcut);

        foreach (var x in xd)
        {
            CurrencyData currencyData = new()
            {
                ExchangeRate = x.ExchangeRate,
                CurrencyCode = x.CurrencyCode,
            };
            CurrencyList.Add(currencyData);
        }

        currencyListView.ItemsSource = CurrencyList;
    }
}