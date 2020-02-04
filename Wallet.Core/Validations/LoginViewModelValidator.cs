using FluentValidation;
using Wallet.Core.Dto.Requests;

namespace Wallet.Core.Validationss
{
    public class LoginViewModelValidator : AbstractValidator<LoginRequest>
    {
        public LoginViewModelValidator()
        {
            RuleFor(vm => vm.Email).NotEmpty().WithMessage("Email cannot be empty");
            RuleFor(vm => vm.Password).NotEmpty().WithMessage("Password cannot be empty");
        }
    }
}