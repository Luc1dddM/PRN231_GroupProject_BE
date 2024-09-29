using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Identity.Configuration
{
    public class Jwt
    {
        public string Secret { get; set; }
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public string DurationInMinutes { get; set; }
        public string RefreshTokenExpiration { get; set; }
    }
}
