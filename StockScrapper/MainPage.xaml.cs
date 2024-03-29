﻿using StockScrapper.Models;
using StockScrapper.Panels;
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

		public MainPage()
		{
			InitializeComponent();
		}

        private async void NBPExchangeRate_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NBPExchangeRate());
        }

        private void btnYahooScrapp_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new NBPExchangeRate());
        }
    }

}
