using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watsons.Common.EmailHelpers.DTO
{
    public class SendEmailBySpParams
    {
        public string? ProfileName { get; set; } = "noreplyaswatsons";
        public string? FromAddress { get; set; } = "";
        public List<string>? Recipients { get; set; }
        public List<string>? CopyRecipients { set; get; }
        public string? Subject { get; set; } = "no subject";
        public string? Body { get; set; } = "<blank>";
        public string? Format { get; set; } = "HTML";
        public List<string>? FileAttachments { get; set; }
    }
}
