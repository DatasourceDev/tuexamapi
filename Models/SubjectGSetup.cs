using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace tuexamapi.Models
{
    public class SubjectGSetup
    {
        [Key]
        public int ID { get; set; }


        [Required]
        [MaxLength(1000, ErrorMessage = "จำนวนอักษรไม่ควรเกิน 1000 ตัวอักษร")]
        [Display(Name = "คำอธิบาย type 1")]
        public string DescriptionType1 { get; set; }

        [Required]
        [MaxLength(1000, ErrorMessage = "จำนวนอักษรไม่ควรเกิน 1000 ตัวอักษร")]
        [Display(Name = "คำอธิบาย type 2")]
        public string DescriptionType2 { get; set; }


        [Required]
        [MaxLength(1000, ErrorMessage = "จำนวนอักษรไม่ควรเกิน 1000 ตัวอักษร")]
        [Display(Name = "คำอธิบาย type 3")]
        public string DescriptionType3 { get; set; }

        [Required]
        [Display(Name = "")]
        public decimal PercentByType { get; set; }

        [Required]
        [Display(Name = "")]
        public decimal PercentBySubjectSub { get; set; }

        [Required]
        [Display(Name = "คะแนน Type 1")]
        public decimal Type1Point { get; set; }

        [Required]
        [Display(Name = "คะแนน Type 2")]
        public decimal Type2Point { get; set; }

        [Required]
        [Display(Name = "คะแนน Type 3")]
        public decimal Type3Point { get; set; }

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
