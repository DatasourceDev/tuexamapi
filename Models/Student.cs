using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tuexamapi.Models
{
    public class Student
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name = "รหัสผู้เข้าสอบ")]
        [MaxLength(100)]
        public string StudentCode { get; set; }

        [Required]
        [Display(Name = "คำนำหน้า")]
        public Prefix Prefix { get; set; }

        [Display(Name = "ชื่อ(ไทย)")]
        [MaxLength(250,ErrorMessage ="จำนวนอักษรไม่ควรเกิน 250 ตัวอักษร")]
        public string FirstName { get; set; }

        [Display(Name = "นามสกุล(ไทย)")]
        [MaxLength(250, ErrorMessage = "จำนวนอักษรไม่ควรเกิน 250 ตัวอักษร")]
        public string LastName { get; set; }

        [Display(Name = "ชื่อ(อังกฤษ)")]
        [MaxLength(250, ErrorMessage = "จำนวนอักษรไม่ควรเกิน 250 ตัวอักษร")]
        public string FirstNameEn { get; set; }

        [Display(Name = "นามสกุล(อังกฤษ)")]
        [MaxLength(250, ErrorMessage = "จำนวนอักษรไม่ควรเกิน 250 ตัวอักษร")]
        public string LastNameEn { get; set; }

        [Required]
        [Display(Name = "สถานะการใช้งาน")]
        public StatusType Status { get; set; }

        [Display(Name = "รหัสบัตรประชาชน")]
        [MaxLength(14)]
        public string IDCard { get; set; }

        [MaxLength(100)]
        [Display(Name = "อีเมล")]
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

        [Display(Name = "คณะ")]
        [MaxLength(250)]
        public string Faculty { get; set; }

        [Display(Name = "หลักสูตร")]
        public Course Course { get; set; }
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
}
