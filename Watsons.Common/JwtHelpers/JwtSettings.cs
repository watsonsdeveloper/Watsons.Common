using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watsons.Common.JwtHelpers
{
    public class JwtSettings
    {
        public string CookieName { get; set; } = null!;
        public string MfaApplicationId { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public double ExpiryInMinutes { get; set; }
    }
}
