using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace tuexamapi.Models
{
    public class Exam
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [MaxLength(250, ErrorMessage = "จำนวนอักษรไม่ควรเกิน 250 ตัวอักษร")]
        [Display(Name = "รหัสรอบสอบ")]
        public string ExamCode { get; set; }

        [Required]
        [Display(Name = "วันที่สอบ")]
        public DateTime ExamDate { get; set; }

        [Required]
        [Display(Name = "รอบสอบ")]
        public ExamPeriod ExamPeriod { get; set; }

        [Required]
        [Display(Name = "รูปแบบการเลือกแบบทดสอบ")]
        public ExamTestType ExamTestType { get; set; }

        [Display(Name = "สถานะการใช้งาน")]
        public StatusType Status { get; set; }

        [Required]
        [Display(Name = "กลุ่มวิชา")]
        public int SubjectGroupID { get; set; }
        [Required]
        [Display(Name = "วิชา")]
        public int SubjectID { get; set; }

        [Required]
        [Display(Name = "แบบทดสอบ")]
        public int TestID { get; set; }

        [Required]
        public virtual SubjectGroup SubjectGroup { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual Test Test { get; set; }


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
