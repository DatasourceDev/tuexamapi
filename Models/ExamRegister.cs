using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace tuexamapi.Models
{
    public class ExamRegister
    {
        [Key]
        public int ID { get; set; }       
        [Required]
        [Display(Name = "รอบสอบ")]
        public int ExamID { get; set; }

        [Required]
        [Display(Name = "ผู้เข้าสอบ")]
        public int StudentID { get; set; }

        public ExamRegisterType ExamRegisterType { get; set; }


        [Required]
        public virtual Exam Exam { get; set; }
        public virtual Student Student { get; set; }

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
