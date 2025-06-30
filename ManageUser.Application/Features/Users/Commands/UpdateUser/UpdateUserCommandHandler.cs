using ManageUser.Application.AuthService.Models;
using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.CQRSAbstractions.DomainEventPublisher;
using ManageUser.Application.JWTService;
using ManageUser.Application.Repositories;
using ManageUser.Domain.Address;
using ManageUser.Domain.Events;
using Microsoft.AspNetCore.Identity;

namespace ManageUser.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : ICommandWResultHandler<UpdateUserCommand, UpdateUserCommandResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDomainEventPublisher _eventPublisher;
        private readonly IJwtService _jwtService;

        public UpdateUserCommandHandler(
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            IDomainEventPublisher eventPublisher,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _eventPublisher = eventPublisher;
            _jwtService = jwtService;
        }

        public async Task<UpdateUserCommandResponse> HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken)
        {

            try
            {
                var userId = _jwtService.GetUserId();
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new UpdateUserCommandResponse { IsSuccess = false, ErrorMessage = "Usuario no encontrado." };
                }

                var countriesBD = await _unitOfWork.Repository<Country>().GetAsync(x => x.cca2 == command.Cca2Address);
                var country = countriesBD.FirstOrDefault();
                user.FirstName = command.Name ?? user.FirstName;
                user.LastName = command.LastName ?? user.LastName;
                user.Email = command.Email ?? user.Email;
                user.PhoneNumber = command.Phone ?? user.PhoneNumber;
                user.Address = new Address() { Street = command.Street, Number = command.Number, City = command.City, Country = country, PostalCode = command.PostalCode, State = command.State, Cca2Address = command.Cca2Address };
                user.CIF = command.CIF ?? user.CIF;

                await _userManager.UpdateAsync(user);


                // Publicar evento de dominio
                var userUpdatedEvent = new UserUpdatedEvent(user.Id, user.Email);
                user.AddDomainEvent(userUpdatedEvent);
                await _eventPublisher.PublishAsync(userUpdatedEvent, cancellationToken);
                await _eventPublisher.SaveDomainEventsAsync(userUpdatedEvent, cancellationToken);
                await _eventPublisher.PublishEventToMessageBrokerAsync(userUpdatedEvent, cancellationToken);

                return new UpdateUserCommandResponse { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new UpdateUserCommandResponse { IsSuccess = false, ErrorMessage = ex.Message };
            }
            
        }
    }
}
