using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if XAMARIN
using Xamarin.Forms;
#elif MAUI
using Microsoft.Maui.Controls;
#else
using System.Windows;
using System.Windows.Markup;
#endif

[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: XmlnsPrefix("http://hexinnovation.com/math", "math")]
#if !WINDOWS_UWP && !NETSTANDARD1_0 && !NETSTANDARD1_3
[assembly: XmlnsDefinition("http://hexinnovation.com/math", "HexInnovation")]
#endif

[assembly: InternalsVisibleTo("MathConverter.UnitTests,PublicKey=" +
    "0024000004800000940000000602000000240000525341310004000001000100056bb3f4bc6f27" +
    "a583fb5713ddbe24f2dabdf9688b60147eca177159a995ef153b183156c4566b457819661af3a1" +
    "b6810a9cae7928ccb10b834de2eaa99c133f2c0540f77cd43040853d166227d6bb252618b95ad3" +
    "0e3e5a1487c19bb9854e94edadb6c5fb2d2eaf771edb3d290a655bfc8c9eb852855f8d339ab102" +
    "78bf44be")]
