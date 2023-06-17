using Microsoft.Maui.Hosting;
using Xunit.Runners.Maui;

namespace HexInnovation.UnitTests.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp() =>
        MauiApp.CreateBuilder()
        .ConfigureTests(new TestOptions
        {
            Assemblies =
            {
                typeof(MauiProgram).Assembly
            }
        })
        .UseVisualRunner()
        .Build();
}
