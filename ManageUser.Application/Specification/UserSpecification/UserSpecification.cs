using ManageUser.Application.AuthService.Models;

namespace ManageUser.Application.Specification.UserSpecification
{
    public class UserSpecification : BaseSpecification<ApplicationUser>
    {
        public UserSpecification(UserSpecificationParams userParams) : base(x => (string.IsNullOrEmpty(userParams.Search) ||
                                                                            x.FirstName.Contains(userParams.Search) ||
                                                                            x.LastName.Contains(userParams.Search) ||
                                                                            x.Email.Contains(userParams.Search)) && 
                                                                            (string.IsNullOrEmpty(userParams.TenantId) || x.TenantId == userParams.TenantId) && 
                                                                            (userParams.OnlyActive || x.IsActive == userParams.OnlyActive))
        {
            
            // Incluir la relación con el tenant
            AddInclude(x => x.Tenant);
            // Apply filters based on userParams
            ApplyPaging(userParams.PageSize * (userParams.PageIndex - 1), userParams.PageSize);

            if (!string.IsNullOrEmpty(userParams.Sort))
            {
                switch (userParams.Sort)
                {
                    case "nameAsc":
                        AddOrderBy(x => x.FirstName);
                        break;

                    case "nameDesc":
                        AddOrderByDescending(x => x.FirstName!);
                        break;

                    case "lastNameAsc":
                        AddOrderBy(x => x.LastName!);
                        break;

                    case "lastNameDesc":
                        AddOrderByDescending(x => x.LastName!);
                        break;

                    default:
                        AddOrderBy(x => x.LastName!);
                        break;

                }
            }
            else
            {
                AddOrderByDescending(x => x.FirstName!);
            }

        }
    }
}
