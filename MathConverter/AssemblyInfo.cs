using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if XAMARIN
using Xamarin.Forms;
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

[assembly: InternalsVisibleTo("MathConverter.UnitTests")]