// Platforms/Android/ScanBroadcastReceiver.cs
[BroadcastReceiver(Enabled = true, Exported = true)]
[IntentFilter(new[] {
    "com.herbavita.logistics.SCAN",  // Custom intent action
    "com.symbol.datawedge.api.NOTIFICATION_ACTION"  // For status updates
})]
public class ScanBroadcastReceiver : BroadcastReceiver
{
    public override void OnReceive(Context context, Intent intent)
    {
        // Handle scan data
        if (intent.Action == "com.herbavita.logistics.SCAN")
        {
            var barcode = intent.GetStringExtra("com.symbol.datawedge.data_string");
            var symbology = intent.GetStringExtra("com.symbol.datawedge.label_type");

            if (!string.IsNullOrEmpty(barcode))
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "ScanReceived", barcode);
        }

        // Handle DataWedge notifications
        if (intent.Action == "com.symbol.datawedge.api.NOTIFICATION_ACTION")
        {
            var status = intent.GetStringExtra("com.symbol.datawedge.api.NOTIFICATION");
            // Process scanner status changes
        }
    }
}