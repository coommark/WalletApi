using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Wallet.Core.Validations;

namespace Wallet.Core.Dto.Requests
{
    public class AccountTypeCreateRequest : IValidatableObject
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public string CategoryCode { get; set; }
        public decimal MinimumBalance { get; set; }
        public bool AllowOverdraw { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new AccountTypeCreateValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}
