using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.SKCharts;
using SkiaSharp;

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
			candlestickChart.WidthRequest = 1000;
			candlestickChart.HeightRequest = 600;
		}
		candlestickChart.TooltipPosition= LiveChartsCore.Measure.TooltipPosition.Left;
		BindingContext = _viewModel;
	}


	private async void CreateImageFromCartesianControl()
	{
		if (candlestickChart == null)
			return;

		try
		{
			// Create an SKCartesianChart from your existing chart
			var skChart = new SKCartesianChart(candlestickChart)
			{
				Width = (int)candlestickChart.WidthRequest,
				Height = (int)candlestickChart.HeightRequest,
			};

			// Convert the chart to an image
			using (var image = skChart.GetImage())
			using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
			{
				var imageData = data.ToArray();
				string filePath = string.Empty;

#if WINDOWS
				var savePicker = new Windows.Storage.Pickers.FileSavePicker();
				var hwnd = ((MauiWinUIWindow)App.Current.Windows[0].Handler.PlatformView).WindowHandle;
				WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

				savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
				savePicker.FileTypeChoices.Add("PNG Image", new List<string>() { ".png" });
				savePicker.SuggestedFileName = "CartesianImageFromControl.png";

				var file = await savePicker.PickSaveFileAsync();
				if (file != null)
				{
					filePath = file.Path;

					using (var stream = await file.OpenStreamForWriteAsync())
					{
						stream.Write(imageData, 0, imageData.Length);
					}
				}
				else
				{
					await DisplayAlert("Canceled", "Image save operation was canceled.", "OK");
					return;
				}
#elif ANDROID
                filePath = Path.Combine(FileSystem.AppDataDirectory, "CartesianImageFromControl.png");
                await File.WriteAllBytesAsync(filePath, imageData);

				await DisplayAlert("Success", $"Chart saved as image to {filePath}", "OK");
#endif
			}
		}
		catch (Exception ex)
		{
			// Handle any errors that may occur
			await DisplayAlert("Error", $"Failed to save chart image: {ex.Message}", "OK");
		}
	}


	private void ScrapButton_Clicked(object sender, EventArgs e)
	{
		_viewModel.ScrapCommand.Execute(null);
	}

	private void CSVButton_Clicked(object sender, EventArgs e)
	{
		_viewModel.GenerateCSVCommand.Execute(null);
	}

	private void Button_Clicked(object sender, EventArgs e)
	{
		CreateImageFromCartesianControl();
	}
}