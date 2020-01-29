using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Wallet.Core.Validations;

namespace Wallet.Core.Dto.ViewModels
{
    public class RegistrationViewModel : IValidatableObject
    {               
        public string Email { get; set; }       
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new RegistrationViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}