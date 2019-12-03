using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace tuexamapi.Models
{
    public class Test
    {
        [Key]
        public int ID { get; set; }

        [Display(Name ="รหัสแบบทดสอบ")]
        [MaxLength(100)]
        public string TestCode { get; set; }

        [Required]
        [MaxLength(250, ErrorMessage = "จำนวนอักษรไม่ควรเกิน 250 ตัวอักษร")]
        [Display(Name = "ชื่อแบบทดสอบ")]
        public string Name { get; set; }

        [Display(Name = "คำอธิบาย")]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Display(Name = "จำนวนข้อสอบ")]
        public int? QuestionCnt { get; set; }


        [Display(Name = "กำหนดข้อสอบ")]
        public TestQuestionType TestQuestionType { get; set; }

        [Display(Name = "ลำดับ")]
        public TestCustomOrderType TestCustomOrderType { get; set; }


        [Display(Name = "เวลาสอบ")]
        public int TimeLimit { get; set; }

        [Display(Name = "หน่วยของเวลาสอบ")]
        public TimeType TimeLimitType { get; set; }

        [Display(Name = "รูปแบบ")]
        public TestDoExamType TestDoExamType { get; set; }

        [Display(Name = "สถานะการใช้งาน")]
        public StatusType Status { get; set; }

        [Display(Name = "สถานะการคัดเลือก")]
        public TestApprovalType ApprovalStatus { get; set; }

        [Display(Name = "หมายเหตุ")]
        [MaxLength(1000)]
        public string Remark { get; set; }

        [Display(Name = "หลักสูตร")]
        public Course Course { get; set; }

        [Display(Name = "แสดงผลคะแนน")]
        public ShowResult ShowResult { get; set; }


        [Required]
        [Display(Name = "กลุ่มวิชา")]
        public int SubjectGroupID { get; set; }
        [Required]
        [Display(Name = "วิชา")]
        public int SubjectID { get; set; }
       
        [Required]
        public virtual SubjectGroup SubjectGroup { get; set; }
        public virtual Subject Subject { get; set; }

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
