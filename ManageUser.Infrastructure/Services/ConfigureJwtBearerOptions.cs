using ManageUser.Application.JWTService.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Infrastructure.Services
{
    public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
    {

        private readonly ConfigToken? _configToken;

        public ConfigureJwtBearerOptions(IOptions<ConfigToken> configToken)
        {
            _configToken = configToken.Value;
        }

              

        public void Configure(string? name, JwtBearerOptions options)
        {
            if (name != JwtBearerDefaults.AuthenticationScheme) return;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configToken!.Issuer,
                ValidAudience = _configToken.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configToken.Key!)),
                ClockSkew = TimeSpan.Zero
            };
        }

        // Invocado solo si el esquema no está nombrado
        public void Configure(JwtBearerOptions options) =>
            Configure(Options.DefaultName, options);

    }
}

