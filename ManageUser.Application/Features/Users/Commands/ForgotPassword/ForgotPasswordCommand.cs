using ManageUser.Application.CQRSAbstractions;

namespace ManageUser.Application.Features.Users.Commands.ForgotPassword
{
    public class ForgotPasswordCommand : ICommandWResult<ForgotPasswordCommandResponse>
    {
        public string Email { get; set; }
    }
}
