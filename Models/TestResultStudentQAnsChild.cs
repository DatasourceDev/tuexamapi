using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tuexamapi.Models
{
    public class TestResultStudentQAnsChild
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "คำตอบแบบข้อความ")]
        [MaxLength(250)]
        public string TextAnswer { get; set; }

        [Display(Name = "คำตอบแบบไฟล์")]
        [MaxLength(250)]
        public string FileAnswerUrl { get; set; }

        [Required]
        [Display(Name = "ข้อสอบ")]
        public int QuestionChildID { get; set; }

        [Required]
        [Display(Name = "คำตอบ")]
        public int QuestionAnsChildID { get; set; }

        [Required]
        [Display(Name = "คำถามคำตอบของผู้เข้าสอบ")]
        public int TestResultStudentQAnsID { get; set; }
        
        public virtual QuestionChild QuestionChild { get; set; }
        public virtual QuestionAnsChild QuestionAnsChild { get; set; }
        public virtual TestResultStudentQAns TestResultStudentQAns { get; set; }
        
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
