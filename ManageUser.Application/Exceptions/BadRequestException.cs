using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.Exceptions
{
    public class BadRequestException: ApplicationException
    {
        public BadRequestException(string message) : base(message)
        {
        }
        public BadRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public BadRequestException() : base("Bad request.")
        {
        }
        public BadRequestException(string name, object key) : base($"Entity \"{name}\" ({key}) Bad request")
        {
        }
    }
}
