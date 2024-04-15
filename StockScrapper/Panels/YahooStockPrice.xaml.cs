using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using StockScrapper.Models;
using StockScrapper_App.Core;
using StockScrapper_App.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
//using UIKit;


namespace StockScrapper.Panels;

public partial class YahooStockPrice : ContentPage
{
	private readonly IHtmlScrappService _scrapp;

	public ObservableCollection<StockData> StockDataList { get; set; } = new ObservableCollection<StockData>();
	public CompanyShortcutEnum SelectedCompany { get; set; }
	public CompanyShortcutEnum[] CompanyShortcuts => (CompanyShortcutEnum[])Enum.GetValues(typeof(CompanyShortcutEnum));

	public ISeries[] Series { get; set; }


	private DateTime _currentDate;
	public DateTime CurrentDate
	{
		get { return _currentDate; }
		set
		{
			if (_currentDate != value)
			{
				_currentDate = value;
				OnPropertyChanged(nameof(CurrentDate));
			}
		}
	}


	public YahooStockPrice()
	{
		try
		{
			InitializeComponent();
			BindingContext = this;
			_scrapp = new HtmlScrappService();
			_currentDate = DateTime.Now;

		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
		}
	}

	private void ScrapButton_Clicked(object sender, EventArgs e)
	{
		StockDataList.Clear();
		string companyShortcut = SelectedCompany.ToString();
		var stockList = _scrapp.ScrapYahoo(companyShortcut);

		var entries = new List<FinancialPoint>();

		foreach (var s in stockList)
		{
			DateTime dateTime = DateTime.Parse(s.Date);
			double lowPrice;
			double openPrice;
			double closePrice;
			double highPrice;

			// Attempt to parse the string representations of doubles
			if (!double.TryParse(s.LowPrice, NumberStyles.Float, CultureInfo.InvariantCulture, out lowPrice) ||
				!double.TryParse(s.OpenPrice, NumberStyles.Float, CultureInfo.InvariantCulture, out openPrice) ||
				!double.TryParse(s.ClosePrice, NumberStyles.Float, CultureInfo.InvariantCulture, out closePrice) ||
				!double.TryParse(s.HighPrice, NumberStyles.Float, CultureInfo.InvariantCulture, out highPrice))
			{
				// Handle parsing errors
				// For example, you can set default values, log the error, or throw an exception
				Console.WriteLine($"Failed to parse stock data for date {s.Date}");
				continue; // Skip this entry and move to the next one
			}


			var entry = new FinancialPoint(dateTime, highPrice, openPrice, closePrice, lowPrice);
			entries.Add(entry);

			StockData stockData = new()
			{
				Date = dateTime,
				LowPrice = lowPrice,
				OpenPrice = openPrice,
				ClosePrice = closePrice,
				HighPrice = highPrice
			};
			StockDataList.Add(stockData);

			
		}

		Series = new ISeries[]
		{
			new CandlesticksSeries<FinancialPoint>
			{
				UpFill = new SolidColorPaint(SKColors.Blue),
				UpStroke = new SolidColorPaint(SKColors.CornflowerBlue) { StrokeThickness = 5 },
				DownFill = new SolidColorPaint(SKColors.Red),
				DownStroke = new SolidColorPaint(SKColors.Orange) { StrokeThickness = 5 },
				
				Values = entries
			}
		};
		candlestickChart.Series = Series;
		candlestickChart.ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.PanX;

		//candlestickChart.ScaleY = -10;
		//candlestickChart.ScaleX = -10;
		//candlestickChart.MaximumHeightRequest = 1000;
		//candlestickChart.MaximumWidthRequest = 1000;

		// Notify the UI that the Series property has changed
		//OnPropertyChanged(nameof(Series));
	}


}
