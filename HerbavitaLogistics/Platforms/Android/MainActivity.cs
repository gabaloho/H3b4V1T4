using Android.App;
using Android.Content.PM;
using Android.OS;

namespace HerbavitaLogistics;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{

    /// Platforms/Android/MainActivity.cs
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Configure DataWedge on startup
        DataWedgeProfileHelper.ConfigureProfile(
            "LogisticsApp",
            PackageName,
            $"{PackageName}.{nameof(MainActivity)}");
    }

    protected override void OnResume()
    {
        base.OnResume();
        DataWedgeProfileHelper.ToggleScanner(true); // Enable scanner when app is foregrounded
    }

    protected override void OnPause()
    {
        DataWedgeProfileHelper.ToggleScanner(false); // Disable scanner when app is backgrounded
        base.OnPause();
    }
}
