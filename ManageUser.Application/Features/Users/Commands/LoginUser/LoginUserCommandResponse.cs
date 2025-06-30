namespace ManageUser.Application.Features.Users.Commands.LoginUser
{
    public class LoginUserCommandResponse
    {
        public bool IsSuccess { get; set; }
        public string? Token { get; set; }
        public DateTime? Expiration { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
