using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tuexamapi.DTO
{
    public class Smtp
    {
        public string SMTP_SERVER { get; set; }
        public int SMTP_PORT { get; set; }
        public string SMTP_USERNAME { get; set; }
        public string SMTP_PASSWORD { get; set; }
        public bool STMP_SSL { get; set; }
        public string SMTP_FROM { get; set; }

    }
}
