using System;
using System.Globalization;
using System.Windows.Data;
namespace Circus.Wpf.Converters {
    public abstract class MultiValue<T> : Markup, IMultiValueConverter {
        public virtual object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            return default(T);
        }
        public virtual object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            return null;
        }
        public override object ProvideValue(IServiceProvider provider) {
            return this;
        }
    }
}
