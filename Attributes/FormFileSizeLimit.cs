using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Illusive.Attributes {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FormFileSizeLimitAttribute : ValidationAttribute {
        private readonly long _maxSize;

        public FormFileSizeLimitAttribute(long maxSize) {
            this._maxSize = maxSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            if ( value is IFormFile file ) {
                var fileName = file.FileName;
                if ( file.Length > this._maxSize ) {
                    return new ValidationResult(
                        $"The file provided exceeds the maximum size of {this._maxSize / 1000 / 1000}MB!");
                }
            }

            return ValidationResult.Success;
        }
    }
}