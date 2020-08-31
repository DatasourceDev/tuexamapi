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
        [Display(Name = "รหัสข้อสอบ")]
        public string QuestionCode { get; set; }
        [Display(Name = "ประเภทข้อสอบ")]
        public QuestionType QuestionType { get; set; }

        [Display(Name = "หลักสูตร")]
        public bool CourseTh { get; set; }
        public bool CourseEn { get; set; }

        [Display(Name = "คำถาม(ไทย)")]
        public string QuestionTh { get; set; }

        [Display(Name = "คำถาม(อังกฤษ)")]
        public string QuestionEn { get; set; }

        [Display(Name = "ไฟล์")]
        [MaxLength(500)]
        public string FileName { get; set; }

        [Display(Name = "ไฟล์ Url")]
        [MaxLength(500)]
        public string FileUrl { get; set; }

        [Display(Name = "ประเภทไฟล์")]
        [MaxLength(100)]
        public string FileType { get; set; }

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

        [Display(Name = "แบบที่")]
        public AttitudeAnsSubType? AttitudeAnsSubType { get; set; }

        [Display(Name = "คะแนน ตัวเลือกถูก")]
        public decimal? TPoint { get; set; }
        [Display(Name = "คะแนน ตัวเลือกผิด")]
        public decimal? FPoint { get; set; }
        [Display(Name = "คะแนน")]
        public decimal? Point { get; set; }

        [Display(Name = "คะแนน 1")]
        public decimal? Point1 { get; set; }
        [Display(Name = "คะแนน 2")]
        public decimal? Point2 { get; set; }
        [Display(Name = "คะแนน 3")]
        public decimal? Point3 { get; set; }
        [Display(Name = "คะแนน 4")]
        public decimal? Point4 { get; set; }
        [Display(Name = "คะแนน 5")]
        public decimal? Point5 { get; set; }
        [Display(Name = "คะแนน 6")]
        public decimal? Point6 { get; set; }
        [Display(Name = "คะแนน 7")]
        public decimal? Point7 { get; set; }

        [Display(Name = "คำตอบย่อย 1")]
        public int? AnswerSubjectSub1 { get; set; }

        [Display(Name = "คำตอบย่อย 2")]
        public int? AnswerSubjectSub2 { get; set; }

        [Display(Name = "คำตอบย่อย 3")]
        public int? AnswerSubjectSub3 { get; set; }

        [Display(Name = "คำตอบย่อย 4")]
        public int? AnswerSubjectSub4 { get; set; }

        [Display(Name = "คำตอบย่อย 5")]
        public int? AnswerSubjectSub5 { get; set; }

        [Display(Name = "คำตอบย่อย 6")]
        public int? AnswerSubjectSub6 { get; set; }

        [Display(Name = "คำตอบย่อย 7")]
        public int? AnswerSubjectSub7 { get; set; }

        [Display(Name = "ประเภทคำตอบ")]
        public AnswerType AnswerType { get; set; }

        public decimal? MaxPoint { get; set; }

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

        [Display(Name = "ข้อสอบหลัก")]
        public int? QuestionParentID { get; set; }

        [Display(Name = "ลำดับ")]
        public int? ChildOrder { get; set; }

        [Display(Name = "ตัวเลือก")]
        public string Choice { get; set; }

        [Display(Name = "ลำดับ")]
        public int Order { get; set; }

        [Display(Name = "No")]
        public string No { get; set; }
        public virtual Question QuestionParent { get; set; }

        [Display(Name = "สลับคำตอบ")]
        public bool RandomChoice{ get; set; }


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
