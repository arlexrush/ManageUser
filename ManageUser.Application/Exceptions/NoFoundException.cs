using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.Exceptions
{
    public class NoFoundException: ApplicationException
    {
        public NoFoundException(string message) : base(message)
        {
        }
        public NoFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public NoFoundException() : base("The requested resource was not found.")
        {
        }

        public NoFoundException(string name, object key) : base($"Entity \"{name}\" ({key}) No founded") 
        {
        }
    }
}
