namespace ManageUser.Domain.Address
{
    public class Currency
    {
        public string name { get; set; } = default!; // Nombre de la moneda
        public string symbol { get; set; } = default!; // Símbolo de la moneda
        public string CountryId { get; set; } = default!; // Identificador del país al que pertenece la moneda
        public virtual Country Country { get; set; } = default!; // Relación con el país

    }
}
