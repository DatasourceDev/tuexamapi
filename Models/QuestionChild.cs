using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace tuexamapi.Models
{
    public class QuestionChild
    {
        [Key]
        public int ID { get; set; }
       

        [Display(Name = "คำถาม(ไทย)")]
        [MaxLength(1000)]
        public string QuestionTh { get; set; }

        [MaxLength(1000)]
        [Display(Name = "คำถาม(อังกฤษ)")]
        public string QuestionEn { get; set; }

        [MaxLength(500)]
        [Display(Name = "ไฟล์คำถาม")]
        public string FileUrl { get; set; }
        

        [Required]
        [Display(Name = "คำถาม")]
        public int QuestionID { get; set; }
    
        public virtual Question Question { get; set; }

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
