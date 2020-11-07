using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Illusive.Attributes {
    public class UnicodeOnlyAttribute : ValidationAttribute {

        public UnicodeOnlyAttribute() { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            if ( value is string str ) {
                if ( str.Any(character => character >= 255) ) {
                    return new ValidationResult($"The string provided does not conform to unicode standards.");
                }
            }

            return ValidationResult.Success;
        }

    }
}