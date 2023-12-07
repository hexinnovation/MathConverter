using Android.App;
using Android.OS;
using Android.Runtime;
using MSTestX;

namespace MathConverter.UnitTests.Android
{
    [Activity(Label = "MathConverter.UnitTests", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : TestRunnerActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
    }
}