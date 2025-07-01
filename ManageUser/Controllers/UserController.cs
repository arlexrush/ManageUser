using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.Features.Users.Commands.InviteUser;
using ManageUser.Application.Features.Users.Commands.RegisterCorporateUser;
using ManageUser.Application.Features.Users.Commands.RegisterInvitedUser;
using ManageUser.Application.Features.Users.Commands.RegisterUser;
using ManageUser.Application.ImageServices;
using ManageUser.Application.ImageServices.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ManageUser.Application.AuthService.Models;
using ManageUser.Application.JWTService;
using ManageUser.Application.Features.Users.Commands.LoginUser;
using ManageUser.Application.Features.Users.Commands.RefreshToken;
using ManageUser.Application.Features.Users.Commands.LoginUserWGmail;
using ManageUser.Application.Features.Role.Commands.ResendConfirmationEmail;
using ManageUser.Application.Features.Users.Commands.ForgotPassword;
using ManageUser.Application.Features.Users.Commands.ResetPassword;
using ManageUser.Application.Features.Users.Commands.UpdateUser;
using ManageUser.Application.Features.Users.Queries.GetMyInfo;
using ManageUser.Application.Specification;
using ManageUser.Application.Specification.UserSpecification;
using ManageUser.Application.Features.Users.Queries.GetPagedUsers;
using ManageUser.Application.Features.Users.Queries.AuditUserDomainEvent;
using ManageUser.Application.Features.Users.Commands.LogoutUser;

