namespace HerbavitaLogistics;

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

		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddSingleton<IScanContext, ScanContext>();
		builder.Services.AddHttpClient<IApiService, ApiService>();
		builder.Services.AddApiService(); // With retry policies
		builder.Services.AddSingleton(AuthState.Current);
		builder.Services.AddSingleton<IScanContext, ScanContext>();
		builder.Services.AddSingleton<ScannerStateService>();

#if DEBUG
builder.Services.AddBlazorWebViewDeveloperTools();
#endif

		var app = builder.Build();

		// Initialize auth state
		await AuthState.InitializeAsync();

        // Initialize offline storage
        await LocalDataStore.Init();



		return builder.Build();
	}
}
