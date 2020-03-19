using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace tuexamapi.Models
{
    public class Faculty
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "คณะ")]
        public string FacultyName { get; set; }      
    }
}
