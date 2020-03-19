using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tuexamapi.Models
{
    public class LoginStaffHistory
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "ID ผุ้ใช้")]
        public int UserID { get; set; }

        [Display(Name = "ID เจ้าหน้าที่")]
        public int StaffID { get; set; }

        [Display(Name = "จำนวนครั้งการเข้าสู่ระบบ")]
        public int NumberOfLogin { get; set; }

        [Display(Name = "ประเภทการใช้งาน")]
        public AuthType AuthType { get; set; }

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

        public Staff Staff { get; set; }

    }
}
