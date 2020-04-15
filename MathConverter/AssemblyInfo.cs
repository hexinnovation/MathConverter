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
[assembly: XmlnsDefinition("http://hexinnovation.com/math", "HexInnovation")]

[assembly: InternalsVisibleTo("MathConverter.UnitTests")]