using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace tuexamapi.Models
{
    public class Question
    {
        [Key]
        public int ID { get; set; }

        [MaxLength(250)]
        [Display(Name ="รหัสข้อสอบ")]
        public string QuestionCode { get; set; }
        [Display(Name = "ประเภทข้อสอบ")]
        public QuestionType QuestionType { get; set; }

        [Display(Name = "หลักสูตร")]
        public bool CourseTh { get; set; }
        public bool CourseEn { get; set; }

        [Display(Name = "คำถาม(ไทย)")]
        [MaxLength(1000)]
        public string QuestionTh { get; set; }

        [Display(Name = "คำถาม(อังกฤษ)")]
        [MaxLength(1000)]
        public string QuestionEn { get; set; }

        [Display(Name = "ไฟล์คำถาม")]
        [MaxLength(500)]
        public string FileUrl { get; set; }

        [Display(Name = "เวลาที่อนุญาตให้ทำข้อสอบ")]
        public int? TimeLimit { get; set; }

        [Display(Name = "หน่วยของเวลาที่อนุญาตให้ทำข้อสอบ")]
        public TimeType TimeLimitType { get; set; }

        [Display(Name = "คำค้นหา")]
        [MaxLength(500)]
        public string Keyword { get; set; }

        [Display(Name = "ระดับความยาก")]
        public QuestionLevel QuestionLevel { get; set; }

        [Display(Name = "สถานะการใช้งาน")]
        public StatusType Status { get; set; }

        [Display(Name = "สถานะการกลั่นกรอง")]
        public QuestionApprovalType ApprovalStatus { get; set; }

        [Display(Name = "หมายเหตุ")]
        [MaxLength(1000)]
        public string Remark { get; set; }

        [Display(Name = "รูปแบบคำตอบ")]
        public AttitudeAnsType? AttitudeAnsType { get; set; }


        [Required]
        [Display(Name = "กลุ่มวิชา")]
        public int SubjectGroupID { get; set; }
        [Required]
        [Display(Name = "วิชา")]
        public int SubjectID { get; set; }
        [Required]
        [Display(Name = "วิชาย่อย")]
        public int SubjectSubID { get; set; }
        
        public virtual SubjectGroup SubjectGroup { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual SubjectSub SubjectSub { get; set; }

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
