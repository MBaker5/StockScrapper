using CsvHelper;
using StockScrapper_App.Core;
using StockScrapper_App.Services;
using StockScrapper_Database.Models;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace StockScrapper.Panels;

public partial class CurrencyStockPrice : ContentPage
{
	private readonly IHtmlScrappService _scrapp;

	private List<CurrencyModel> currencyModels = new List<CurrencyModel>();
	public CurrencyStockPrice()
	{
		_scrapp = new HtmlScrappService();
		InitializeComponent();
		LoadData();
	}

	private async void LoadData()
	{
		currencyModels = await Task.Run(() => _scrapp.GetDataFromNBP());
		CurrencyListView.ItemsSource = currencyModels;
	}

	private void CSVGenerate_Clicked(object sender, EventArgs e)
	{
		ConvertToCsv(currencyModels);
	}

	public void ConvertToCsv(List<CurrencyModel> currencyDataList)
	{
		try
		{
			var filePath = $"C:\\Users\\Administrator\\Desktop\\Currencies.csv";
			using (var writer = new StreamWriter(filePath))
			using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
			{
				csv.WriteHeader<StockData>();
				csv.NextRecord();
				foreach (var stockData in currencyDataList)
				{
					csv.WriteRecord(stockData);
					csv.NextRecord();
				}
			}
		}
		catch(Exception ex)
		{

		}
		
	}

	
}