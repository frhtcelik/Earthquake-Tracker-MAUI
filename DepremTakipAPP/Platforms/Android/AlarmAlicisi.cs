using Android.App;
using Android.Content;
using System.Diagnostics;

namespace DepremTakipAPP.Platforms.Android
{
    [BroadcastReceiver(Enabled = true)]
    public class AlarmAlicisi : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Debug.WriteLine(">>> AlarmAlicisi: Alarm alındı, servis başlatılıyor.");
            var serviceIntent = new Intent(context, typeof(DepremKontrolServisi));
            context.StartService(serviceIntent);
        }
    }
}