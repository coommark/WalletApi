using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Core.Dto.Requests;

namespace Wallet.Core.Validations
{
    public class CustomerAccountStatusCreateValidator : AbstractValidator<CustomerAccountStatusCreateRequest>
    {
        List<string> conditions = new List<string> { "Active", "Dormant", "Suspended", "Closed" };

        public CustomerAccountStatusCreateValidator()
        {
            RuleFor(m => m.CustomerAccountId).GreaterThan(0).WithMessage("Application User Id must be an integer greater than 0.");
            RuleFor(m => m.Status).NotEmpty().WithMessage("Account Type cannot be empty")
               .Must(x => conditions.Contains(x)).WithMessage("Account type must be one of: " + String.Join(", or ", conditions));
            RuleFor(m => m.Comment).NotEmpty().WithMessage("Comment cannot be empty");
        }
    }
}
