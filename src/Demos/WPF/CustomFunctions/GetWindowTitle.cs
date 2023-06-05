using HexInnovation;
using System;
using System.Globalization;
using System.Windows;

namespace MathConverter.Demo.CustomFunctions;

public class GetWindowTitleFunction : OneArgFunction
{
    public override object Evaluate(CultureInfo cultureInfo, object argument)
    {
        return argument is Type t && t.IsAssignableTo(typeof(Window)) ? ((Window)Activator.CreateInstance(t)).Title : null;
    }
}
