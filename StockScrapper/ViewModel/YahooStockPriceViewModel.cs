using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Maui.Controls;
using SkiaSharp;
using StockScrapper.Models;
using StockScrapper_App.Core;
using StockScrapper_App.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

namespace StockScrapper.Panels
{
	public partial class YahooStockPriceViewModel : BindableObject
	{
		private readonly IHtmlScrappService _scrapp;

		public ObservableCollection<StockData> StockDataList { get; set; } = new ObservableCollection<StockData>();
		public CompanyShortcutEnum SelectedCompany { get; set; }
		public CompanyShortcutEnum[] CompanyShortcuts => (CompanyShortcutEnum[])Enum.GetValues(typeof(CompanyShortcutEnum));

		private ISeries[] _series;
		public ISeries[] Series
		{
			get => _series;
			set
			{
				if (_series != value)
				{
					_series = value;
					OnPropertyChanged(nameof(Series));
				}
			}
		}
		private Axis[] _xaxsis;
		public Axis[] XAxsis 
		{  
			get => _xaxsis; 
			set
			{
				if (_xaxsis != value)
				{
					_xaxsis = value;
					OnPropertyChanged(nameof(XAxsis));
				}
			}
		}

		private DateTime _currentDate;
		public DateTime CurrentDate
		{
			get => _currentDate;
			set
			{
				if (_currentDate != value)
				{
					_currentDate = value;
					OnPropertyChanged(nameof(CurrentDate));
				}
			}
		}

		public ICommand ScrapCommand { get; }

		public YahooStockPriceViewModel()
		{
			_scrapp = new HtmlScrappService();
			_currentDate = DateTime.Now;
			ScrapCommand = new Command(Scrap);
		}

		private void Scrap()
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

				if (!double.TryParse(s.LowPrice, NumberStyles.Float, CultureInfo.InvariantCulture, out lowPrice) ||
					!double.TryParse(s.OpenPrice, NumberStyles.Float, CultureInfo.InvariantCulture, out openPrice) ||
					!double.TryParse(s.ClosePrice, NumberStyles.Float, CultureInfo.InvariantCulture, out closePrice) ||
					!double.TryParse(s.HighPrice, NumberStyles.Float, CultureInfo.InvariantCulture, out highPrice))
				{
					Console.WriteLine($"Failed to parse stock data for date {s.Date}");
					continue;
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
					DownFill = new SolidColorPaint(SKColors.Red),
					Values = entries
				}
			};


			XAxsis = new Axis[]
			{
				new Axis
				{
					LabelsRotation = 60,
					Labeler = value => new DateTime((long)value).ToString("yyyy MMM dd"),
				// set the unit width of the axis to "days"
				// since our X axis is of type date time and 
				// the interval between our points is in days
					UnitWidth = TimeSpan.FromDays(1).Ticks,
				}
			};
			
		}
	}
}
