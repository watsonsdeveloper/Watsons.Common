using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Watsons.Common.EmailHelpers.DTO;
using Watsons.Common.EmailHelpers.Entities;

namespace Watsons.Common.EmailHelpers
{
    public class EmailHelper
    {
        private readonly EmailContext _context;
        private readonly EmailSettings _emailSettings;
        public EmailHelper(EmailContext context, IOptions<EmailSettings> emailSettings)
        {
            _context = context;
            _emailSettings = emailSettings.Value;
        }
        public async Task<bool> SendEmailBySmtp(string to, string subject, string body, string? attachment)
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
        public async Task<bool> SendEmailBySp(SendEmailBySpParams parameters)
        {
            var sqlQuery = "EXEC [dbo].[SendEmail] @var_profile_name, @var_from_address, @var_recipients, @var_copy_recipients, @var_file_attachments, @var_subject, @var_body, @var_format";
            var profileNameParam = new SqlParameter("@var_profile_name", parameters.ProfileName);
            var fromAddressParam = new SqlParameter("@var_from_address", parameters.FromAddress);
            var recipientsParam = new SqlParameter("@var_recipients", parameters.Recipients != null ? String.Join(";", parameters.Recipients) : "");
            var copyRecipientsParam = new SqlParameter("@var_copy_recipients", parameters.CopyRecipients != null ? String.Join(";", parameters.CopyRecipients) : "");
            var fileAttachmentsParam = new SqlParameter("@var_file_attachments", parameters.FileAttachments != null ? String.Join(";", parameters.FileAttachments) : "");
            var subjectParam = new SqlParameter("@var_subject", parameters.Subject);
            var bodyParam = new SqlParameter("@var_body", parameters.Body);
            var formatParam = new SqlParameter("@var_format", parameters.Format);

            var results = await _context.Database.ExecuteSqlRawAsync(sqlQuery, profileNameParam, fromAddressParam, recipientsParam, copyRecipientsParam, fileAttachmentsParam, subjectParam, bodyParam, formatParam);
            return true;
        }
    }
}
