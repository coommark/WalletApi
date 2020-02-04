using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Core.Dto.Requests;

namespace Wallet.Core.Validations
{
    public class CustomerTransactionCreateValidator : AbstractValidator<CustomerTransactionCreateRequest>
    {
        List<string> conditions = new List<string> { "Transfer", "Admin Transfer" };
        List<string> flowConditions = new List<string> { "Source", "Destination" };
        public CustomerTransactionCreateValidator()
        {
            RuleFor(m => m.CustomerId).GreaterThan(0).WithMessage("Id must be an integer greater than 0.");
            RuleFor(m => m.TransactionType).NotEmpty().WithMessage("Transaction Type cannot be empty")
                .Must(x => conditions.Contains(x)).WithMessage("Transaction type must be one of: " + String.Join(", or ", conditions));
            RuleFor(m => m.Description).NotEmpty().WithMessage("Description cannot be empty");
            RuleFor(m => m.Flow).NotEmpty().WithMessage("Flow cannot be empty")
                .Must(x => flowConditions.Contains(x)).WithMessage("Flow type must be one of: " + String.Join(", or ", flowConditions)); ;
            RuleFor(m => m.CustomerTransactionBatchId).GreaterThanOrEqualTo(0).WithMessage("Batch Id must be 0 or above.");
            RuleFor(m => m.Credit).GreaterThanOrEqualTo(0).WithMessage("Credit must be 0 or above.");
            RuleFor(m => m.Debit).GreaterThanOrEqualTo(0).WithMessage("Debit must be 0 or above.");
            When(x => x.Credit > 0, () =>
            {
                RuleFor(m => m.Debit).Equal(0).WithMessage("Debit cannot be above 0 when Credit amount is greater than 0.");
            });

            When(x => x.Debit > 0, () =>
            {
                RuleFor(m => m.Credit).Equal(0).WithMessage("Credit cannot be above 0 when Debit amount is greater than 0.");
            });
        }
    }
}
