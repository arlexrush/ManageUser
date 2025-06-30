using ManageUser.Application.AuthService.Models;
using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.Repositories;
using ManageUser.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.Features.Tenant.Commands.RegisterTenant
{
    public class RegisterTenantCommandHandler : ICommandWResultHandler<RegisterTenantCommand, RegisterTenantCommandResponse>
    {

        private readonly IAsyncRepository<Tenant<ApplicationUser>> _tenantRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterTenantCommandHandler(IAsyncRepository<Tenant<ApplicationUser>> tenantRepository, IUnitOfWork unitOfWork)
        {
            _tenantRepository = tenantRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<RegisterTenantCommandResponse> HandleAsync(RegisterTenantCommand request, CancellationToken cancellationToken)
        {
            var tenant = new Tenant<ApplicationUser>
            {
                Name = request.Name,
                Description = request.Description,
                LogoUrl = request.LogoUrl,
                ImageTenantId = request.ImageTenantId
            };
            // Validate the tenant data (you can add more validation logic here if needed)
            try
            {
                await _tenantRepository.AddAsync(tenant);
                await _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("An error occurred while registering the tenant.", ex);
            }

            return new RegisterTenantCommandResponse
            {
                IsSuccess = true,
                Message = "Tenant registered successfully.",
                TenantId = tenant.Id // Assuming Id is the identifier for the tenant
            };
        }
        
    }
}
