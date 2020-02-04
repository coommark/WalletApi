using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Wallet.Core.Validations;

namespace Wallet.Core.Dto.Requests
{
    public class CustomerAccountStatusCreateRequest : IValidatableObject
    {
        public int CustomerAccountId { get; set; }
        public string Status { get; set; }
        public bool IsCurrentStatus { get; set; }
        public string Comment { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new CustomerAccountStatusCreateValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}
