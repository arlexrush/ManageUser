using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.CQRSAbstractions
{
    public interface ICommandWResult <TResult>
    {
        // This interface is a marker interface for commands that return a result in the CQRS pattern.
        // It doesn't define any members, but it can be used to identify command classes that return a result.
        // You can add common properties or methods for commands with results here if needed.
    }
}
