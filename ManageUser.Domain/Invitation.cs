namespace ManageUser.Domain
{
    public class Invitation : BaseEntity<string>
    {
        public string Email { get; set; }
        public string TenantId { get; set; }
        public string InvitedByUserId { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public bool Accepted { get; set; }
    }
}
