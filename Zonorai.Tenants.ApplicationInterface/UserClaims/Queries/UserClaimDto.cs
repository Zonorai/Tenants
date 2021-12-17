namespace Zonorai.Tenants.ApplicationInterface.UserClaims.Queries
{
    public class UserClaimDto
    {
        public string UserId { get; set; }
        public string ClaimId { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}