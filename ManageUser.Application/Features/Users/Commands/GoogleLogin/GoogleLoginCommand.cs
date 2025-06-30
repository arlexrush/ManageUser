using ManageUser.Application.CQRSAbstractions;

namespace ManageUser.Application.Features.Users.Commands.LoginUserWGmail
{
    public class GoogleLoginCommand : ICommandWResult<GoogleLoginCommandResponse>
    {
        public string IdToken { get; set; } // Token JWT de Google recibido en el frontend
    }
}
