using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using CoreApiBase.Resources;

namespace CoreApiBase.Configurations
{
    /// <summary>
    /// Base class for validation attributes that support localization.
    /// </summary>
    public abstract class LocalizedValidationAttribute : ValidationAttribute
    {
        private readonly string _localizationKey;

        protected LocalizedValidationAttribute(string localizationKey)
        {
            _localizationKey = localizationKey;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var isValid = IsValidCore(value, validationContext);
            
            if (!isValid)
            {
                var localizer = validationContext.GetService<IStringLocalizer<SharedResource>>();
                var errorMessage = localizer?[_localizationKey] ?? _localizationKey;
                return new ValidationResult(errorMessage);
            }

            return ValidationResult.Success;
        }

        protected abstract bool IsValidCore(object? value, ValidationContext validationContext);
    }

    /// <summary>
    /// Localized Required validation attribute.
    /// </summary>
    public class LocalizedRequiredAttribute : LocalizedValidationAttribute
    {
        public LocalizedRequiredAttribute(string localizationKey) : base(localizationKey) { }

        protected override bool IsValidCore(object? value, ValidationContext validationContext)
        {
            return value != null && !string.IsNullOrWhiteSpace(value.ToString());
        }
    }

    /// <summary>
    /// Localized MinLength validation attribute.
    /// </summary>
    public class LocalizedMinLengthAttribute : LocalizedValidationAttribute
    {
        private readonly int _minLength;

        public LocalizedMinLengthAttribute(int minLength, string localizationKey) : base(localizationKey)
        {
            _minLength = minLength;
        }

        protected override bool IsValidCore(object? value, ValidationContext validationContext)
        {
            if (value == null) return true; // Let Required handle null values
            
            if (value is string str)
                return str.Length >= _minLength;
            
            if (value is Array array)
                return array.Length >= _minLength;

            return true;
        }
    }

    /// <summary>
    /// Localized Range validation attribute.
    /// </summary>
    public class LocalizedRangeAttribute : LocalizedValidationAttribute
    {
        private readonly int _minimum;
        private readonly int _maximum;

        public LocalizedRangeAttribute(int minimum, int maximum, string localizationKey) : base(localizationKey)
        {
            _minimum = minimum;
            _maximum = maximum;
        }

        protected override bool IsValidCore(object? value, ValidationContext validationContext)
        {
            if (value == null) return true;
            
            if (int.TryParse(value.ToString(), out int intValue))
            {
                return intValue >= _minimum && intValue <= _maximum;
            }

            return false;
        }
    }

    /// <summary>
    /// Localized URL validation attribute.
    /// </summary>
    public class LocalizedUrlAttribute : LocalizedValidationAttribute
    {
        public LocalizedUrlAttribute(string localizationKey) : base(localizationKey) { }

        protected override bool IsValidCore(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return true; // Let Required handle empty values

            return Uri.TryCreate(value.ToString(), UriKind.Absolute, out _);
        }
    }
}
