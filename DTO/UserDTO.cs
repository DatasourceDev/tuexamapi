using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tuexamapi.DTO
{
    public class UserDTO
    {
        public int id { get; set; }
        public string prefix { get; set; }
        public string idcard { get; set; }
        public string email{ get; set; }
        public string phone { get; set; }
        public string phone2 { get; set; }
        public string opendate { get; set; }
        public string expirydate { get; set; }
        
        public string address { get; set; }
        public string passport { get; set; }
        public string userid { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string firstnameen { get; set; }
        public string lastnameen { get; set; }
        public string update_by { get; set; }
        public string status { get; set; }
        public int? faculty { get; set; }
        public string course { get; set; }
        public string studentcode { get; set; }
        
        public bool? isadmin { get; set; }
        public bool? ismasteradmin { get; set; }
        public bool? isquestionappr { get; set; }
        public bool? ismasterquestionappr { get; set; }
        public bool? istestappr { get; set; }
        public bool? ismastertestappr { get; set; }

    }
}
