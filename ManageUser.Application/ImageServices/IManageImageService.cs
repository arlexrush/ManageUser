using ManageUser.Application.ImageServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.ImageServices
{
    public interface IManageImageService
    {
        //the follow method is for upload an image
        public Task<ImageResponse> CloudinaryUploadImage(ImageData imageStream);
    }
}
