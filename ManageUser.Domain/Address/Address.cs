namespace ManageUser.Domain.Address
{
    public class Address
    {
        public string Street { get; set; } = default!;
        public string? Number { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string Cca2Address { get; set; } = default!; // ISO 3166-1 alpha-2
        public virtual Country Country { get; set; } = default!; // Relación con el país

        // Opcional: métodos de igualdad para Value Object
        public override bool Equals(object? obj)
        {
            if (obj is not Address other) return false;
            return Street == other.Street &&
                   Number == other.Number &&
                   City == other.City &&
                   State == other.State &&
                   PostalCode == other.PostalCode &&
                   Cca2Address == other.Cca2Address;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Street, Number, City, State, PostalCode, Cca2Address);
        }
    }
}
