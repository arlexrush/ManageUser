using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.Features.Tenant.Commands.RegisterTenant;
using ManageUser.Application.ImageServices;
using ManageUser.Application.ImageServices.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ManageUser.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TenantController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IManageImageService _manageImageService;
        private readonly ILogger<UserController> _logger;

        public TenantController(IMediator mediator, IManageImageService manageImageService, ILogger<UserController> logger)
        {
            _mediator = mediator;
            _manageImageService = manageImageService ?? throw new ArgumentNullException(nameof(manageImageService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Registrar un Tenant Privado
        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterTenantCommandResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<RegisterTenantCommandResponse>> Register([FromBody] RegisterTenantCommand command, CancellationToken cancellationToken)
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

            var response = await _mediator.SendCommandWRAsync<RegisterTenantCommand, RegisterTenantCommandResponse>(command, cancellationToken);
            if (!response.IsSuccess) return BadRequest(response);

            return Ok(response);
        }








    }
}
