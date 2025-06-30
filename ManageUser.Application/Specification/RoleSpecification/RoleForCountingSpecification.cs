using ManageUser.Application.AuthService.Models;

namespace ManageUser.Application.Specification.RoleSpecification
{
    public class RoleForCountingSpecification : BaseSpecification<ApplicationRole>
    {
        public RoleForCountingSpecification(RoleSpecificationParams roleParams)
            : base(x => string.IsNullOrEmpty(roleParams.Search) || x.Name.Contains(roleParams.Search))
        {

        }

    }
}
