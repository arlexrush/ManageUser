using FluentValidation;

namespace ManageUser.Application.Features.Users.Commands.RegisterUser
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
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
            
        }
    }
}
