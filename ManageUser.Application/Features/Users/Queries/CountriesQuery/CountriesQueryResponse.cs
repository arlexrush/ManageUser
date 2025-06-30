namespace ManageUser.Application.Features.Users.Queries.CountriesQuery
{
    public class CountriesQueryResponse
    {
        public string Caa2 { get; set; } = default!; // Código ISO 3166-1 alpha-2
        public string OfficialName { get; set; } = default!; // Nombre del país
        
    }
}
