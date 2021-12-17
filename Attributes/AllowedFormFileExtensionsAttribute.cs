using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Illusive.Attributes {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class AllowedFormFileExtensionsAttribute : ValidationAttribute {
        private List<string> AllowedExtensions { get; }

        public AllowedFormFileExtensionsAttribute(string fileExtensions) {
            this.AllowedExtensions = fileExtensions.Replace(" ", string.Empty).Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            if ( value is IFormFile file ) {
                var fileName = file.FileName;
                if ( this.AllowedExtensions.Any(y => fileName.EndsWith(y)) ) {
                    return ValidationResult.Success;
                }

                return new ValidationResult($"The file-type specified is not an image! ({this.AllowedExtensions.Aggregate((a, b) => $"{a}, {b}")}");
            }

            return ValidationResult.Success;
        }
    }
}