using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace WebDispacher.Service.EmailSmtp
{
    public class AuthMessageSender
    {

        public async Task Execute(string email, string subject, string body)
        {
            try
            {
                int cId = 1;
                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress("truckonnow.info@gmail.com")
                };
                
                mail.To.Add(new MailAddress(email));
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("truckonnow.info@gmail.com", "flcasipusazpzfdo");
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}