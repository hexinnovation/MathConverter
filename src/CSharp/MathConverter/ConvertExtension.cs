using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

#if WPF
using BindableProperty = System.Windows.DependencyProperty;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
#elif MAUI
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
#elif XAMARIN
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
#endif

namespace HexInnovation;


/// <summary>
/// A wrapper around MultiBinding that simplifies the syntax of creating a multi-input MathConverter binding.
/// </summary>
#if WPF
[MarkupExtensionReturnType(typeof(object))]
[Localizability(LocalizationCategory.None, Modifiability = Modifiability.Unmodifiable, Readability = Readability.Unreadable)]
#else
[ContentProperty(nameof(Expression))]
[AcceptEmptyServiceProvider]
#endif
public sealed class ConvertExtension
#if WPF
    : MarkupExtension
#else
    : IMarkupExtension<BindingBase>
#endif
{
    /// <summary>
    /// This is the default <see cref="MathConverter"/> used by the <see cref="ConvertExtension"/>. 
    /// </summary>
    public static MathConverter DefaultConverter { get; }

    static ConvertExtension()
    {
#if WPF
        if (Application.Current?.TryFindResource("Math") is MathConverter mathConverter)
#else
        if (Application.Current?.Resources.TryGetValue("Math", out var obj) == true && obj is MathConverter mathConverter)
#endif
            DefaultConverter = mathConverter;
        else
            DefaultConverter = new();
    }

    /// <summary>
    /// The <see cref="MultiBinding"/> of which we are a wrapper.
    /// </summary>
    internal readonly MultiBinding _binding;

    /// <summary>
    /// A binding to <see cref="BindableProperty.UnsetValue"/> that is used if we skip a value when adding bindings.
    /// </summary>
    private static readonly Binding unsetValueBinding = new Binding { Source = BindableProperty.UnsetValue };

    /// <summary>
    /// Creates a new ConvertExtension
    /// </summary>
    public ConvertExtension()
    {
        _binding = new MultiBinding { Converter = DefaultConverter, Mode = BindingMode.OneWay };
    }


#if WPF
    /// <summary>
    /// Creates a new ConvertExtension
    /// </summary>
    /// <param name="expression">The ConverterParameter</param>
    public ConvertExtension(string expression)
        : this()
    {
        Expression = expression;
    }

    /// <summary>
    /// Update type
    /// </summary>
    [DefaultValue(UpdateSourceTrigger.PropertyChanged)]
    public UpdateSourceTrigger UpdateSourceTrigger
    {
        get => _binding.UpdateSourceTrigger;
        set => _binding.UpdateSourceTrigger = value;
    }

    /// <summary>
    /// Raise SourceUpdated event whenever a value flows from target to source
    /// </summary>
    [DefaultValue(false)]
    public bool NotifyOnSourceUpdated
    {
        get => _binding.NotifyOnSourceUpdated;
        set => _binding.NotifyOnSourceUpdated = value;
    }

    /// <summary>
    /// Raise TargetUpdated event whenever a value flows from source to target
    /// </summary>
    [DefaultValue(false)]
    public bool NotifyOnTargetUpdated
    {
        get => _binding.NotifyOnTargetUpdated;
        set => _binding.NotifyOnTargetUpdated = value;
    }

    /// <summary>
    /// Raise ValidationError event whenever there is a ValidationError on Update
    /// </summary>
    [DefaultValue(false)]
    public bool NotifyOnValidationError
    {
        get => _binding.NotifyOnValidationError;
        set => _binding.NotifyOnValidationError = value;
    }

    /// <summary>
    /// Converter to convert the source values to the target value
    /// </summary>
    public MathConverter Converter
    {
        get => _binding.Converter as MathConverter;
        set => _binding.Converter = value;
    }

    /// <summary>
    /// Culture in which to evaluate the converter
    /// </summary>
    [DefaultValue(null)]
    [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
    public CultureInfo ConverterCulture
    {
        get => _binding.ConverterCulture;
        set => _binding.ConverterCulture = value;
    }

    /// <summary>
    ///     <see cref="Collection&lt;ValidationRule&gt;"/> is a collection of <see cref="ValidationRule"/>
    ///     instances on this MultiBinding. Each of the rules is checked for validity on update
    /// </summary>
    public Collection<ValidationRule> ValidationRules => _binding.ValidationRules;


    /// <summary>
    /// True if an exception during source updates should be considered a validation error.
    /// </summary>
    [DefaultValue(false)]
    public bool ValidatesOnExceptions
    {
        get => _binding.ValidatesOnExceptions;
        set => _binding.ValidatesOnExceptions = value;
    }

    /// <summary>
    /// True if a data error in the source item should be considered a validation error.
    /// </summary>
    [DefaultValue(false)]
    public bool ValidatesOnDataErrors
    {
        get => _binding.ValidatesOnDataErrors;
        set => _binding.ValidatesOnDataErrors = value;
    }

    /// <summary>
    /// True if a data error from INotifyDataErrorInfo source item should be considered a validation error.
    /// </summary>
    [DefaultValue(true)]
    public bool ValidatesOnNotifyDataErrors
    {
        get => _binding.ValidatesOnDataErrors;
        set => _binding.ValidatesOnDataErrors = value;
    }

    /// <summary>
    /// Name of the <see cref="BindingGroup"/> this binding should join.
    /// </summary>
    [DefaultValue("")]
    public string BindingGroupName
    {
        get => _binding.BindingGroupName;
        set => _binding.BindingGroupName = value;
    }


    /// <summary>
    /// Return the value to set on the property for the target for this
    /// binding.
    /// </summary>
    public sealed override object ProvideValue(IServiceProvider serviceProvider) => _binding.ProvideValue(serviceProvider);
#else
    /// <summary>
    /// Returns a MultiBinding with a x<see cref="MathConverter"/> to convert the value as specified.
    /// </summary>
    /// <param name="serviceProvider">The service that provides the value.</param>
    /// <returns>A MultiBinding that uses a MathConverter</returns>
    public BindingBase ProvideValue(IServiceProvider serviceProvider) => _binding;
    /// <summary>
    /// Returns a MultiBinding with a x<see cref="MathConverter"/> to convert the value as specified.
    /// </summary>
    /// <param name="serviceProvider">The service that provides the value.</param>
    /// <returns>A MultiBinding that uses a MathConverter</returns>
    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
#endif


    /// <summary>
    /// The parameter to pass to converter.
    /// </summary>
    [DefaultValue(null)]
    public string Expression
    {
        get => _binding.ConverterParameter as string;
        set => _binding.ConverterParameter = value;
    }


    /// <summary>
    ///     Value to use when source cannot provide a value
    /// </summary>
    /// <remarks>
    ///     Initialized to DependencyProperty.UnsetValue; if FallbackValue is not set, BindingExpression
    ///     will return target property's default when Binding cannot get a real value.
    /// </remarks>
    public object FallbackValue
    {
        get => _binding.FallbackValue;
        set => _binding.FallbackValue = value;
    }


    /// <summary>
    /// Value used to represent "null" in the target property.
    /// </summary>
    public object TargetNullValue
    {
        get => _binding.TargetNullValue;
        set => _binding.TargetNullValue = value;
    }


    private void SetBinding(int index, BindingBase binding)
    {
        while (_binding.Bindings.Count < index)
            _binding.Bindings.Add(unsetValueBinding);

        if (_binding.Bindings.Count == index)
            _binding.Bindings.Add(binding);
        else
            _binding.Bindings[index] = binding;
    }

    /// <summary>
    /// The first variable (accessed by [0] or x)
    /// </summary>
    public BindingBase x
    {
        get => _binding.Bindings[0];
        set => SetBinding(0, value);
    }
    /// <summary>
    /// The second variable (accessed by [1] or y)
    /// </summary>
    public BindingBase y
    {
        get => _binding.Bindings[1];
        set => SetBinding(1, value);
    }
    /// <summary>
    /// The third variable (accessed by [2] or z)
    /// </summary>
    public BindingBase z
    {
        get => _binding.Bindings[2];
        set => SetBinding(2, value);
    }

    /// <summary>
    /// The fourth variable (accessed by [3])
    /// </summary>
    public BindingBase Var3
    {
        get => _binding.Bindings[3];
        set => SetBinding(3, value);
    }
    /// <summary>
    /// The fifth variable (accessed by [4])
    /// </summary>
    public BindingBase Var4
    {
        get => _binding.Bindings[4];
        set => SetBinding(4, value);
    }
    /// <summary>
    /// The sixth variable (accessed by [5])
    /// </summary>
    public BindingBase Var5
    {
        get => _binding.Bindings[5];
        set => SetBinding(5, value);
    }
    /// <summary>
    /// The seventh variable (accessed by [6])
    /// </summary>
    public BindingBase Var6
    {
        get => _binding.Bindings[6];
        set => SetBinding(6, value);
    }
    /// <summary>
    /// The eighth variable (accessed by [7])
    /// </summary>
    public BindingBase Var7
    {
        get => _binding.Bindings[7];
        set => SetBinding(7, value);
    }
    /// <summary>
    /// The ninth variable (accessed by [8])
    /// </summary>
    public BindingBase Var8
    {
        get => _binding.Bindings[8];
        set => SetBinding(8, value);
    }
    /// <summary>
    /// The tenth variable (accessed by [9])
    /// </summary>
    public BindingBase Var9
    {
        get => _binding.Bindings[9];
        set => SetBinding(9, value);
    }
}
