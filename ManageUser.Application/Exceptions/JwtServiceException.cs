using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.Exceptions
{
    public class JwtServiceException:Exception
    {
        public JwtServiceException(string message) : base(message) { }
    }
}
