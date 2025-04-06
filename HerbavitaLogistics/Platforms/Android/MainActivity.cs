using Android.App;
using Android.Content.PM;
using Android.OS;

namespace HerbavitaLogistics;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{

    // Platforms/Android/MainActivity.cs
    protected override void OnResume()
    {
        base.OnResume();

        // Check DataWedge status before configuring
        var dwStatusIntent = new Intent();
        dwStatusIntent.SetAction("com.symbol.datawedge.api.ACTION");
        dwStatusIntent.PutExtra("com.symbol.datawedge.api.GET_DATAWEDGE_STATUS", "");
        SendBroadcast(dwStatusIntent);

        // Configure profile only after confirming DataWedge is ready
        DataWedgeProfileHelper.ConfigureDataWedgeProfile(
            "WarehouseApp",
            PackageName,
            Class.Name);
    }
}
