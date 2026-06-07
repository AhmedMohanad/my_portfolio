namespace Portofolio.Models.UserModels
{
    public class BlacklistedToken
    {

        // this class is for logout functionality, to store the blacklisted tokens in the database
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
