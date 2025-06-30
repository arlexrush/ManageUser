using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.CQRSAbstractions
{
    public interface ICommand
    {
        // This interface is a marker interface for commands in the CQRS pattern.
        // It doesn't define any members, but it can be used to identify command classes.
        // You can add common properties or methods for commands here if needed.
    }
}
