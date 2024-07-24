using LiveChartsCore.SkiaSharpView.Drawing;

namespace StockScrapper.Panels.Tabs;

public partial class YahooStockPriceChartsTab : ContentPage
{
	private readonly YahooStockPriceViewModel _viewModel;
	public YahooStockPriceChartsTab()
	{
		InitializeComponent();
		_viewModel = new YahooStockPriceViewModel();

		if(DeviceInfo.Current.Platform == DevicePlatform.Android)
		{
			candlestickChart.WidthRequest = 400;
			candlestickChart.HeightRequest = 450;
		}
		else
		{
			candlestickChart.WidthRequest = 1200;
			candlestickChart.HeightRequest = 400;
		}
		candlestickChart.TooltipPosition= LiveChartsCore.Measure.TooltipPosition.Left;
		BindingContext = _viewModel;
	}

	private void ScrapButton_Clicked(object sender, EventArgs e)
	{
		_viewModel.ScrapCommand.Execute(null);
	}

	private void CSVButton_Clicked(object sender, EventArgs e)
	{
		_viewModel.GenerateCSVCommand.Execute(null);
	}
}