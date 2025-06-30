using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.Specification;

namespace ManageUser.Application.Features.Role.Queries.GetPagedRoles
{
    public class GetPagedRolesQuery :PaginationBaseQuery, IQuery<PaginationVm<GetPagedRolesQueryResponse>>
    {
       
    }
}
