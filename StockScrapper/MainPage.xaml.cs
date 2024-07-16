using StockScrapper.Models;
using StockScrapper.Panels;
using StockScrapper.Panels.Tabs;
using StockScrapper_App.Core;
using StockScrapper_App.Services;
using StockScrapper_Database.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StockScrapper
{
	public partial class MainPage : ContentPage, INotifyPropertyChanged
	{
		private readonly IHtmlScrappService _scrapp;
		private bool isBusy;

		public event PropertyChangedEventHandler PropertyChanged;

		public bool IsBusy
		{
			get => isBusy;
			set
			{
				if (isBusy != value)
				{
					isBusy = value;
					OnPropertyChanged();
				}
			}
		}

		public MainPage()
		{
			InitializeComponent();
			BindingContext = this;
		}

		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private async void NBPExchangeRate_Clicked(object sender, EventArgs e)
		{
			await SetBusyStateAsync(true);
			await Navigation.PushAsync(new YahooStockPriceChartsTab());
			await SetBusyStateAsync(false);
		}

		private async void btnYahooScrapp_Clicked(object sender, EventArgs e)
		{
			await SetBusyStateAsync(true);
			await Navigation.PushAsync(new YahooStockPriceChartsTab());
			await SetBusyStateAsync(false);
		}

		private async void btnCurrencyData_Clicked(object sender, EventArgs e)
		{
			await SetBusyStateAsync(true);
			await Navigation.PushAsync(new CurrencyStockPrice());
			await SetBusyStateAsync(false);
		}

		private async Task SetBusyStateAsync(bool isBusy)
		{
			IsBusy = isBusy;
			await Task.Delay(100); // Add a small delay to give the UI a chance to update
		}
	}
}
