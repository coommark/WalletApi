﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Wallet.Core.Validationss;

namespace Wallet.Core.Dto.ViewModels
{
    public class LoginViewModel : IValidatableObject
    {       
        public string Email { get; set; }      
        public string Password { get; set; }
        public bool RememberMe { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new LoginViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}