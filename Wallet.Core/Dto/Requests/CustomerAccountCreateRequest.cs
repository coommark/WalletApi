using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Wallet.Core.Validations;

namespace Wallet.Core.Dto.Requests
{
    public class CustomerAccountCreateRequest : IValidatableObject
    {
        public int ApplicationUserId { get; set; }
        public int AccountTypeId { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new CustomerAccountCreateValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
    
}
