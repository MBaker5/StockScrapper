using Microsoft.Maui.Controls;

namespace StockScrapper.Panels;

public partial class YahooStockPrice : ContentPage
{
	private readonly YahooStockPriceViewModel _viewModel;

	public YahooStockPrice()
	{
		InitializeComponent();
		_viewModel = new YahooStockPriceViewModel();
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
