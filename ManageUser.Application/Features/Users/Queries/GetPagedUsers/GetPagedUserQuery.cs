using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.Specification;

namespace ManageUser.Application.Features.Users.Queries.GetPagedUsers
{
    public class GetPagedUserQuery: PaginationBaseQuery, IQuery<PaginationVm<GetPagedUserQueryResponse>>
    {
    }
}
