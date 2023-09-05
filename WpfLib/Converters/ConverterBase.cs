using System;
using System.Windows.Markup;

namespace WpfLib.Converters;

public class ConverterBase<TConverter> : MarkupExtension
    where TConverter : ConverterBase<TConverter>, new()
{
    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
