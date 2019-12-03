using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace tuexamapi.Models
{
    public class TestQRandom
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name = "ประเภทข้อสอบ")]
        public QuestionType QuestionType { get; set; }

        [Required]
        [Display(Name = "แบบทดสอบ")]
        public int TestID { get; set; }

        [Display(Name = "วิชาย่อย")]
        public int? SubjectSubID { get; set; }
       
        public virtual Test Test { get; set; }
        public virtual SubjectSub SubjectSub { get; set; }

        [Display(Name = "ง่ายมาก")]
        public int? VeryEasy { get; set; }
        [Display(Name = "ง่าย")]
        public int? Easy { get; set; }
        [Display(Name = "ปานกลาง")]
        public int? Mid { get; set; }
        [Display(Name = "ยาก")]
        public int? Hard{ get; set; }
        [Display(Name = "ยากมาก")]
        public int? VeryHard{ get; set; }

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
