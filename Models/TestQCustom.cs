using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace tuexamapi.Models
{
    public class TestQCustom
    {
        [Key]
        public int ID { get; set; }
        public int? Order { get; set; }

        [Required]
        [Display(Name = "แบบทดสอบ")]
        public int TestID { get; set; }

        [Display(Name = "ข้อสอบ")]
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
