using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watsons.Common.TrafficLogHelpers
{
    public partial class TrafficLog
    {
        public Guid RequestId { get; set; } = Guid.NewGuid();
        public string? AccessToken { get; set; }
        public string? AbsoluteUrlWithQuery { get; set; }
        public string? Action { get; set; }
        public string? Headers { get; set; }
        public string? Request { get; set; }
        public string? Response { get; set; }
        public string? HttpStatus { get; set; }
        public DateTime RequestDT { get; set; } = DateTime.Now;
        public DateTime? ResponseDT { get; set; }
        public double? TimeTaken { get; set; }
    }
}
