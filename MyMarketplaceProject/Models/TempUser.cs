namespace MyMarketplaceProject.Models
{
    public class TempUser : RegisterModel
    {
        public int VerificationCode { get; set; }
        public DateTime VerificationCodeExpires { get; set; }
    }
}
