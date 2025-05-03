namespace YellowBlossom.Infrastructure.Services
{
    public class EmailSettings
    {
        public string Server { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public string SenderEmail { get; set; } = "nguyenbinh31104@gmail.com";
        public string SenderPassword { get; set; } = "axnn dipn iwco rzug";
    }

}
