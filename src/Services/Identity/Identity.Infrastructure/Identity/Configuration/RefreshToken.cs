using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Identity.Configuration
{
    public class RefreshToken
    {
        public string ExpiredDurationInHour { get; set; }
        public string MaxDurationInDay { get; set; }
    }
}
