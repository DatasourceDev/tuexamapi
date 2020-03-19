using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace tuexamapi.Models
{
    public class TestResultStudent
    {

        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name = "แบบทดสอบ")]
        public int TestID { get; set; }

        [Required]
        [Display(Name = "การเข้าสอบ")]
        public int TestResultID { get; set; }     

        [Required]
        [Display(Name = "จำนวนข้อสอบ")]
        public int QuestionCnt { get; set; }

        [Required]
        [Display(Name = "ผู้เข้าสอบ")]
        public int StudentID { get; set; }

        [Required]
        [Display(Name = "รอบสอบ")]
        public int ExamID { get; set; }

        [Display(Name = "ช่องทางลงทะเบียน")]
        public ExamRegisterType ExamRegisterType { get; set; }

        public virtual Student Student { get; set; }
        public virtual Exam Exam { get; set; }

        public virtual Test Test { get; set; }
        public virtual TestResult TestResult { get; set; }


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

        [Display(Name = "เวลาที่เริ่มทำแบบทดสอบ")]
        public Nullable<DateTime> Start_On { get; set; }

        [Display(Name = "หมดเวลาแบบทดสอบ")]
        public Nullable<DateTime> Expriry_On { get; set; }

        [Display(Name = "เวลาที่สิ้นสุดแบบทดสอบ")]
        public Nullable<DateTime> End_On { get; set; }

        [Display(Name = "เวลาที่เหลือในการทำแบบทดสอบ")]
        public int? TimeRemaining { get; set; }

        [Required]
        [Display(Name = "สถานะการตรวจข้อสอบ")]
        public ExamingStatus ExamingStatus { get; set; }

        /* After done on test*/
        [Required]
        [Display(Name = "สถานะการตรวจข้อสอบ")]
        public ProveStatus ProveStatus  { get; set; }

        [Required]
        [Display(Name = "จำนวนข้อที่ทำ")]
        public int AnsweredCnt { get; set; }

        [Display(Name = "คะแนนที่ได้")]
        public decimal Point { get; set; }

        [Display(Name = "จำนวนข้อที่ถูก")]
        public int CorrectCnt { get; set; }

        [Display(Name = "จำนวนข้อที่ผิด")]
        public int WrongCnt { get; set; }

        [Display(Name = "อีเมล")]
        public bool SendByEmail { get; set; }
        [Display(Name = "ไปรษณีย์")]
        public bool SendByPost { get; set; }
        [Display(Name = "อื่นๆ")]
        public bool Other { get; set; }

        [Display(Name = "อีเมล")]
        [MaxLength(250)]
        public string Email { get; set; }

        [Display(Name = "ที่อยู่")]
        [MaxLength(1000)]
        public string Address { get; set; }

    }
}
