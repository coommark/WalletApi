using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Wallet.Core.Validations;

namespace Wallet.Core.Dto.Requests
{
    public class CustomerTransactionCreateRequest : IValidatableObject
    {
        public int CustomerId { get; set; }
        public string TransactionType { get; set; }
        public decimal Credit { get; set; } = 0.0m;
        public decimal Debit { get; set; } = 0.0m;
        public string Description { get; set; }
        public string Flow { get; set; }
        public int CustomerTransactionBatchId { get; set; }

        public string SourceAccount { get; set; }
        public string DestinationAccount { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new CustomerTransactionCreateValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}
