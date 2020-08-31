using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using tuexamapi.Models;

namespace tuexamapi.DTO
{
    public class TestSendResultDTO
    {
        [Key]
        public int ID { get; set; }
      
        public string Email { get; set; }

      
        public string Address { get; set; }
                
        public string Create_By { get; set; }

      
        public string Update_By { get; set; }

        public bool SendByEmail { get; set; }

        public string Description { get; set; }
        public bool SendByPost { get; set; }
        public bool Other { get; set; }

    }
}
