using ManageUser.Application.CountryService;
using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.CQRSAbstractions.DomainEventPublisher;
using ManageUser.Application.CQRSAbstractions.DomainEventPublisher.Event;
using ManageUser.Application.Features.Users.Commands.UpdateUser;
using ManageUser.Domain.Events;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ManageUser.Application
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();

            services.AddScoped<ICommandWResultHandler<UpdateUserCommand, UpdateUserCommandResponse>, UpdateUserCommandHandler>();
            services.AddScoped<IDomainEventHandler<UserUpdatedEvent>, UserUpdatedEventHandler>();

            services.AddCqrs(new[] { typeof(ApplicationServiceExtensions).Assembly });

            services.AddHttpClient();
            

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            }).AddCookie().AddGoogle(options =>
            {
                options.ClientId = configuration["Authentication:Google:ClientId"] ?? throw new ArgumentNullException(nameof(configuration), "ClientId cannot be null.");
                options.ClientSecret = configuration["Authentication:Google:ClientSecret"] ?? throw new ArgumentNullException(nameof(configuration), "ClientSecret cannot be null.");
                options.CallbackPath = "/signin-google";
            });

            return services;
        }
    }  
}
