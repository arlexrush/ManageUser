using ManageUser.Application.AuthService.Models;

namespace ManageUser.Application.Specification.RoleSpecification
{
    public class RoleSpecification : BaseSpecification<ApplicationRole>
    {
        public RoleSpecification(RoleSpecificationParams roleParams)
            : base(x => string.IsNullOrEmpty(roleParams.Search) || x.Name.Contains(roleParams.Search))
        {
            if (roleParams.Sort is not null)
            {
                switch (roleParams.Sort.ToLower())
                {
                    case "nameasc":
                        AddOrderBy(r => r.Name!);
                        break;
                    case "namedesc":
                        AddOrderByDescending(r => r.Name!);
                        break;
                    default:
                        AddOrderBy(r => r.Name!);
                        break;
                }
            }
            else
            {
                AddOrderBy(r => r.Name!);
            }

            if (roleParams.PageIndex.HasValue && roleParams.PageSize.HasValue)
            {
                ApplyPaging((roleParams.PageIndex.Value - 1) * roleParams.PageSize.Value, roleParams.PageSize.Value);
            }
        }
    }
}
