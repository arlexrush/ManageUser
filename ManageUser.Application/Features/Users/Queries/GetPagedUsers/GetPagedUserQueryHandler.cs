using ManageUser.Application.AuthService.Models;
using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.Repositories;
using ManageUser.Application.Specification;
using ManageUser.Application.Specification.UserSpecification;
using Microsoft.AspNetCore.Http;

namespace ManageUser.Application.Features.Users.Queries.GetPagedUsers
{
    public class GetPagedUserQueryHandler : IQueryHandler<GetPagedUserQuery, PaginationVm<GetPagedUserQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetPagedUserQueryHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PaginationVm<GetPagedUserQueryResponse>> HandleAsync(GetPagedUserQuery query, CancellationToken cancellationToken)
        {
            var specParams = new UserSpecificationParams()
            {
                Sort = query.Sort,
                PageSize = query.PageSize,
                Search = query.Search,
                PageIndex = query.PageIndex,
            };

            var countSpec = new UserForCountingSpecification(specParams);
            var totalUsers = await _unitOfWork.Repository<ApplicationUser>().CountAsync(countSpec);
            var spec = new UserSpecification(specParams);
            var users = await _unitOfWork.Repository<ApplicationUser>().GetAllByIdWithSpec(spec);
            var rounded = Math.Ceiling((Convert.ToDecimal(totalUsers)) / (Convert.ToDecimal(query.PageSize)));
            var totalPage = Convert.ToInt32(rounded);
            var usersByPages = users.Count();
            var pagination = new PaginationVm<GetPagedUserQueryResponse>()
            {
                Count = totalUsers,
                Data = users.Select(u => new GetPagedUserQueryResponse
                {
                    UserName = u.UserName!,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber!,
                    IsActive = u.IsActive,
                    IsDeleted = u.IsDeleted,
                    TenantId = u.TenantId,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    CIF = u.CIF,
                }).ToList(),
                PageSize = query.PageSize,
                PageIndex = query.PageIndex,
                PageCount = totalPage,
                ResultByPage = usersByPages
            };
            return pagination;
        }
    }
}
