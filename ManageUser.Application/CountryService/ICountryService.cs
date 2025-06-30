using ManageUser.Domain.Address;

namespace ManageUser.Application.CountryService
{
    public interface ICountryService // Changed from 'class' to 'interface'
    {
        Task<List<Country>> GetAllAsync();
        Task<Country?> GetByCodeAsync(string code);
        Task SeedCountriesAsync();
    }
}
