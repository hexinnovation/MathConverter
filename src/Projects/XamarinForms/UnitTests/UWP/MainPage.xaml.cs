using NUnit.Runner.Services;
using System.Reflection;

namespace MathConverter.UnitTests.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            // Windows Universal will not load all tests within the current project,
            // you must do it explicitly below
            var nunit = new NUnit.Runner.App();

            // If you want to add tests in another assembly, add a reference and
            // duplicate the following line with a type from the referenced assembly
            nunit.AddTestAssembly(typeof(MainPage).GetTypeInfo().Assembly);

            // Available options for testing
            nunit.Options = new TestOptions
            {
                // If True, the tests will run automatically when the app starts
                // otherwise you must run them manually.
                AutoRun = true,

                // Creates a NUnit Xml result file on the host file system using PCLStorage library.
                CreateXmlResultFile = false,
            };

            LoadApplication(nunit);
        }
    }
}
