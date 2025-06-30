using FluentValidation;
using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.CQRSAbstractions.Behaviors;
using ManageUser.Application.CQRSAbstractions.DomainEventPublisher;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ManageUser.Application
{
    public static class CqrsServiceRegistration
    {
        public static IServiceCollection AddCqrs(this IServiceCollection services, Assembly[] assemblies)
        {
            // Registra todos los ICommandHandler y IQueryHandler encontrados en los ensamblados
            var handlerTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces(), (t, i) => new { Type = t, Interface = i })
                .Where(x =>
                    (x.Interface.IsGenericType && x.Interface.GetGenericTypeDefinition() == typeof(ICommandHandler<>)) ||
                    (x.Interface.IsGenericType && x.Interface.GetGenericTypeDefinition() == typeof(ICommandWResultHandler<,>)) ||
                    (x.Interface.IsGenericType && x.Interface.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)) ||
                    (x.Interface.IsGenericType && x.Interface.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)) || // <-- Evenet Sourcing
                    (x.Interface.IsGenericType && x.Interface.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>))
                    )

                .ToList();

            foreach (var handler in handlerTypes)
            {
                services.AddTransient(handler.Interface, handler.Type);
            }

            // Registro automático de todos los IValidator<T> usando reflexión
            var validatorTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces(), (t, i) => new { Type = t, Interface = i })
                .Where(x => x.Interface.IsGenericType && x.Interface.GetGenericTypeDefinition() == typeof(IValidator<>))
                .ToList();

            foreach (var validator in validatorTypes)
            {
                services.AddScoped(validator.Interface, validator.Type);
            }

            


            return services;
        }
    }
}



