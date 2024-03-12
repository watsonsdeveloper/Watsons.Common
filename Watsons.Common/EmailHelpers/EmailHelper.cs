using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Watsons.Common.EmailHelpers
{
    public class EmailHelper
    {
        private readonly EmailSettings _emailSettings;
        public EmailHelper(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public async Task<bool> SendEmail(string to, string subject, string body, string? attachment)
        {
            try
            {
                using (var client = new SmtpClient(_emailSettings.SmtpServer))
                {
                    client.Port = _emailSettings.SmtpPort;
                    client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
                    client.EnableSsl = true;

                    var message = new MailMessage
                    {
                        From = new MailAddress(_emailSettings.Username),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    //if(!string.IsNullOrEmpty(attachment))
                    //{
                    //    Attachment attachment;
                    //    attachment = new Attachment(attachment);
                    //    message.Attachments.Add(attachment);
                    //}
                    message.To.Add(to);

                    client.Send(message);

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
