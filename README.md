![Math Converter: A XAML Converter that does it all.](ReadmeAssets/biglogo.png)

Installation:
-------------

`MathConverter` is available on Nuget. There are two packages:

| Nuget Package                                                                          | UI Framework                                                             | Target Frameworks                                                                                                                                     |
|----------------------------------------------------------------------------------------|--------------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------|
| [MathConverter](https://www.nuget.org/packages/MathConverter)                          | [WPF](https://learn.microsoft.com/dotnet/desktop/wpf/overview)           | <ul><li>.NET Framework 3.5+</li><li>.NET Core 3.0+</li></ul>                                                                                          |
| [MathConverter.XamarinForms](https://www.nuget.org/packages/MathConverter.XamarinForms)| [Xamarin.Forms](https://dotnet.microsoft.com/apps/xamarin/xamarin-forms) | <ul><li>.NET Standard 1.0+</li><li>.NET Core 2.0+</li><li>Xamarin.iOS 10+</li><li>MonoAndroid 10+</li><li>UAP 10.0</li><li>Xamarin.Mac 2.0+</li></ul> |

To install MathConverter, run the one of the following commands in the [Package Manager Console](https://docs.microsoft.com/en-us/nuget/tools/package-manager-console):

```
PM> Install-Package MathConverter
PM> Install-Package MathConverter.XamarinForms
```

What is MathConverter?
----------------------

`MathConverter` allows you to do Math in XAML.

`MathConverter` is a powerful `Binding` converter that allows you to specify how to perform conversions directly in XAML, without needing to define a new `IValueConverter` in C# for every single conversion.

Getting Started:
----------------

It's as easy as 1-2-3.

**1)** Install the Nuget package.

**2)** Add a `MathConverter` resource.

```xaml
<Application.Resources>
    <math:MathConverter x:Key="Math" />
</Application.Resources>
```

The `math` namespace is defined as follows<b>*</b>:

```xaml
xmlns:math="http://hexinnovation.com/math"
```

**3)** Do Math. Now, you can use `MathConverter` on any `Binding`. Specify a `ConverterParameter` to specify the rules of the conversion.

> <b>*Note:</b> In some targets (e.g. .NET Standard 1.0), you might have to define the `math` namespace as `xmlns:math="clr-namespace:HexInnovation;assembly=MathConverter.XamarinForms"`

Example: Rounded Rectangle
--------------------------

Suppose we want to make a rounded rectangle. If we create a `Border` and bind bind its `CornerRadius` to its own `ActualHeight`, we end up with a flattened oval ([Full XAML file](src/Demos/WPF/Demos/FlattenedOval.xaml)):

```xaml
<Border CornerRadius={Binding ActualHeight}" … />
```

![If CornerRadius = ActualHeight, we get a flattened oval](ReadmeAssets/Flattened%20Oval.png)

We can use MathConverter to instead bind to `ActualHeight / 2` ([Full XAML file](src/Demos/WPF/Demos/WideRoundedRectangle.xaml)):

```xaml
<Border CornerRadius="{Binding ActualHeight, ConverterParameter=x/2, Converter={StaticResource Math}}" … />
```

![If CornerRadius = ActualHeight / 2, we get a rounded rectangle](ReadmeAssets/Wide%20Rounded%20Rectangle.png)

The simple conversion of `ActualHeight / 2` works well, as long as the rectangle is wider than it is tall. If we need to make a rounded rectangle of an arbitrary size, we need to use a `MultiBinding` to set the `CornerRadius` to the smaller of the `ActualWidth` and the `ActualHeight` divided by two ([Full XAML file](src/Demos/WPF/Demos/TrueRoundedRectangle.xaml)):

```xaml
<Border.CornerRadius>
    <MultiBinding ConverterParameter="Min(x,y)/2" Converter="{StaticResource Math}">
        <Binding Path="ActualHeight" />
        <Binding Path="ActualWidth" />
    </MultiBinding>
</Border.CornerRadius>
```

