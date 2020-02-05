using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Core.Dto.Requests;

namespace Wallet.Core.Validations
{
    public class CustomerAccountUpdateValidator : AbstractValidator<CustomerAccountUpdateRequest>
    {
        public CustomerAccountUpdateValidator()
        {
            RuleFor(m => m.Id).GreaterThan(0).WithMessage("Id must be an integer greater than 0.");
            RuleFor(m => m.DailyTransactionLimit).GreaterThanOrEqualTo(0).WithMessage("Daily Transaction Limit must be 0 or above.");
        }
    }
}
