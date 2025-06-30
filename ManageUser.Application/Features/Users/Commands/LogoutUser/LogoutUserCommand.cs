using ManageUser.Application.CQRSAbstractions;

namespace ManageUser.Application.Features.Users.Commands.LogoutUser
{
    public class LogoutUserCommand: ICommand
    {
        public string UserId { get; set; }
        public string RefreshToken { get; set; }
    }
}
