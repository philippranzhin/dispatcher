namespace DispatcherDesktop.Infrastructure.ValidationRules
{
    using System.Globalization;
    using System.Windows.Controls;

    public class MoreThanRule : ValidationRule
    {
        public int Value { get; set; } = 0;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            switch (value)
            {
                case int number:
                    return this.Validate(number);

                case string row:
                    if (int.TryParse(row, out int result))
                    {
                        return this.Validate(result);
                    };
                    return new ValidationResult(false, Properties.Resources.ShouldBeNumberErrorMsg);

                default: return new ValidationResult(false, Properties.Resources.ShouldBeNumberErrorMsg);
            }
        }

        private ValidationResult Validate(int number)
        {
            return number > this.Value
                ? ValidationResult.ValidResult
                : new ValidationResult(false, $"{Properties.Resources.ShouldBeMoreErrorMsg} {this.Value}");
        }
    }
}
