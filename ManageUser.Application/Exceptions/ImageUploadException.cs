using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.Exceptions
{
    public class ImageUploadException : ApplicationException
    {
        public ImageUploadException(string message) : base(message)
        {
        }
        public ImageUploadException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public ImageUploadException() : base("Image upload failed.")
        {
        }
    }
}
