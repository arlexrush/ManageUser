namespace ManageUser.Domain.Address
{
    public class NameCountry
    {
        public string common { get; set; }
        public string official { get; set; }
        public virtual List<NativeName> nativeNames { get; set; } = new List<NativeName>(); // Diccionario de nombres nativos por idioma
        public Country country { get; set; } // Relación con Country
    }
}
