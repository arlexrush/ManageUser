using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.Features.Tenant.Commands.RegisterTenant
{
    public class RegisterTenantCommandValidator : AbstractValidator<RegisterTenantCommand>
    {
        public RegisterTenantCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name cannot be empty.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));
            RuleFor(x => x.LogoUrl)
                .NotEmpty().WithMessage("LogoUrl cannot be empty.")
                .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
                .WithMessage("LogoUrl must be a valid absolute URL.");
            RuleFor(x => x.ImageTenantId)
                .NotEmpty().WithMessage("ImageTenantId cannot be empty.");
        }
    }
}
