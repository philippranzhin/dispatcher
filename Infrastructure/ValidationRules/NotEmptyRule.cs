namespace DispatcherDesktop.Infrastructure.ValidationRules
{
    using System.Globalization;
    using System.Windows.Controls;

    class NotEmptyRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrWhiteSpace((value ?? "").ToString())
                       ? new ValidationResult(false, Properties.Resources.FieldIsRequredErrorMsg)
                       : ValidationResult.ValidResult;
        }
    }
}
