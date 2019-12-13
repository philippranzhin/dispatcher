namespace DispatcherDesktop.Infrastructure.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using Models;

    public class EditRegisterModeToDialogHeaderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EditRegisterMode mode)
            {
                return mode == EditRegisterMode.Create
                    ? Properties.Resources.AddRegisterDialogHeader
                    : Properties.Resources.EditRegisterDialogHeader;
            }

            else
            {
                throw new ArgumentException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
