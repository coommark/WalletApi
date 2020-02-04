using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Core.Dto.Requests;

namespace Wallet.Core.Validations
{
    public class MessageCreateRequestValidator : AbstractValidator<MessageCreateRequest>
    {
        List<string> conditions = new List<string> { "Feedback", "Complaint" };
        public MessageCreateRequestValidator()
        {
            RuleFor(m => m.Type).NotEmpty().WithMessage("Message Type cannot be empty")
                .Must(x => conditions.Contains(x)).WithMessage("Message type must be one of: " + String.Join(", or ", conditions));
            
            RuleFor(m => m.Body).NotEmpty().WithMessage("Body cannot be empty");
            RuleFor(m => m.MessageThreadId).GreaterThan(0).WithMessage("Message Thread Id must be an integer greater than 1.");
        }
    }
}
