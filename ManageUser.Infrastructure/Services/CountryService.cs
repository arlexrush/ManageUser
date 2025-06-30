using ManageUser.Application.CountryService;
using ManageUser.Application.Repositories;
using ManageUser.Domain.Address;
using System.Text.Json;

namespace ManageUser.Infrastructure.Services
{
    public class CountryService : ICountryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpClientFactory _httpClientFactory;

        public CountryService(IUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory)
        {
            _unitOfWork = unitOfWork;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<Country>> GetAllAsync()
        {
            try
            {
                var countries = await _unitOfWork.Repository<Country>().GetAllAsync();
                if (countries is null || !countries.Any())
                {
                    throw new Exception("No se encontraron países en la base de datos");
                }
                return countries.ToList();
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                throw new Exception("Error al obtener todos los países", ex);
            }
        }
        public async Task<Country?> GetByCodeAsync(string cca2param)
        {
            var country = await _unitOfWork.Repository<Country>().GetByIdAsync(cca2param);
            if (country is null)
            {
                throw new Exception($"No se encontró el país con el código {cca2param}");
            }
            return country;
        }
        public async Task SeedCountriesAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var url = "https://restcountries.com/v3.1/all?fields=name,cca2,idd,currencies,languages";
                var response = await client.GetStringAsync(url);
                var countriesRaw = JsonSerializer.Deserialize<List<Country>>(response);
                if (countriesRaw is null) throw new Exception("Sin Respuesta del Server de Paises");
                var countries=countriesRaw.Where(c => !string.IsNullOrWhiteSpace(c.cca2) && c.name is null).Select(c=>new Country()
                    { 
                        Id = Guid.NewGuid().ToString(), 
                        name = new NameCountry()
                        {   
                            common = c.name.common,     
                            official = c.name.official, 
                            nativeNames = c.name.nativeNames.Select(n => new NativeName(){common = n.common, official = n.official}).ToList()
                        },
                        cca2 = c.cca2,
                        idd = new Idd(){root = c.idd.root, suffixes = c.idd.suffixes.Select(su=>su).ToList()},
                        currencies = c.currencies.Select(cu => new Currency() { name=cu.name, symbol=cu.symbol }).ToList(),
                        languages = c.languages.Select(l => l).ToList()
                    }
                ).ToList();
                
                // Verificar si ya existen países en la base de datos
                var existingCountries = await _unitOfWork.Repository<Country>().GetAllAsync();
                if (existingCountries.Any())
                {
                    // Si ya existen países, no se realiza la siembra
                    return;
                }
                else
                {   
                    // Si no existen países, se procede a sembrar
                    _unitOfWork.Repository<Country>().AddRange(countries);
                    await _unitOfWork.Complete();
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                throw new Exception("Error al sembrar los países", ex);
            }

        }
    }
}
