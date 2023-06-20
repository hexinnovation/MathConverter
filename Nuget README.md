[![Math Converter: A XAML Converter that does it all.](https://raw.githubusercontent.com/hexinnovation/MathConverter/main/ReadmeAssets/Banner.svg)](https://github.com/hexinnovation/MathConverter)

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

The `math` namespace is defined as follows:

```xaml
xmlns:math="http://hexinnovation.com/math"
```

**3)** Do Math. Now, you can use `MathConverter` on any `Binding`. Specify a `ConverterParameter` to specify the rules of the conversion.

```xaml
<Border CornerRadius="{Binding ActualHeight, ConverterParameter=x/2, Converter={StaticResource Math}}" />
```
Or, for conversions with multiple bindings.

```xaml
<Border CornerRadius="{math:Convert 'Min(x,y)/2', x={Binding ActualHeight}, y={Binding ActualWidth}}" />
```

See [the GitHub repository](https://github.com/hexinnovation/MathConverter) for documentation and examples.
