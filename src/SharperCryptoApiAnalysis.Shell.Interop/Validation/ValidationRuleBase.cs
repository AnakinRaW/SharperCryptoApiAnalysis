using System;
using System.Globalization;
using System.Windows.Controls;

namespace SharperCryptoApiAnalysis.Shell.Interop.Validation
{
    /// <inheritdoc />
    /// <summary>
    /// WPF <see cref="T:System.Windows.Controls.ValidationResult" /> code base
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <seealso cref="T:System.Windows.Controls.ValidationRule" />
    public abstract class ValidationRuleBase<TSource> : ValidationRule
    {
        public sealed override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!(value is TSource) && (value != null || typeof(TSource).IsValueType))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Source is not type: {0} but is type {1}",
                    new object[]
                    {
                        typeof(TSource).FullName,
                        value?.GetType().FullName
                    }));
            return Validate((TSource) value, cultureInfo);
        }

        protected abstract ValidationResult Validate(TSource value, CultureInfo cultureInfo);
    }
}