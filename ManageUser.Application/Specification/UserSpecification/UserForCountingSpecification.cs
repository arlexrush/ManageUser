using ManageUser.Application.AuthService.Models;

namespace ManageUser.Application.Specification.UserSpecification
{
    public class UserForCountingSpecification : BaseSpecification<ApplicationUser>
    {
        public UserForCountingSpecification(UserSpecificationParams userParams) : base(x => (string.IsNullOrEmpty(userParams.Search) ||
                                                                                     x.FirstName!.Contains(userParams.Search!) ||
                                                                                     x.LastName!.Contains(userParams.Search) ||
                                                                                     x.Email!.Contains(userParams.Search)))
        {
        }
    }
}
