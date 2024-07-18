using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using SkiaSharp;
using StockScrapper.Models;
using StockScrapper_App.Core;
using StockScrapper_App.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;
using CsvHelper;
using System.Collections.Generic;

namespace StockScrapper.Panels
{
	public partial class YahooStockPriceViewModel : BindableObject
	{
		private readonly IHtmlScrappService _scrapp;

		public ObservableCollection<StockData> StockDataList { get; set; } = new ObservableCollection<StockData>();
		public string SelectedCompany { get; set; }

		private List<string> _companyShortcuts;
		public List<string> CompanyShortcuts
		{
			get => _companyShortcuts;
			set
			{
				if (_companyShortcuts != value)
				{
					_companyShortcuts = value;
					OnPropertyChanged(nameof(CompanyShortcuts));
				}
			}
		}

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

		private Axis[] _yaxsis;
		public Axis[] YAxsis
		{
			get => _yaxsis;
			set
			{
				if (_yaxsis != value)
				{
					_yaxsis = value;
					OnPropertyChanged(nameof(YAxsis));
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

		private DateTime _startDate;
		public DateTime StartDate
		{
			get => _startDate;
			set
			{
				if (_startDate != value)
				{
					_startDate = value;
					OnPropertyChanged(nameof(StartDate));
				}
			}
		}

		private DateTime _endDate;
		public DateTime EndDate
		{
			get => _endDate;
			set
			{
				if (_endDate != value)
				{
					_endDate = value;
					OnPropertyChanged(nameof(EndDate));
				}
			}
		}

		private bool _isBusy;
		public bool IsBusy
		{
			get => _isBusy;
			set
			{
				if (_isBusy != value)
				{
					_isBusy = value;
					OnPropertyChanged();
				}
			}
		}

		private bool _isVisibility;
		public bool isVisibility
		{
			get => _isVisibility;
			set
			{
				if (_isVisibility != value)
				{
					_isVisibility = value;
					OnPropertyChanged();
				}
			}
		}

		public ICommand ScrapCommand { get; }
		public ICommand GenerateCSVCommand { get; }

		public YahooStockPriceViewModel()
		{
			_scrapp = new HtmlScrappService();

			_currentDate = DateTime.Now;
			_isVisibility = false;

			ScrapCommand = new Command(async () => await ScrapAsync());
			GenerateCSVCommand = new Command(async () => await ScrapAsync());

			EndDate = DateTime.Now;
			StartDate = DateTime.Now;

			LoadShortcuts();
		}

		private async Task ActivateIndicatorAsync(bool state)
		{
			isVisibility = state;
			IsBusy = state;
			await Task.Delay(20);
		}

		private void LoadShortcuts()
		{
			_companyShortcuts = _scrapp.GetMostActiveOnMarket();
		}

		private async Task SaveToCsvAsync(string filePath)
		{
			try
			{
				using (var writer = new StreamWriter(filePath))
				using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
				{
					csv.WriteHeader<StockData>();
					await csv.NextRecordAsync();
					foreach (var stockData in StockDataList)
					{
						csv.WriteRecord(stockData);
						await csv.NextRecordAsync();
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"An error occurred while saving to CSV: {ex.Message}");
			}
		}

		public async Task LoadChartAsync(List<FinancialPoint> entries)
		{
			Series = new ISeries[]
				{
					new CandlesticksSeries<FinancialPoint>
					{
						UpFill = new SolidColorPaint(SKColors.Blue),
						UpStroke = new SolidColorPaint(SKColors.CornflowerBlue) { StrokeThickness = 0 },
						DownFill = new SolidColorPaint(SKColors.Red),
						DownStroke = new SolidColorPaint(SKColors.Orange) { StrokeThickness = 0 },
						Values = entries
					}
				};

			XAxsis = new Axis[]
			{
					new Axis
					{
						UnitWidth = TimeSpan.FromDays(1).Ticks,
						LabelsRotation = 60,
						Labeler = value =>
						{
							DateTime dateTime = new DateTime((long)value);
							return dateTime.ToString("dd/M/yyyy");
						},
						MinStep = 1,
						SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray)
						{
							StrokeThickness = 0.5f,
							PathEffect = new DashEffect(new float[] { 3, 3 }),
						},
						SeparatorsAtCenter = true,
						ShowSeparatorLines = true,
					}
			};

			YAxsis = new Axis[]
			{
					new Axis
					{
						UnitWidth = 5,
						Labeler = Labelers.Currency
					}
			};
		}

		private async Task ScrapAsync()
		{
			try
			{
				await ActivateIndicatorAsync(true);

				StockDataList.Clear();
				string companyShortcut = SelectedCompany.ToString();

				var url = _scrapp.ConstructUrl(companyShortcut, StartDate, EndDate);
				var stockList = _scrapp.ScrapYahooAsync(url);
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
						Debug.WriteLine($"Failed to parse stock data for date {s.Date}");
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

				List<string> dateLabels = new List<string>();

				DateTime currentDate = StockDataList.Min(x => x.Date);
				while (currentDate <= StockDataList.Max(x => x.Date))
				{
					if (currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
					{
						dateLabels.Add(currentDate.ToString("yyyy-MM-dd"));
					}
					currentDate = currentDate.AddDays(1);
				}

				await LoadChartAsync(entries);

				await ActivateIndicatorAsync(false);
			}
			catch
			{

			}
		}
	}
}