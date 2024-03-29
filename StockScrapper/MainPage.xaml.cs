using StockScrapper.Models;
using StockScrapper_App.Core;
using StockScrapper_App.Services;
using StockScrapper_Database.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace StockScrapper
{
	public partial class MainPage : ContentPage
	{
		private readonly IHtmlScrappService _scrapp;

		public ObservableCollection<CurrencyData> CurrencyList { get; set; } = new ObservableCollection<CurrencyData>();


		int count = 0;

		public MainPage()
		{
			try 
			{
				InitializeComponent();
				_scrapp = new HtmlScrappService();
				var xd = _scrapp.GetDataFromNBP();

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
			catch(Exception ex) 
			{ 
				Debug.WriteLine(ex);
			}
			
		}

		
	}

}
