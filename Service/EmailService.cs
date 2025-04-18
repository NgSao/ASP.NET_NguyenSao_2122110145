namespace NguyenSao_2122110145.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using var client = new System.Net.Mail.SmtpClient(_configuration["Email:SmtpHost"], int.Parse(_configuration["Email:SmtpPort"]!));
            client.Credentials = new System.Net.NetworkCredential(_configuration["Email:SmtpUser"], _configuration["Email:SmtpPass"]);
            client.EnableSsl = true;

            var mailMessage = new System.Net.Mail.MailMessage
            {
                From = new System.Net.Mail.MailAddress(_configuration["Email:FromEmail"]!),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}