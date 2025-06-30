using ManageUser.Application.AuthService.Models;
using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.Repositories;
using ManageUser.Application.Specification;
using ManageUser.Application.Specification.RoleSpecification;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ManageUser.Application.Features.Role.Queries.GetPagedRoles
{
    public class GetPagedRolesQueryHandler : IQueryHandler<GetPagedRolesQuery, PaginationVm<GetPagedRolesQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetPagedRolesQueryHandler> _logger;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public GetPagedRolesQueryHandler(IUnitOfWork unitOfWork, ILogger<GetPagedRolesQueryHandler> logger, RoleManager<ApplicationRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager), "Role Manager cannot be null.");
        }

        public async Task<PaginationVm<GetPagedRolesQueryResponse>> HandleAsync(GetPagedRolesQuery query, CancellationToken cancellationToken)
        {
            var specParams = new RoleSpecificationParams()
            {
                Sort = query.Sort,
                PageSize = query.PageSize,
                Search = query.Search,
                PageIndex = query.PageIndex,

            };
            var countSpec = new RoleForCountingSpecification(specParams);
            var totalRoles = await _unitOfWork.Repository<ApplicationRole>().CountAsync(countSpec);
            var spec = new RoleSpecification(specParams);
            //var roles = await _unitOfWork.Repository<ApplicationRole>().GetAllByIdWithSpec(spec);
            var roles= _roleManager.Roles.ToList().Select(x=>new GetPagedRolesQueryResponse() { Name=x.Name!}).ToList();
            var rounded = Math.Ceiling((Convert.ToDecimal(totalRoles)) / (Convert.ToDecimal(query.PageSize)));
            var totalPage = Convert.ToInt32(rounded);
            var rolesByPages = roles.Count();
            var pagination= new PaginationVm<GetPagedRolesQueryResponse>()
            {
                Count = totalRoles,
                Data = roles,
                PageSize = query.PageSize,
                PageIndex = query.PageIndex,
                PageCount = totalPage,
                ResultByPage = rolesByPages
            };

            return pagination;
        }
    }
}
