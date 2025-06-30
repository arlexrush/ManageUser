using ManageUser.Application.CQRSAbstractions;

namespace ManageUser.Application.Features.Users.Commands.RefreshToken
{
    public class RefreshTokenCommand : ICommandWResult<RefreshTokenCommandResponse>
    {
        public string Email { get; set; }
        public string RefreshToken { get; set; }
    }
}
