namespace ManageUser.Domain.Address
{
    public class Country
    {
        public string Id { get; set; } // Identificador único del país
        public virtual NameCountry name { get; set; } // Nombre del país
        public string cca2 { get; set; } // Código de país de dos letras (ISO 3166-1 alpha-2)
        public virtual Idd idd { get; set; } // Información de prefijo telefónico
        public virtual List<Currency> currencies { get; set; } = new List<Currency>(); // Diccionario de monedas, clave es el código de la moneda
        public List<string> languages { get; set; } = new List<string>(); // Diccionario de idiomas, clave es el código del idioma
        public virtual List<Address> Addressess { get; set; } = new List<Address>(); // Relación con Address

    }
}
