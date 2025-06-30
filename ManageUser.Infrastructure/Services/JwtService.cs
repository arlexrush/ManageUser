using ManageUser.Application.Exceptions;
using ManageUser.Application.JWTService;
using ManageUser.Application.JWTService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ManageUser.Application.Services;

public class JwtService : IJwtService
{
    private readonly ConfigToken _configToken; // Use the strongly-typed config
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<JwtService> _logger;


    // Inject IOptions<ConfigToken> and IHttpContextAccessor
    public JwtService(IOptions<ConfigToken> configTokenOptions, IHttpContextAccessor httpContextAccessor, ILogger<JwtService> logger)
    {
        _httpContextAccessor = httpContextAccessor;

        // Validate the injected IHttpContextAccessor
        if (httpContextAccessor == null)
        {
            throw new ArgumentNullException(nameof(httpContextAccessor), "IHttpContextAccessor cannot be null.");
        }

        // Validate the injected IOptions<ConfigToken>
        if (configTokenOptions == null || configTokenOptions.Value == null)
        {
            throw new ArgumentNullException(nameof(configTokenOptions), "ConfigToken options cannot be null.");
        }

        // Assign the config token options to the private field

        _configToken = configTokenOptions.Value; // Get the actual config object
        if (string.IsNullOrEmpty(_configToken.Key) || _configToken.Key.Length < 32)
        {
            throw new JwtServiceException("JWT Key must be at least 32 characters long for security reasons.");
        }

        _logger = logger ?? throw new ArgumentNullException(nameof(logger)); 
    }

    public async Task<string> GenerateTokenAsync(string userId, string email, IEnumerable<string> roles)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new JwtServiceException("User ID cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(email))
            throw new JwtServiceException("email cannot be null or empty.");

        if (roles == null || !roles.Any())
            throw new ArgumentException("Roles cannot be null or empty.", nameof(roles));


        var claims = GetClaims(userId, email, roles);

        if (string.IsNullOrEmpty(_configToken.Key) || _configToken.Key.Length < 32)
        {
            throw new JwtServiceException("JWT Key must be at least 32 characters long for security reasons.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configToken.Key!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        // Use values from _configToken
        var expires = DateTime.UtcNow.AddMinutes(_configToken.DurationInMinutes);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            Issuer = _configToken.Issuer,
            Audience = _configToken.Audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        _logger.LogInformation("Generating JWT token for user {UserId}", userId);

        return tokenHandler.WriteToken(token);
    }
    public ClaimsPrincipal? GetCurrentPrincipal()
    {
        return _httpContextAccessor.HttpContext?.User;
    }
    private IEnumerable<Claim> GetClaims(string userId, string email, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var newClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));
        claims.AddRange(newClaims);

        return claims;
    }
    public bool HasClaim(string claimType, string? claimValue = null)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null) return false;

        if (claimValue == null)
            return user.HasClaim(c => c.Type == claimType);

        return user.HasClaim(c => c.Type == claimType && c.Value == claimValue);
    }
    public IEnumerable<Claim> GetAllClaims()
    {
        return _httpContextAccessor.HttpContext?.User?.Claims ?? Enumerable.Empty<Claim>();
    }
    public string? GetUserId()
    {
        return GetClaimValue(ClaimTypes.NameIdentifier);
    }
    public string? GetUserEmail()
    {
        return GetClaimValue(ClaimTypes.Email);
    }
    public List<string> GetUserRoles()
    {
        var roles = _httpContextAccessor.HttpContext?.User?.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();
        return roles ?? new List<string>();
    }
    public string GetSessionUser()
    {
        // UserName from  token
        var userName = _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        return userName!;
    }
    public string? GetClaimValue(string claimType)
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(claimType)?.Value;
    }

    

}