![If CornerRadius = ActualHeight / 2, we get a rounded rectangle](ReadmeAssets/True%20Rounded%20Rectangle.png)

> **Note:** MathConverter can take any number of Bindings. The first binding's value can be accessed by `x` or `[0]`, the second can be accessed by `y` or `[1]`, and the third can be accessed by `z` or `[2]`. Any value beyond the third can be accessed only by its index: `[3]`, `[4]`, etc.
> 
>**Note:** Instead of using the `Min` function, we could also use the ternary operator: `ConverterParameter = "(x > y ? y : x) / 2"`, but that's a little cumbersome to add to XAML.

Example: Different margins on different sides
---------------------------------------------
You can specify multiple values for types like `CornerRadius`, `Thickness`, `Size`, `Point`, and `Rect`, just like in normal XAML. For example, we can specify different values for vertical/horizontal margins ([Full XAML file](src/Demos/WPF/Demos/DifferentMargins.xaml)):

```xaml
<Rectangle Fill="Green" … Margin="{Binding Source={StaticResource Margin}, ConverterParameter=0;x, Converter={StaticResource Math}}" />
```

![Three rectangles with 20 pixels of margin between them. The middle rectangle has top and bottom margins of 20, but left and right margins of 0.](ReadmeAssets/Evenly-Spaced%20Rectangles.png)

