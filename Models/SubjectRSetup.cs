using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace tuexamapi.Models
{
    public class SubjectRSetup
    {
        [Key]
        public int ID { get; set; }


        [Required]
        [MaxLength(1000, ErrorMessage = "จำนวนอักษรไม่ควรเกิน 1000 ตัวอักษร")]
        [Display(Name = "คำอธิบาย")]
        public string Description { get; set; }


        [Display(Name = "ค่ายืนยันจากส่วนที่ 1")]
        public int? SubjectSubfromPart1ID { get; set; }
        //[Required]
        //public virtual SubjectSub SubjectSubfromPart1 { get; set; }

        [Required]
        public decimal Percent { get; set; }

        [Required]
        public bool Sub1MoreThanPercent { get; set; }

        [Required]
        public bool Sub2MoreThanPercent { get; set; }

        
        [Required]
        public int SubjectSubID1 { get; set; }
        [Required]
        public int SubjectSubID2 { get; set; }
        //[Required]
        //public virtual SubjectSub SubjectSub1 { get; set; }
        //[Required]
        //public virtual SubjectSub SubjectSub2 { get; set; }

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
