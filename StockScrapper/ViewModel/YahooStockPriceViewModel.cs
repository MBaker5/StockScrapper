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
using CommunityToolkit.Maui.Storage;
using System.Text;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.Controls.PlatformConfiguration;
using LiveChartsCore.SkiaSharpView.SKCharts;

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
		public Axis[] XAxes { get; set; } =
	{
		new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("dd/MM"))
	};

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

		private bool _isChartEnabled;
		public bool IsChartEnabled
		{
			get => _isChartEnabled;
			set
			{
				if (_isChartEnabled != value)
				{
					_isChartEnabled = value;
					OnPropertyChanged();
				}
			}
		}

		private bool _stockType; // true = mostActive, false = mostTrending

		public ICommand ScrapCommand { get; }
		public ICommand GenerateCSVCommand { get; }

		public YahooStockPriceViewModel()
		{
			_scrapp = new HtmlScrappService();
			_currentDate = DateTime.Now;
			_isVisibility = false;

			ScrapCommand = new Command(async () => await ScrapAsync());
			GenerateCSVCommand = new Command(async () => await SaveToCsvAsync("scrap.csv"));

			EndDate = DateTime.Now;
			StartDate = DateTime.Now;
			IsChartEnabled = false;
			
		}

		public void LoadShortcuts(bool stockType)
		{
			if (stockType)
			{
				_companyShortcuts = _scrapp.GetMostActiveOnMarket();
			}
			else
			{
				_companyShortcuts = _scrapp.GetMostTrendingOnMarket();
			}
		}

		private async Task ActivateIndicatorAsync(bool state)
		{
			isVisibility = state;
			IsBusy = state;
			await Task.Delay(20);
		}

		


		public static string ConvertToCsv(ObservableCollection<StockData> stockDataList)
		{
			try
			{
				var csv = new StringBuilder();
				csv.AppendLine("Date,OpenPrice,HighPrice,LowPrice,ClosePrice");

				foreach (var stockData in stockDataList)
				{
					csv.AppendLine($"{stockData.Date:yyyy-MM-dd},{stockData.OpenPrice},{stockData.HighPrice},{stockData.LowPrice},{stockData.ClosePrice}");
				}

				return csv.ToString();
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		private async Task SaveToCsvAsync(string filePath)
		{
			try
			{
				bool isAndroid = DeviceInfo.Current.Platform == DevicePlatform.Android; //Check if its running on android
				await ActivateIndicatorAsync(true);

				StockDataList.Clear();
				string companyShortcut = SelectedCompany.ToString();
				if (companyShortcut != null && (StartDate != null && EndDate != null || StartDate != DateTime.MinValue && EndDate != DateTime.MinValue))
				{
					var url = _scrapp.ConstructUrl(companyShortcut, StartDate, EndDate);
					var stockList = _scrapp.ScrapYahooAsync(url);
					var entries = ProcessScrappedData(stockList);

					if (!isAndroid)
					{

						filePath = $"C:\\Users\\Administrator\\Desktop\\{filePath}"; // Todo zmienić path
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
					else
					{
						string csvData = ConvertToCsv(StockDataList);
						using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvData)))
						{
							string fileName = "StockData.csv";
							var fileSaver = FileSaver.Default;

							await fileSaver.SaveAsync(filePath, fileName, stream);

							await Toast.Make("File saved successfully!").Show();
						}
					}
				}

				await ActivateIndicatorAsync(false);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"An error occurred while saving to CSV: {ex.Message}");
			}
		}

		public List<FinancialPoint> ProcessScrappedData(List<StockScrapper_Database.Models.StockData> stockList)
		{
			try
			{
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

				return entries;
			}
			catch (Exception ex)
			{
				return null;
			}

		}

		private async Task ScrapAsync()
		{
			try
			{
				if (SelectedCompany == null)
					return;
				await ActivateIndicatorAsync(true);

				StockDataList.Clear();
				string companyShortcut = SelectedCompany.ToString();

				var url = _scrapp.ConstructUrl(companyShortcut, StartDate, EndDate);
				var stockList = _scrapp.ScrapYahooAsync(url);
				var entries = ProcessScrappedData(stockList);

				//await LoadChartAsync(entries);
				await LoadCandlestickChartAsync(entries);

				await ActivateIndicatorAsync(false);

				//List<string> dateLabels = new List<string>();

				//DateTime currentDate = StockDataList.Min(x => x.Date);
				//while (currentDate <= StockDataList.Max(x => x.Date))
				//{
				//	if (currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
				//	{
				//		dateLabels.Add(currentDate.ToString("yyyy-MM-dd"));
				//	}
				//	currentDate = currentDate.AddDays(1);
				//}


			}
			catch (Exception ex)
			{

			}
		}

		public async Task LoadCandlestickChartAsync(List<FinancialPoint> entries)
		{
			try
			{
				Series = new ISeries[]
				{
					new CandlesticksSeries<FinancialPoint>
					{
						UpFill = new SolidColorPaint(SKColors.ForestGreen),
						UpStroke = new SolidColorPaint(SKColors.DarkGreen) { StrokeThickness = 0 },
						DownFill = new SolidColorPaint(SKColors.Red),
						DownStroke = new SolidColorPaint(SKColors.DarkRed) { StrokeThickness = 0 },
						DataLabelsPadding = new LiveChartsCore.Drawing.Padding
						{
							Left = 3f,  // Increase padding for more space
							Right = 3f  // Increase padding for more space
						},
						MaxBarWidth = 7.5,
						Values = entries
					}
				};


				XAxes = new Axis[]
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
						//UnitWidth = 0.1,
						ForceStepToMin = true,
						//MinStep = 0.1,
						ShowSeparatorLines = true,
						//TicksAtCenter = true,

						Labeler = Labelers.Currency
					}
				};

				IsChartEnabled = true;
			}
			catch (Exception ex)
			{

			}


		}

		public class NodeLinePoint
		{
			public DateTime Date { get; set; }
			public double Value { get; set; }
		}

		

		public async Task LoadChartAsync(List<FinancialPoint> entries)
		{
			try
			{
				if (entries == null || !entries.Any())
				{
					Debug.WriteLine("No data available to load the chart.");
					return;
				}

				foreach (var entry in entries)
				{
					Debug.WriteLine($"Date: {entry.Date}, Open: {entry.Open}, Close: {entry.Close}, High: {entry.High}, Low: {entry.Low}");
				}

				List<DateTimePoint> points = new List<DateTimePoint>();
				foreach (var entry in entries) 
				{
					new DateTimePoint(entry.Date, entry.Close);
				}

				//List<NodeLinePoint> points = new List<NodeLinePoint>();
				//foreach (FinancialPoint entry in entries)
				//{
				//	var nodeLinePoint = new NodeLinePoint
				//	{
				//		Date = entry.Date,
				//		Value = (double)entry.Close
				//	};
				//	points.Add(nodeLinePoint);
				//}

				Series = new ISeries[]
				{
					new LineSeries<DateTimePoint>
					{
						Values = points,
						Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 2 },
						Fill = new SolidColorPaint(SKColors.Red)
					}
				};
				//XAxsis = new Axis[]
				//{
				//	new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("dd mm yyyy"))
				//};

				IsChartEnabled = true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"An error occurred while loading the chart: {ex.Message}");
			}
		}

	}
}