namespace ManageUser.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController:ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJwtService _jwtService;
        private readonly IMediator _mediator;
        private readonly IManageImageService _manageImageService;
        private readonly ILogger<UserController> _logger;

        public UserController(IMediator mediator, IManageImageService manageImageService, ILogger<UserController> logger, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IJwtService jwtService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _manageImageService = manageImageService ?? throw new ArgumentNullException(nameof(manageImageService));
            _logger = logger;
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }


        // registro de usuario individual
        [AllowAnonymous]
        [HttpPost("register", Name = "Register")]
        [ProducesResponseType(typeof(RegisterUserCommandResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<RegisterUserCommandResponse>> Register([FromForm] RegisterUserCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(command.CIF))
                {
                    command.CIF = "CIF-DEFAULT"; // Asigna un valor por defecto si no se proporciona
                }
                if (string.IsNullOrEmpty(command.TypeUser))
                {
                    command.TypeUser = "Individual"; // Asigna un valor por defecto si no se proporciona
                }


                if (command.ImageUser is not null)
                {
                    ImageResponse? resultImage;
                    try
                    {
                        resultImage = await _manageImageService.CloudinaryUploadImage(new ImageData
                        {
                            ImageStream = command.ImageUser.OpenReadStream(),
                            Name = command.ImageUser.Name,
                        });
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        _logger.LogError(ex, "Error uploading image: {Message}", ex.Message);
                        return BadRequest(new { Message = "Error processing the image." });
                    }
                    
                    if (resultImage?.PublicId is null || resultImage.Url is null)
                        return BadRequest(new { Message = "No se pudo obtener la información de la imagen." });

                    command.ImageUserId = resultImage.PublicId;
                    command.ImageUserUrl = resultImage.Url;
                }

                // Commando con respuesta
                var response = await _mediator.SendCommandWRAsync<RegisterUserCommand, RegisterUserCommandResponse>(command, cancellationToken);

                if (!response.IsSuccess)
                    return BadRequest(response);

                return Ok(response);
            }
            catch(Exception ex)
            {
                // Loguea el error aquí si tienes un logger
                _logger.LogError(ex, "Error al registrar el usuario: {Message}", ex.Message);
                return StatusCode(500, "Ocurrió un error al registrar el usuario.");
            }
            
        }

        // registro de usuario corporativo
        [AllowAnonymous]
        [HttpPost("registerCorporate", Name = "RegisterCorporate")]
        [ProducesResponseType(typeof(RegisterCorporateUserCommandResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<RegisterCorporateUserCommandResponse>> RegisterCorporate([FromForm] RegisterCorporateUserCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (command.ImageUser is not null)
                {
                    ImageResponse? resultImage;
                    try
                    {
                        resultImage = await _manageImageService.CloudinaryUploadImage(new ImageData
                        {
                            ImageStream = command.ImageUser.OpenReadStream(),
                            Name = command.ImageUser.Name,
                        });
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        _logger.LogError(ex, "Error uploading image: {Message}", ex.Message);
                        return BadRequest(new { Message = "Error processing the image." });
                    }

                    if (resultImage?.PublicId is null || resultImage.Url is null)
                        return BadRequest(new { Message = "No se pudo obtener la información de la imagen." });

                    command.ImageUserId = resultImage.PublicId;
                    command.ImageUserUrl = resultImage.Url;
                }

                // Commando con respuesta
                var response = await _mediator.SendCommandWRAsync<RegisterCorporateUserCommand, RegisterCorporateUserCommandResponse>(command, cancellationToken);

                if (!response.IsSuccess)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Loguea el error aquí si tienes un logger
                _logger.LogError(ex, "Error al registrar el usuario: {Message}", ex.Message);
                return StatusCode(500, "Ocurrió un error al registrar el usuario.");
            }

        }

        [Authorize(Roles = "Manager")]
        [HttpPost("invite", Name = "Invite")]
        [ProducesResponseType(typeof(InviteUserCommandResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<InviteUserCommandResponse>> Invite([FromBody] InviteUserCommand command, CancellationToken cancellationToken)
        {
            var response = await _mediator.SendCommandWRAsync<InviteUserCommand, InviteUserCommandResponse>(command, cancellationToken);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }


        [AllowAnonymous]
        [HttpPost("registerinvited", Name ="RegisterInvite")]
        [ProducesResponseType(typeof(RegisterUserCommandResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<RegisterUserCommandResponse>> RegisterInvited([FromBody] RegisterInvitedUserCommand command, CancellationToken cancellationToken)
        {
            try
            {

                if (command.ImageUser is not null)
                {
                    ImageResponse? resultImage;
                    try
                    {
                        resultImage = await _manageImageService.CloudinaryUploadImage(new ImageData
                        {
                            ImageStream = command.ImageUser.OpenReadStream(),
                            Name = command.ImageUser.Name,
                        });
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        _logger.LogError(ex, "Error uploading image: {Message}", ex.Message);
                        return BadRequest(new { Message = "Error processing the image." });
                    }

                    if (resultImage?.PublicId is null || resultImage.Url is null)
                        return BadRequest(new { Message = "No se pudo obtener la información de la imagen." });

                    command.ImageUserId = resultImage.PublicId;
                    command.ImageUserUrl = resultImage.Url;
                }

                var response = await _mediator.SendCommandWRAsync<RegisterInvitedUserCommand, RegisterUserCommandResponse>(command, cancellationToken);
                if (!response.IsSuccess)
                    return BadRequest(response);
                return Ok(response);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar el usuario invitado: {Message}", ex.Message);
                return StatusCode(500, "Ocurrió un error al registrar el usuario invitado.");
            }

            
        }


        // Endpoint para iniciar el login con Google
        [HttpGet("externallogin")]
        public IActionResult ExternalLogin(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("ExternalLoginCallback", new { returnUrl }) };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // Callback de Google
        [HttpGet("externallogincallback")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/")
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
                return BadRequest("Error al autenticar con Google.");

            var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;

            
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Crear usuario sin password, marcando el email como confirmado
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = name ?? "",
                    IsEmailConfirmed = true,
                    TypeUser = "Individual", // O el tipo que corresponda
                    AvatarUrl = "", // Puedes obtener la foto de Google si está disponible
                    IsDeleted = false
                };
                await _userManager.CreateAsync(user);                
                await _userManager.AddToRoleAsync(user, "Player");
            }

            var roles = await _userManager.GetRolesAsync(user);
            // Genera el JWT o realiza el login en tu sistema
            var token = await _jwtService.GenerateTokenAsync(user.Id, user.Email!, roles);

            // Redirige o responde según tu frontend
            return Ok(new { email, name, token });
        }

        [AllowAnonymous]
        [HttpPost("login", Name = "Login")]
        [ProducesResponseType(typeof(LoginUserCommandResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<LoginUserCommandResponse>> Login([FromBody] LoginUserCommand command, CancellationToken cancellationToken)
        {
            var response = await _mediator.SendCommandWRAsync<LoginUserCommand, LoginUserCommandResponse>(command, cancellationToken);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token", Name = "RefreshToken")]
        [ProducesResponseType(typeof(RefreshTokenCommandResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<RefreshTokenCommandResponse>> RefreshToken([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            var response = await _mediator.SendCommandWRAsync<RefreshTokenCommand, RefreshTokenCommandResponse>(command, cancellationToken);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }


        [AllowAnonymous]
        [HttpPost("login-google", Name = "LoginGoogle")]
        [ProducesResponseType(typeof(GoogleLoginCommandResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<GoogleLoginCommandResponse>> LoginGoogle([FromBody] GoogleLoginCommand command, CancellationToken cancellationToken)
        {
            var response = await _mediator.SendCommandWRAsync<GoogleLoginCommand, GoogleLoginCommandResponse>(command, cancellationToken);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("resend-confirmation-email", Name = "ResendConfirmationEmail")]
        [ProducesResponseType(typeof(ResendConfirmationEmailCommandResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ResendConfirmationEmailCommandResponse>> ResendConfirmationEmail([FromBody] ResendConfirmationEmailCommand command, CancellationToken cancellationToken)
        {
            var response = await _mediator.SendCommandWRAsync<ResendConfirmationEmailCommand, ResendConfirmationEmailCommandResponse>(command, cancellationToken);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }



        [AllowAnonymous]
        [HttpPost("forgot-password", Name = "ForgotPassword")]
        [ProducesResponseType(typeof(ForgotPasswordCommandResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ForgotPasswordCommandResponse>> ForgotPassword([FromBody] ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            var response = await _mediator.SendCommandWRAsync<ForgotPasswordCommand, ForgotPasswordCommandResponse>(command, cancellationToken);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("reset-password", Name = "ResetPassword")]
        [ProducesResponseType(typeof(ResetPasswordCommandResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ResetPasswordCommandResponse>> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            var response = await _mediator.SendCommandWRAsync<ResetPasswordCommand, ResetPasswordCommandResponse>(command, cancellationToken);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }


        [Authorize(Roles ="Manager, Admin")]
        [HttpPut("update", Name = "UpdateUser")]
        [ProducesResponseType(typeof(UpdateUserCommandResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<UpdateUserCommandResponse>> UpdateUser([FromBody] UpdateUserCommand command, CancellationToken cancellationToken)
        {

            if (command.ImageUser is not null)
            {
                ImageResponse? resultImage;
                try
                {
                    resultImage = await _manageImageService.CloudinaryUploadImage(new ImageData
                    {
                        ImageStream = command.ImageUser.OpenReadStream(),
                        Name = command.ImageUser.Name,
                    });
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it as needed
                    _logger.LogError(ex, "Error uploading image: {Message}", ex.Message);
                    return BadRequest(new { Message = "Error processing the image." });
                }

                if (resultImage?.PublicId is null || resultImage.Url is null)
                    return BadRequest(new { Message = "No se pudo obtener la información de la imagen." });

                command.ImageUserId = resultImage.PublicId;
                command.ImageUserUrl = resultImage.Url;
            }
            var response = await _mediator.SendCommandWRAsync<UpdateUserCommand, UpdateUserCommandResponse>(command, cancellationToken);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(GetMyInfoQueryResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<GetMyInfoQueryResponse>> GetMyInfo(CancellationToken cancellationToken)
        {
            var query = new GetMyInfoQuery();
            var response = await _mediator.SendQueryAsync<GetMyInfoQuery, GetMyInfoQueryResponse>(query, cancellationToken);
            
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("userPagination")]
        [ProducesResponseType(typeof(PaginationVm<GetPagedUserQueryResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginationVm<GetPagedUserQueryResponse>>> GetUserPagination(CancellationToken cancellationToken)
        {
            var query = new GetPagedUserQuery();
            var response = await _mediator.SendQueryAsync<GetPagedUserQuery, PaginationVm<GetPagedUserQueryResponse>>(query, cancellationToken);
                        
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("auditUserDomainEvents", Name = "AuditUserDomainEvents")]
        [ProducesResponseType(typeof(List<AuditUserDomainEventQueryResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<AuditUserDomainEventQueryResponse>>> AuditUserDomainEvents(CancellationToken cancellationToken)
        {
            var query = new AuditUserDomainEventQuery();
            var response = await _mediator.SendQueryAsync<AuditUserDomainEventQuery, List<AuditUserDomainEventQueryResponse>>(query, cancellationToken);
            if (response is null || !response.Any())
                return NotFound(new { Message = "No se encontraron eventos de dominio para el usuario." });
            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutUserCommand command, CancellationToken cancellationToken)
        {
            await _mediator.SendCommandAsync(command, cancellationToken);
            return Ok(new { message = "Logout exitoso" });
        }

    }

}
