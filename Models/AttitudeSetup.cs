using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tuexamapi.Models
{
    public class AttitudeSetup
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name = "รูปแบบคำตอบ")]
        public AttitudeAnsType AttitudeAnsType { get; set; }

        [MaxLength(250)]
        [Display(Name = "ข้อความ")]
        public string Text1 { get; set; }
        [Display(Name = "ข้อความ")]
        [MaxLength(250)]
        public string Text2 { get; set; }
        [Display(Name = "ข้อความ")]
        [MaxLength(250)]
        public string Text3 { get; set; }
        [Display(Name = "ข้อความ")]
        [MaxLength(250)]
        public string Text4 { get; set; }
        [Display(Name = "ข้อความ")]
        [MaxLength(250)]
        public string Text5 { get; set; }
        [Display(Name = "ข้อความ")]
        [MaxLength(250)]
        public string Text6 { get; set; }
        [Display(Name = "ข้อความ")]
        [MaxLength(250)]
        public string Text7 { get; set; }

        [Display(Name = "คะแนน")]
        public decimal Point { get; set; }

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
    }
}
