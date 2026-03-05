using HexInnovation;
using System.Globalization;

namespace MathConverter.Demo.CustomFunctions;

public class GetPageTitleFunction : OneArgFunction
{
    public override object? Evaluate(CultureInfo cultureInfo, object? argument) =>
        argument is Type t && t.IsAssignableTo(typeof(Page)) ? ((Page)Activator.CreateInstance(t)!).Title : null;
}
