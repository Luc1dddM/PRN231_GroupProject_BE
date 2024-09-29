using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Email.Configuration
{
    public class EmailSettings
    {
        public string MailServer { get; set; }
        public string FromEmail { get; set; }
        public string Password { get; set; }
        public int MailPort { get; set; }
    }
}
