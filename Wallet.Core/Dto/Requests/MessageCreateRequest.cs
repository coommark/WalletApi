using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Wallet.Core.Validations;

namespace Wallet.Core.Dto.Requests
{
    public class MessageCreateRequest : IValidatableObject
    {
        public string Type { get; set; }
        public string Body { get; set; }
        public int MessageThreadId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new MessageCreateRequestValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}
