using ManageUser.Application.CQRSAbstractions;

namespace ManageUser.Application.Features.Role.Commands.ResendConfirmationEmail
{
    public class ResendConfirmationEmailCommand : ICommandWResult<ResendConfirmationEmailCommandResponse>
    {
        public string Email { get; set; }
    }
}
