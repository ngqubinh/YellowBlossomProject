using System.Net;
using System.Net.Mail;

namespace YellowBlossom.Infrastructure.Services
{
    public static class MailService
    {
        public async static Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            try
            {
                using (var smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential("nguyenbinh031104@gmail.com", "axnn dipn iwco rzug");
                    smtpClient.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("nguyenbinh031104@gmail.com"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true,
                    };

                    mailMessage.To.Add(recipientEmail);
                    await smtpClient.SendMailAsync(mailMessage);
                }
                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }
}
