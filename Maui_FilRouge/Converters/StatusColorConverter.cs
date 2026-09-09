using System.Globalization;

namespace Maui_FilRouge.Converters
{
    /// <summary>Convertit l'état de validation d'une commande en couleur de badge.</summary>
    public class StatusColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var isValidated = value is bool b && b;
            return isValidated
                ? Application.Current!.Resources["Success"]
                : Application.Current!.Resources["Secondary"];
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
