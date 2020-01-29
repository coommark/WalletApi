using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Core.Dto.Requests;

namespace Wallet.Core.Validations
{
    

    public class AccountTypeCreateValidator : AbstractValidator<AccountTypeCreateRequest>
    {
        List<string> conditions = new List<string> { "Current Account", "Savings Account" };
        public AccountTypeCreateValidator()
        {
            RuleFor(m => m.Type).NotEmpty().WithMessage("Account Type cannot be empty")
                .Must(x => conditions.Contains(x)).WithMessage("Account type must be one of: " + String.Join(", or ", conditions));
            RuleFor(m => m.CategoryCode).NotEmpty().WithMessage("Category Code cannot be empty")
                .Length(3).WithMessage("Category code must be three numbers exact.");                
            RuleFor(m => m.Description).NotEmpty().WithMessage("Description cannot be empty");
            RuleFor(m => m.MinimumBalance).GreaterThanOrEqualTo(0).WithMessage("Minimum balance must be 0 or above.");
            When(x => x.AllowOverdraw, () =>
            {
                RuleFor(m => m.MinimumBalance).Equal(0).WithMessage("Minimum balance cannot be set when allow overdraw is true.");
            });
        }
    }
}
