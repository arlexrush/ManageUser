using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ManageUser.Application.Exceptions;
using ManageUser.Application.ImageServices;
using ManageUser.Application.ImageServices.Models;
using Microsoft.Extensions.Options;
using System.Net;

namespace ManageUser.Infrastructure.Services
{
    public class ManageImageService: IManageImageService
    {
        private readonly CloudinarySettings _cloudinarySettings;

        public ManageImageService(IOptions<CloudinarySettings> cloudinarySettings)
        {
            _cloudinarySettings = cloudinarySettings.Value;
        }

        public async Task<ImageResponse> CloudinaryUploadImage(ImageData imageStream)
        {
            
            if (imageStream.ImageStream == null || string.IsNullOrEmpty(imageStream.Name))
            {
                throw new ArgumentException("Image stream and name must be provided.");
            }          


            //initialize the cloudinay account
            var account = new Account(_cloudinarySettings.CloudName,
                                    _cloudinarySettings.ApiKey,
                                    _cloudinarySettings.ApiSecret);

            //Cloudinary Client

            var cloudinary = new Cloudinary(account);

            //Upload Object

            var uploadImage = new ImageUploadParams()
            {
                File = new FileDescription(imageStream.Name, imageStream.ImageStream)
            };
            try
            {
                var uploadResult = await cloudinary.UploadAsync(uploadImage);
                // Check if the upload was successful
                if (uploadResult == null || uploadResult.StatusCode != HttpStatusCode.OK)
                {
                    string errorMsg = uploadResult?.Error?.Message ?? "Unknown error";
                    throw new Exception($"Upload failed: {errorMsg}");
                }
                else
                {
                    var response = new ImageResponse()
                    {
                        PublicId = uploadResult.PublicId,
                        Url = uploadResult.Url.ToString(),
                    };
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw new ImageUploadException($"Failed to set public ID for image '{imageStream.Name}'", ex);
            }       

        }

    }
}
