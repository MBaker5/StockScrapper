namespace StockScrapper.Panels.Tabs;

public partial class YahooStockPriceChartsTab : ContentPage
{
	private readonly YahooStockPriceViewModel _viewModel;
	public YahooStockPriceChartsTab()
	{
		InitializeComponent();
		_viewModel = new YahooStockPriceViewModel();
		BindingContext = _viewModel;
	}

	private void ScrapButton_Clicked(object sender, EventArgs e)
	{
		_viewModel.ScrapCommand.Execute(null);
	}
}