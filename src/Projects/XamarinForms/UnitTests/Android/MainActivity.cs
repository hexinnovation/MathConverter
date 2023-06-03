using Android.App;
using Android.Content.PM;
using Android.OS;
using MSTestX;

namespace MathConverter.UnitTests.Android
{
    [Activity(Name = "MathConverter.RunTestsActivity", Label = "MathConverter.UnitTests", Icon = "@mipmap/ic_launcher", Theme = "@style/AppTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : TestRunnerActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.tab_bar;
            ToolbarResource = Resource.Layout.tool_bar;

            base.OnCreate(bundle);
        }
    }
}