namespace ManageUser.Domain.Address
{
    public class NativeName
    {
        public string official { get; set; }  // Nombre oficial del país en el idioma nativo
        public string common { get; set; } // Nombre común del país en el idioma nativo
        public virtual NameCountry nameCountry { get; set; } = default!; // Relación con NameCountry
    }
}
