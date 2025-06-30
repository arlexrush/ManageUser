using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.Features.Tenant.Commands.RegisterTenant
{
    public class RegisterTenantCommandResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } // Optional: You can include a message for success or failure
        public string TenantId { get; set; } // Optional: Include the TenantId if needed
    }
}
