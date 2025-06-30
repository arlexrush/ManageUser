using ManageUser.Application.CQRSAbstractions;

namespace ManageUser.Application.Features.Users.Commands.LoginUser
{
    public class LoginUserCommand : ICommandWResult<LoginUserCommandResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
