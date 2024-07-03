using StockScrapper_App.Core;
using StockScrapper_App.Services;
using StockScrapper_Database.Models;

namespace StockScrapper.Panels;

public partial class CurrencyStockPrice : ContentPage
{
	private readonly IHtmlScrappService _scrapp;
	public CurrencyStockPrice()
	{
		_scrapp = new HtmlScrappService();
		InitializeComponent();
		LoadData();
	}

	private async void LoadData()
	{
		List<CurrencyModel> currencyList = await Task.Run(() => _scrapp.GetDataFromNBP());
		CurrencyListView.ItemsSource = currencyList;
	}


}