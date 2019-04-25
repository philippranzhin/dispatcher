namespace DispatcherDesktop.Helpers.ValidationRules
{
    using System.Globalization;
    using System.Windows.Controls;

    class NotEmptyRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrWhiteSpace((value ?? "").ToString())
                       ? new ValidationResult(false, "Field is required.")
                       : ValidationResult.ValidResult;
        }
    }
}
