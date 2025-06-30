using ManageUser.Application.CQRSAbstractions;

namespace ManageUser.Application.Features.Users.Commands.ResetPassword
{
    public class ResetPasswordCommand : ICommandWResult<ResetPasswordCommandResponse>
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
