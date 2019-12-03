using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tuexamapi.Models
{
    public class Staff
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "รหัสผู้ใช้")]
        [MaxLength(250)]
        public string StaffCode { get; set; }

        [Required]
        [Display(Name = "คำนำหน้า")]
        public Prefix Prefix { get; set; }

        [Display(Name = "ชื่อ")]
        [MaxLength(250, ErrorMessage = "จำนวนอักษรไม่ควรเกิน 250 ตัวอักษร")]
        public string FirstName { get; set; }

        [Display(Name = "นามสกุล")]
        [MaxLength(250, ErrorMessage = "จำนวนอักษรไม่ควรเกิน 250 ตัวอักษร")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "สถานะการใช้งาน")]
        public StatusType Status { get; set; }

        [Display(Name = "รหัสบัตรประชาชน")]
        [MaxLength(14)]
        public string IDCard { get; set; }

        [Display(Name = "อีเมล")]
        [MaxLength(100)]
        public string Email { get; set; }

        [Display(Name = "โทรศัพท์")]
        [MaxLength(50)]
        public string Phone { get; set; }

        [Display(Name = "พาสปอร์ต")]
        [MaxLength(50)]
        public string Passport { get; set; }

        [Display(Name = "ที่อยู่")]
        [MaxLength(1000)]
        public string Address { get; set; }

        [Display(Name = "วันที่เปิดใช้งาน")]
        public DateTime? OpenDate { get; set; }

        [Display(Name = "วันที่หมดอายุ")]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "ผู้ใช้งาน")]
        public int UserID { get; set; }


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
        public User User { get; set; }



    }

    public class StaffRole
    {
        [Key]
        public int ID { get; set; }
        public int StaffID { get; set; }
        public Role Role { get; set; }
        public Staff Staff { get; set; }

    }
}
