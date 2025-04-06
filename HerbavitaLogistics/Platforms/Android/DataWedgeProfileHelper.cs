
public static class DataWedgeProfileHelper
{
    public static void ConfigureProfile(string profileName, string packageName, string activityName)
    {
        var profileConfig = new Bundle();
        profileConfig.PutString("PROFILE_NAME", profileName);
        profileConfig.PutString("PROFILE_ENABLED", "true");

        var appConfig = new Bundle();
        appConfig.PutString("PACKAGE_NAME", packageName);
        appConfig.PutStringArray("ACTIVITY_LIST", new[] { activityName });
        profileConfig.PutBundle("APP_LIST", appConfig);

        var intentConfig = new Bundle();
        intentConfig.PutString("ACTION", "com.yourcompany.warehouse.SCAN");
        intentConfig.PutString("INTENT_DELIVERY", "2");
        profileConfig.PutBundle("PLUGIN_CONFIG", intentConfig);

        var intent = new Intent();
        intent.SetAction("com.symbol.datawedge.api.ACTION");
        intent.PutExtra("com.symbol.datawedge.api.EXTRA_PARAM", profileConfig);
        Android.App.Application.Context.SendBroadcast(intent);
    }
}

