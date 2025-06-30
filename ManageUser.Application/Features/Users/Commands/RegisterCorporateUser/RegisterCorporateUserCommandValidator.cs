using FluentValidation;

namespace ManageUser.Application.Features.Users.Commands.RegisterCorporateUser
{
    public class RegisterCorporateUserCommandValidator : AbstractValidator<RegisterCorporateUserCommand>
    {
        public RegisterCorporateUserCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Email).MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");
            RuleFor(x => x.Email).Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
                .WithMessage("Email must be in a valid format.");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.UserName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.ImageUser).NotNull().WithMessage("ImageUser cannot be null. Please provide a valid image file.");
            RuleFor(x => x.ImageUserId).NotEmpty().WithMessage("ImageUserId cannot be empty. Please provide a valid ImageUserId.");
            RuleFor(x => x.ImageUserUrl).NotEmpty().WithMessage("ImageUserUrl cannot be empty. Please provide a valid ImageUserUrl.");
            RuleFor(x => x.Phone).Matches(@"^\+?[0-9\s\-\(\)\.]{7,20}$").WithMessage("Phone number must be in a valid format.");
            RuleFor(x => x.ImageUserUrl).Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
                .WithMessage("ImageUserUrl must be a valid absolute URL.");
            RuleFor(x => x.NameTenant).NotEmpty().WithMessage("NameTenant cannot be empty. Please provide a valid NameTenant.");
            RuleFor(x => x.AddressTenant).NotEmpty().WithMessage("AddressTenant cannot be empty. Please provide a valid AddressTenant.");
            RuleFor(x => x.CIFTenant).NotEmpty().WithMessage("CIFTenant cannot be empty. Please provide a valid CIFTenant.");
            RuleFor(x => x.CIFTenant).Matches(@"^[A-Z0-9]{9}$").WithMessage("CIFTenant must be a valid CIF format (9 alphanumeric characters).");
            RuleFor(x => x.CIFTenant).MaximumLength(9).WithMessage("CIFTenant cannot exceed 9 characters.");

        }
    }
}
