using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Core.Dto.Requests;

namespace Wallet.Core.Validations
{

    public class CustomerAccountCreateValidator : AbstractValidator<CustomerAccountCreateRequest>
    {
        public CustomerAccountCreateValidator()
        {           
            RuleFor(m => m.ApplicationUserId).GreaterThan(0).WithMessage("Application User Id must be an integer greater than 0.");
            RuleFor(m => m.AccountTypeId).GreaterThan(0).WithMessage("Application User Id must be an integer greater than 0.");
        }
    }

}
