using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.CQRSAbstractions
{
    public interface IQuery <TResult>
    {
        // This interface is a marker interface for queries in the CQRS pattern.
        // It doesn't define any members, but it can be used to identify query classes.
        // You can add common properties or methods for queries here if needed.
    }
}
