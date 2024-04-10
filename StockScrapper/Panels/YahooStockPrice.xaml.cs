using StockScrapper.Models;
using StockScrapper_App.Core;
using StockScrapper_App.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microcharts;
using System.Collections.Generic;
using SkiaSharp;
using System.Globalization;

namespace StockScrapper.Panels;

public partial class YahooStockPrice : ContentPage
{
    private readonly IHtmlScrappService _scrapp;

    public ObservableCollection<StockData> StockDataList { get; set; } = new ObservableCollection<StockData>();
    public CompanyShortcutEnum SelectedCompany { get; set; }
    public CompanyShortcutEnum[] CompanyShortcuts => (CompanyShortcutEnum[])Enum.GetValues(typeof(CompanyShortcutEnum));

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

		var entries = new List<ChartEntry>();

		foreach (var s in stockList)
		{
			DateTime dateTime = DateTime.Parse(s.Date);
			float lowPrice;
			float openPrice;
			float closePrice;
			float highPrice;

			// Attempt to parse the string representations of floats
			if (!float.TryParse(s.LowPrice, NumberStyles.Float, CultureInfo.InvariantCulture, out lowPrice) ||
				!float.TryParse(s.OpenPrice, NumberStyles.Float, CultureInfo.InvariantCulture, out openPrice) ||
				!float.TryParse(s.ClosePrice, NumberStyles.Float, CultureInfo.InvariantCulture, out closePrice) ||
				!float.TryParse(s.HighPrice, NumberStyles.Float, CultureInfo.InvariantCulture, out highPrice))
			{
				// Handle parsing errors
				// For example, you can set default values, log the error, or throw an exception
				Console.WriteLine($"Failed to parse stock data for date {s.Date}");
				continue; // Skip this entry and move to the next one
			}

			var entry = new ChartEntry(openPrice)
			{
				Label = s.Date,
				ValueLabel = $"{s.LowPrice} - {s.HighPrice}",
				Color = SKColor.Parse("#266489") // Adjust color as needed
			};
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



		var chart = new LineChart 
		{
			Entries = entries,
			LineMode = LineMode.Straight,
			LineSize = 1,
			PointMode = PointMode.Square,
			PointSize = 4,
			LabelTextSize = 12,
			BackgroundColor = SKColor.Parse("#FAFAFA")
		};
		candlestickChart.Chart = chart;
	}
}