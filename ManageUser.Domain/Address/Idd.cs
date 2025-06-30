namespace ManageUser.Domain.Address
{
    public class Idd
    {
        public string root { get; set; } = default!; // Prefijo internacional, por ejemplo, "+34" para España
        public List<string> suffixes { get; set; } // Sufijos adicionales, por ejemplo, ["0", "1", "2"] para España
        public virtual Country Country { get; set; } = default!; // Relación con Country
    }
}
