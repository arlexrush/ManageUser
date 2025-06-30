using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.Features.Role.Commands.CreateRole;
using ManageUser.Application.Features.Role.Commands.DeleteRole;
using ManageUser.Application.Features.Role.Commands.UpdateRole;
using ManageUser.Application.Features.Role.Queries.GetPagedRoles;
using ManageUser.Application.Features.Role.Queries.GetRolesByUserId;
using ManageUser.Application.Features.Users.Commands.AddClaimsToRole;
using ManageUser.Application.Features.Users.Commands.RemoveRoleFromUser;
using ManageUser.Application.JWTService;
using ManageUser.Application.Specification;
using ManageUser.Application.Specification.RoleSpecification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ManageUser.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IJwtService _jwtService;

        public RoleController(IMediator mediator, IJwtService jwtService)
        {
            _mediator = mediator;
            _jwtService = jwtService;
        }

        [Authorize(Roles = "Manager, Admin")]
        [HttpPost("{roleId}/claims")]
        [ProducesResponseType(typeof(AddClaimToRoleCommandResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<AddClaimToRoleCommandResponse>> AddClaimToRole(string roleId, [FromBody] AddClaimToRoleCommand command, CancellationToken cancellationToken)
        {
            command.RoleId = roleId;
            var result = await _mediator.SendCommandWRAsync<AddClaimToRoleCommand, AddClaimToRoleCommandResponse>(command, cancellationToken);
            return Ok(result);
        }

        [Authorize(Roles = "Manager, Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(CreateRoleCommandResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CreateRoleCommandResponse>> CreateRole([FromBody] CreateRoleCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.SendCommandWRAsync<CreateRoleCommand, CreateRoleCommandResponse>(command, cancellationToken);
            return Ok(result);
        }

        
        [Authorize(Roles = "Manager, Admin")]
        [HttpDelete("{userId}/roles/{roleName}")]
        [ProducesResponseType(typeof(RemoveRoleFromUserCommandResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<RemoveRoleFromUserCommandResponse>> RemoveRoleFromUser(string userId, string roleName, CancellationToken cancellationToken)
        {
            var command = new RemoveRoleFromUserCommand { RoleName = roleName };
            var result = await _mediator.SendCommandWRAsync<RemoveRoleFromUserCommand, RemoveRoleFromUserCommandResponse>(command, cancellationToken);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result);
        }


        [Authorize(Roles = "Manager, Admin")]
        [HttpPut("{roleId}")]
        [ProducesResponseType(typeof(UpdateRoleCommandResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<UpdateRoleCommandResponse>> UpdateRole(string roleId, [FromBody] UpdateRoleCommand command, CancellationToken cancellationToken)
        {
            command.Id = roleId;
            var result = await _mediator.SendCommandWRAsync<UpdateRoleCommand, UpdateRoleCommandResponse>(command, cancellationToken);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result);
        }

        [Authorize(Roles = "Manager, Admin")]
        [HttpDelete("{roleId}")]
        [ProducesResponseType(typeof(DeleteRoleCommandResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<DeleteRoleCommandResponse>> DeleteRole(string roleId, CancellationToken cancellationToken)
        {
            var command = new DeleteRoleCommand { Id = roleId };
            var result = await _mediator.SendCommandWRAsync<DeleteRoleCommand, DeleteRoleCommandResponse>(command, cancellationToken);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result);
        }

        [Authorize(Roles = "Manager, Admin")]
        [HttpGet("/rolesByUserId")]
        [ProducesResponseType(typeof(GetRolesByUserIdQueryResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<GetRolesByUserIdQueryResponse>> GetRolesByUserId(GetRolesByUserIdQuery getRolesByUserIdQuery ,CancellationToken cancellationToken)
        {
            var result = await _mediator.SendQueryAsync<GetRolesByUserIdQuery, GetRolesByUserIdQueryResponse>(getRolesByUserIdQuery, cancellationToken);
            return Ok(result);
        }


        [Authorize(Roles = "Manager, Admin")]
        [HttpGet]
        [ProducesResponseType(typeof(PaginationVm<GetPagedRolesQueryResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginationVm<GetPagedRolesQueryResponse>>> GetPagedRoles(CancellationToken cancellationToken)
        {
            var query = new GetPagedRolesQuery();
            var result = await _mediator.SendQueryAsync<GetPagedRolesQuery, PaginationVm<GetPagedRolesQueryResponse>>(query, cancellationToken);
            return Ok(result);
        }

    }
}
