using Maui_FilRouge.Services;
using Maui_FilRouge.Views;
using Microsoft.Extensions.Logging;

namespace Maui_FilRouge;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Services applicatifs
		builder.Services.AddSingleton<ISessionService, SessionService>();
		builder.Services.AddSingleton<IConnectivityService, ConnectivityService>();
		builder.Services.AddSingleton<ILocalCacheService, LocalCacheService>();
		builder.Services.AddSingleton<HttpClient>();
		builder.Services.AddSingleton<IApiClient, ApiClient>();
		builder.Services.AddSingleton<ISyncService, SyncService>();

		// Pages (résolues par Shell via le conteneur DI)
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<OrdersListPage>();
		builder.Services.AddTransient<OrderDetailPage>();
		builder.Services.AddTransient<CatalogPickerPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
