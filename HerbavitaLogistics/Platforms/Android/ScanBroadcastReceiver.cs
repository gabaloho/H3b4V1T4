[BroadcastReceiver(Enabled = true, Exported = true)]
[IntentFilter(new[] { "com.yourcompany.warehouse.SCAN" })]
public class ScanBroadcastReceiver : BroadcastReceiver
{
    public override void OnReceive(Context context, Intent intent)
    {
        var scanData = intent.GetStringExtra("com.symbol.datawedge.data_string");
        if (!string.IsNullOrEmpty(scanData))
        {
            MessagingCenter.Send(Xamarin.Forms.Application.Current, "ScanReceived", scanData);
        }
    }
}