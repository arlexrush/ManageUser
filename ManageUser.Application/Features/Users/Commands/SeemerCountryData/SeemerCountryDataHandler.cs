using ManageUser.Application.CountryService;
using ManageUser.Application.CQRSAbstractions;

namespace ManageUser.Application.Features.Users.Commands.SeemerCountryData
{
    public class SeemerCountryDataCommandHandler : ICommandWResultHandler<SeemerCountryDataCommand, SeemerCountryDataCommandResponse>
    {
        private readonly ICountryService _countryService;

        public SeemerCountryDataCommandHandler(ICountryService countryService)
        {
            _countryService = countryService;
        }

        public async Task<SeemerCountryDataCommandResponse> HandleAsync(SeemerCountryDataCommand command, CancellationToken cancellationToken)
        {
            try
            {
                await _countryService.SeedCountriesAsync();
                return new SeemerCountryDataCommandResponse() { ErrorMessage = null, IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new SeemerCountryDataCommandResponse()
                {
                    ErrorMessage = ex.Message,
                    IsSuccess = false
                };

            }

        }
    }
}
