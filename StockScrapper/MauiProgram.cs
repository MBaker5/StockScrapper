using HtmlAgilityPack;
using Microcharts.Maui;
using Microsoft.Extensions.Logging;
using StockScrapper_App.Core;
using StockScrapper_App.Services;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Mopups;
using Mopups.Hosting;

namespace StockScrapper
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.UseSkiaSharp(true)
				.UseMicrocharts()
				.ConfigureMopups()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				})
				.Services.AddSingleton<IHtmlScrappService, HtmlScrappService>();
				
#if DEBUG
			builder.Logging.AddDebug();
#endif

			return builder.Build();
		}
	}
}
