using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tuexamapi.Models
{
    public class LoginToken
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "ID ผุ้ใช้")]
        public int UserID { get; set; }

        [Display(Name = "ID ผู้เข้าสอบ")]
        public int StudentID { get; set; }

        [Display(Name = "Token")]
        public string Token { get; set; }

       
        [Display(Name = "ผู้สร้าง")]
        [MaxLength(250)]
        public string Create_By { get; set; }
        [Display(Name = "เวลาสร้าง")]
        public Nullable<DateTime> Create_On { get; set; }
        [Display(Name = "ผู้แก้ไข")]
        [MaxLength(250)]
        public string Update_By { get; set; }
        [Display(Name = "เวลาแก้ไข")]
        public Nullable<DateTime> Update_On { get; set; }

        [Display(Name = "เวลาหมดอายุ")]
        public Nullable<DateTime> ExpiryDate { get; set; }

    }
}
