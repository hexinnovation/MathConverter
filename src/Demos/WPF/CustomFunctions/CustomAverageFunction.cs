using HexInnovation;
using System;
using System.Globalization;
using System.Linq;

namespace MathConverter.Demo.CustomFunctions;

sealed class MyCustomAverageFunction : ArbitraryArgFunction
{
    public override object Evaluate(CultureInfo cultureInfo, Func<object>[] arguments)
    {
        var args = arguments.Select(x => TryConvert<double>(x(), out var d) ? Math.Round(d) : new double?())
            .Where(x => x.HasValue).Select(x => x.Value).ToList();

        return args.Count == 0 ? new double?() : args.Average();
    }
}