> **Note:** To facilitate entering multiple values into XAML, MathConverter, commas and semicolons are equivalent. We can use either one as separators between the values. So the following margins are equivalent:
> * `ConverterParameter=0;x`
> * `ConverterParameter='0,x'`
> * `ConverterParameter=0;x;0;x`
> * `ConverterParameter='0,x;0,x'`
> * ``ConverterParameter=' $`0,{x},0,{x}` '`` (See [the interpolated strings documentation](#example-interpolated-strings))

Example: No Parameter at all
----------------------------

The `ConverterParameter` is optional. When it is omitted, MathConverter will attempt to convert all of the binding values string-joined with a comma.

In this example, we create two different GridLength (margin) values: one by specifying `Margin` for all four sides, and the other by specifying `Margin` for horizontal margins, and `SmallMargin` for vertical margins ([Full XAML file](src/Demos/WPF/Demos/NoConverterParameter.xaml)).

The first essentially converts as `Margin="20"`. The second converts as `Margin="20,10"`.

```xaml
<Border BorderThickness="1" BorderBrush="Black" Grid.Row="2" Margin="{Binding Source={StaticResource Margin}, Converter={StaticResource Math}}">
    <Border BorderThickness="1" BorderBrush="Red">
        <Border.Margin>
            <MultiBinding Converter="{StaticResource Math}">
                <Binding Source="{StaticResource Margin}" />
                <Binding Source="{StaticResource SmallMargin}" />
            </MultiBinding>
        </Border.Margin>
    </Border>
</Border>
```

![We can convert one, two, or four values to GridLength without specifying a ConverterParameter.](ReadmeAssets/No%20ConverterParameter.png)

Example: Boolean to Visibility
------------------------------

Suppose you want to show a `Control` based on a `Boolean` condition. You can simply bind the `Visibility` parameter and use the ternary operator to convert the boolean to a `Visibility` ([Full XAML file](src/Demos/WPF/Demos/BooleanToVisibility.xaml)):

```xaml
<TextBox Visibility="{Binding IsChecked, ElementName=CheckBox, ConverterParameter='x ? `Visible` : `Collapsed`', Converter={StaticResource Math}}" />
```

![When we toggle the CheckBox, the TextBox appears and disappears](ReadmeAssets/Boolean%20to%20Visibility.gif)

There's a lot going on in this conversion, so let's take this one slowly.

The conversion parameter is `` x ? `Visible` : `Collapsed` ``. MathConverter allows us to input strings very similarly to C#. To more easily facilitate adding strings to XAML, we can use `"` (double quote), `'` (single quote), or `` ` `` (grave) characters to start and end a string.

Suppose that `x` evaluates to true (`CheckBox.IsChecked` is `true`). Then, `` x ? `Visible` : `Collapsed` `` would evaluate to a `System.String` of `"Visible"`. Since we're binding to a property of `Visibility`, MathConverter later converts this value to `Visibility.Visible` for us.

> **Note:** You can backslash-escape characters such as `\t` and `\n` in strings, just like C#. Additionally, you can backslash-escape double quotes, single quotes, and grave characters.
>
> **Note:** All strings must start and end with the same character. So a ConverterParameter of `'Hello, world"` would throw an exception because `'` and `"` do not match, whereas ConverterParameters of `` `Hello, world` ``, `"Hello, world"`, and `'Hello, world'` are equivalent.

Example: Interpolated Strings
-----------------------------
Not only can we include arbitrary strings in the `ConverterParameter`, we can also use interpolated strings to format arbitrary strings ([Full XAML file](src/Demos/WPF/Demos/CountClicks.xaml)):

```xaml
<TextBlock Text="{Binding NumClicks, ConverterParameter='$`You have clicked the button {x} time{(x == 1 ? `` : `s`)}.`', Converter={StaticResource Math}}" />
```

![You have clicked the button x time(s), where x increments with each click](ReadmeAssets/Interpolated%20String%20Animation.gif)

Interpolated strings work the same way as they do in C#. The same rules above apply: a string must start and end with the same character. For example, the following are all valid interpolated strings: `$'Coordinates: ({x:N2},{y:N2}).'`, `$"The weather outside is {x}."`, `` $`Progress: {x:P} complete` ``, whereas the string `$'Invalid"` would throw an exception since `'` and `"` do not match.

> **Note:** Just like in C#, an interpolated string is just a wrapper around a call to the `Format` function (which uses `string.Format`), so the converter parameter `` $`Hello, {x}` `` is equivalent to `Format('Hello, {0}', x)`.

Functions
----------------------------------

We've already alluded to `Min` and `Format`. There are many more functions built into MathConverter, and you can always add your own functions (see [the "Custom Functions" section](#custom-functions)). For now, we're just going to cover some of the functions built into MathConverter.

Functions are case-sensitive (They were not case sensitive in version 1.x).

Functions include:
* `Now()` returns [`System.DateTime.Now`](https://learn.microsoft.com/dotnet/api/system.datetime.now)
* `UnsetValue()` returns [`DependencyProperty.UnsetValue`](https://learn.microsoft.com/dotnet/api/system.windows.dependencyproperty.unsetvalue) or [`BindableProperty.UnsetValue`](https://learn.microsoft.com/dotnet/api/xamarin.forms.bindableproperty.unsetvalue)
* `Cos(x)`, `Sin(x)`, `Tan(x)`, `Abs(x)`, `Acos(x)`/`ArcCos(x)`, `Asin(x)`/`ArcSin(x)`, `Atan(x)`/`ArcTan(x)`, `Ceil(x)`/`Ceiling(x)`, `Floor(x)`, `Sqrt(x)`, `Log(x, y)`, `Atan2(x, y)`/`ArcTan2(x, y)`, `Round(x)`/`Round(x, y)` all behave like their counterparts in [`System.Math`](https://learn.microsoft.com/dotnet/api/system.math). They return `null` if at least one argument is `null`.
* `Deg(x)`/`Degrees(x)` returns `x / pi * 180`
* `Rad(x)`/`Radians(x)` returns `x / 180 * pi`
* `ToLower(x)`/`LCase(x)` returns ``$"{x}".ToLower()``
* `ToUpper(x)`/`UCase(x)` returns ``$"{x}".ToUpper()``
* `TryParseDouble(x)` will attempt to cast/convert `x` to `double` or cast/convert `x` to `string` and parse it to double. The function returns `null` if it fails to convert the input.
* `StartsWith(x, y)` will return `true` or `false` if it can cast/convert `x` to string, based on if `x` starts with `y` or `$"{y}"`. If `x` is not a string or `$"{y}".Length` is `0`, the function returns `null` instead.
* `EndsWith(x, y)` behaves the same way as `StartsWith` except it detects if `x` _ends_ with `y`.
* `Contains(x, y)` is a bit different. `x` can be an `IEnumerable`, in which case we check to see if it contains `y`, or if `x` is a string, the function checks if `x` contains `$"{y}"`. If `$"{y}".Length` is zero (but notably, not if `y == ""`), then the function returns `null` instead.
* `IsNull(x, y)`/`IfNull(x, y)` are equivalent to `x ?? y`
* `And()`, `Or()`, and `Nor()` each accept an arbitrary number of functions. They use reflection to call the logical operators UnaryNot (`Nor(x, y)` evaluates as `!Or(x, y)`), BitwiseAnd, and BitwiseOr. This means that we can accept and return non-boolean values, provided that their types would compile with `&&`, `||`, and `!` in C#. We only evaluate as many parameters as we need to. For example, `And(…)` will evaulate parameters only until it encounters a false value, in which case it will return the false value.
* `Max()`, `Min()`, and `Avg()`/`Average()` ignore values that can't be converted to double, and return `null` if no they do not encounter any numeric values.
* `Format()` simply returns [`string.Format`](https://learn.microsoft.com/dotnet/api/system.string.format)
* `Concat()` simply returns [`string.Concat`](https://learn.microsoft.com/dotnet/api/system.string.concat).
* `Join()` simply returns [`string.Join`](https://learn.microsoft.com/dotnet/api/system.string.join).
* `GetType(x)` simply returns `x?.GetType()`
* `ConvertType(x, y)` will do whatever it can to convert `x` to type `y`. If `y` is not a `Type` or `x` cannot be converted, the function returns `x` instead. Because TypeConverters are inconsistent, we always use [InvariantCulture](https://learn.microsoft.com/dotnet/api/system.globalization.cultureinfo.invariantculture) when converting.
* `EnumEquals(x, y)` will see if two enum values are equal. Example use cases: ``EnumEquals(x, `Visible`)``, ``EnumEquals(`Visible`, x)``. If two enum values are different types are compared, `EnumEquals` will return `false`, even if `x.Equals(y)` is true for the same inputs in C#.
* `Throw()` will throw an exception when evaluated. The exception contains helpful information for debugging issues with a conversion.
* `TryCatch()` takes two or more arguments, and returns immediately as soon as it finds an argument that does not throw an exception. If every argument throws an exception, `TryCatch()` will not catch the last exception.

Using these operators, you can do very powerful things. One such example ([Full XAML file](src/Demos/WPF/Demos/NowPlusSixHours.xaml)):

```xaml
<TextBlock Text="{Binding Source={x:Type sys:TimeSpan}, ConverterParameter='$`Six hours from now, the time will be {Now() + ConvertType(`6:00:00`, x):h\':\'mm\':\'ss tt}`', Converter={StaticResource Math}}" />
```

We use `ConvertType` to convert `"6:00:00"` from string to `TimeSpan`, then add that `TimeSpan` to the current time, and format it with the format string `"h:mm:ss tt"`.

![Six hours from now, the time will be 12:35:09 AM](ReadmeAssets/Now%20Plus%20Six%20Hours.png)


Custom Functions
----------------

MathConverter's built-in functions are implemented in [CustomFunctions.cs](src/CSharp/MathConverter/CustomFunctions.cs). Those classes can be used as examples to follow to create your own custom functions. This allows you to effectively extend MathConverter to do whatever you want.

The main window of our demo app is a perfect example ([Full XAML file](src/Demos/WPF/MainWindow.xaml)).

We have a ListBox with Types added.

```xaml
<ListBox>
    <ListBox.Items>
        <x:Type TypeName="demos:FlattenedOval" />
        <x:Type TypeName="demos:WideRoundedRectangle" />
        <x:Type TypeName="demos:TrueRoundedRectangle" />
        <!-- More Types -->
    </ListBox.Items>

    <!-- More stuff -->
</ListBox>
```

The `ListBox` uses a `DataTemplate` to show a `TextBox` for each item. We use the custom function `GetWindowTitle()` to convert the `Type`s to a display value.

```xaml
<TextBlock Text="{Binding ConverterParameter='GetWindowTitle(x)', Converter={StaticResource Math}}" />
```

The `GetWindowTitle` function is added to MathConverter as follows:

```xaml
<Window.Resources>
    <math:MathConverter x:Key="Math">
        <!-- "GetWindowTitle" in the parameter will invoke the `GetWindowTitleFunction` function. -->
        <math:CustomFunctionDefinition Name="GetWindowTitle" Function="functions:GetWindowTitleFunction" />
    </math:MathConverter>
</Window.Resources>
```

`GetWindowTitleFunction` is defined as follows ([Full C# file](src/Demos/WPF/CustomFunctions/GetWindowTitle.cs)):

```c#
public class GetWindowTitleFunction : OneArgFunction
{
    public override object Evaluate(CultureInfo cultureInfo, object argument)
    {
        return argument is Type t && t.IsAssignableTo(typeof(Window)) ? ((Window)Activator.CreateInstance(t)).Title : null;
    }
}
```

 So, our `GetWindowTitleFunction` instantiates the `Type` and get the `Title` property of the resulting `Window`.

![The GetWindowTitle function converts the Types to display names for us](ReadmeAssets/Custom%20Function.png)

In this example, `GetWindowTitleFunction` extends `OneArgFunction`, but all that matters is that we extend `CustomFunction`. It is recommended that you implement one of its predefined subclasses:
* ZeroArgFunction
* OneArgFunction
* OneDoubleFunction
* TwoArgFunction
* ArbitraryArgFunction

Again, there are plenty of examples in [CustomFunctions.cs](src/CSharp/MathConverter/CustomFunctions.cs).

Overriding Built-In Functions
-----------------------------

Suppose you don't like how a function is implemented. You can always override the function with your own custom function.

As a concrete example, we can implement a [`CustomAverageFunction`](src/Demos/WPF/CustomFunctions/CustomAverageFunction.cs) function. This is similar to MathConverter's built-in `AverageFunction`, except that it rounds each input, instead of simply taking the average.

```xaml
<TextBlock x:Name="TextBlock" Text="{Binding ConverterParameter='`Average(1, 1.5) returns ` + Average(1, 1.5)', Converter={StaticResource Math}}" />
```

![The Average function changes each time the RadioButton is changed](ReadmeAssets/Custom%20Function%20Animation.gif)

With [a little bit of code-behind](src/Demos/WPF/Demos/CustomAverageFunction.xaml.cs), we can remove and replace the `Average` function with our `CustomAverageFunction`:

```c#
private void RadioButton_Changed(object sender, RoutedEventArgs e)
{
    if (!(FindResource("Math") is HexInnovation.MathConverter math))
        return;

    if (UseStockFunction.IsChecked == true)
    {
        // Go back to the stock function.
        math.CustomFunctions.Clear();
        math.CustomFunctions.RegisterDefaultFunctions();
    }
    else
    {
        // Remove the default Average function and define our own.
        math.CustomFunctions.Remove("Average");
        math.CustomFunctions.Add(CustomFunctionDefinition.Create<MyCustomAverageFunction>("Average"));
    }

    // Tell the TextBlock to refresh its binding again.
    TextBlock?.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
}
```

> **Note:** In this example, the built-in `Average` function is still available with the name `Avg`. Most functions are not defined with multiple names (see [the "Functions" section](#functions)).

Syntax
------

MathConverter's `ConverterParameter` syntax is very similar to C#, so you can generally expect it to behave just like C#. We follow [the standard C# rules regarding operator ordering](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/#operator-precedence), except as noted below:

* Since `MathConverter` is specifically designed to perform math calculations, the caret (`^`) operator does not perform the `XOR` operation. Rather, it is an exponent symbol. It uses [`System.Math.Pow`](https://learn.microsoft.com/dotnet/api/system.math.pow) to evaluate expressions, and its precedence is just above multiplicative operations (`*`, `/`, and `%`).
* The multiplication operator can often be safely ommitted. A `ConverterParameter` value of `xyz` will evaluate to `x*y*z`. The parameter `x2y` will evaluate to `x^2*y` (or equivalently, `xxy` or `x*x*y`). Similarly, `2x3` is equivalent to `2*x^3` or `2*x*x*x`. Note that `x(2)` is equivalent to `x*(2)`, in the same way that `x(y+z)` is equivalent to `x*(y+z)`. Note that `1/xy` will evaluate to `1/x*y`, not to `1/(x*y)`, as you might expect.
* `MathConverter` doesn't support all of the operations that C# does. The following operators are examples of those not supported:
    * Assignment operators (`=`, `+=`, `&&=`, etc)
    * Logical operators (`|`, `&`, and `^` as `XOR`)
         - Note that `||` and `&&` are supported operators.
    * `switch` and `with` expressions are not supported.
    * `is` and `as` (since Types are not supported)
    * Bitwise operations (`<<`, `>>`, `~`) are not supported.
    * The unary operators `++` and `--` are not supported, since they change the values of the inputs.
    * Primary operators (`x.y`, `f(x)`, `a[i]`, `new`, `typeof`, `checked`, `unchecked`, `default`, `nameof`, `sizeof`) are not supported.

MathConverter uses reflection to evaluate operator calls, so you can use custom types with custom operator implementations and MathConverter will use those operators while converting.

Numeric Types
-------------
Generally, MathConverter will favor using `double` values over other numeric types. When evaluating which operator to call, `MathConverter` will convert any operands to `double`, if possible, before calling the operator. If an input is of type `char`, it will convert to `int` then convert to `double`. Where a path to implicitly convert an operand to `double` exists, MathConverter will convert for you in order to apply an operator that takes numeric inputs.

Hence, supposing `x = 1` (an integer), C# would evaluate that `1 + x/2 = 1`, since `(int)1 / 2 = 0`. MathConverter will implicitly converter all variables to doubles. So, the expression `1 + x/2` is evaluated as `1.0 + (double)x/2.0`, so MathConverter will return `1.5`.

Parser
------

Each time a conversion must be made, MathConverter must parse and evaluate an expression. When it parses an expression, it reads through the string one character at a time, and returns a syntax tree. The parsing is done in the `Parser` class. The `Parser` returns an `AbstractSyntaxTree` for each comma-separated (or semicolon-separated) value. In an effort to improve efficiency, `MathConverter` uses a cache to save the `AbstractSyntaxTree`s for each string it evaluates. Therefore, if you have a lot of conversion strings, it is discouraged to use the same `MathConverter` instance across your entire application. It is a better idea to use a different `MathConverter` object for each `UserControl`, `Page`, or `Window`. You can turn off caching on a per-instance basis:

```xaml
<math:MathConverter x:Key="nocache" UseCache="False" />
```

Breaking Changes From V1
-------------------------
There are a few breaking changes from version 1.

* Function names are now case-sensitive.
* `e`, `pi`, `null`, `true`, and `false` keywords are now required to be lower-case.
* `VisibleOrCollapsed` and `VisibleOrHidden` functions were deprecated, and will be removed in a future release. You should change your conversions from `VisibleOrCollapsed(x)` to `` x ? `Visible` : `Collapsed` ``
* There are several small differences in how/when types are converted. For example, we no longer convert from int to double unless it needs to be used as an operand in an operator such as `+`, `*`, etc.
