using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tuexamapi.Models
{
    public class TestResultStudentQAns
    {
        [Key]
        public int ID { get; set; }

        public int? Index { get; set; }


        [Display(Name = "คำตอบแบบข้อความ")]
        [MaxLength(250)]
        public string TextAnswer { get; set; }

        [Display(Name = "คำตอบแบบไฟล์")]
        [MaxLength(250)]
        public string FileAnswerUrl { get; set; }

        [Display(Name = "คำตอบแบบ Attitude")]
        public int? AttitudeAnswer { get; set; }

        [Required]
        [Display(Name = "แบบทดสอบของผู้เข้าสอบ")]
        public int TestResultStudentID { get; set; }

        [Required]
        [Display(Name = "ข้อสอบ")]
        public int QuestionID { get; set; }

        [Display(Name = "คำตอบ")]
        public int? QuestionAnsID { get; set; }

        


        [Display(Name = "ตอบคำถามแล้ว")]
        public bool Answered { get; set; }
        public virtual TestResultStudent TestResultStudent { get; set; }
        public virtual Question Question { get; set; }
        public virtual QuestionAns QuestionAns { get; set; }


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

        /* After done on test*/
        [Display(Name = "คะแนนที่ได้")]
        public int Point { get; set; }


    }
}
