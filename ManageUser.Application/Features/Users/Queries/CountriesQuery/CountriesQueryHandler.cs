using ManageUser.Application.CountryService;
using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.Repositories;
using ManageUser.Domain.Address;

namespace ManageUser.Application.Features.Users.Queries.CountriesQuery
{
    public class CountriesQueryHandler : IQueryHandler<CountriesQuery, List<CountriesQueryResponse>>
    {
        private readonly ICountryService _countryService;
        private readonly IUnitOfWork _unitOfWork;

        public CountriesQueryHandler(ICountryService countryService, IUnitOfWork unitOfWork)
        {
            _countryService = countryService;
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork), "Unit of Work cannot be null.");
        }

        public async Task<List<CountriesQueryResponse>> HandleAsync(CountriesQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var countries= (await _unitOfWork.Repository<Country>().GetAllAsync()).ToList();
                var response = countries.Select(c => new CountriesQueryResponse
                {
                    Caa2 = c.cca2,
                    OfficialName = c.name.official
                }).ToList();
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los paises", ex);

            }
        }

    }
}
