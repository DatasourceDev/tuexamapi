using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tuexamapi.Models
{
    public class AttitudeSetup
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name = "จำนวนตัวเลือก")]
        public AttitudeAnsType AttitudeAnsType { get; set; }

        [Required]
        [Display(Name = "แบบที่")]
        public AttitudeAnsSubType AttitudeAnsSubType { get; set; }

        [MaxLength(250)]
        [Display(Name = "ตัวเลือกที่ 1")]
        public string Text1 { get; set; }
        [Display(Name = "ตัวเลือกที่ 2")]
        [MaxLength(250)]
        public string Text2 { get; set; }
        [Display(Name = "ตัวเลือกที่ 3")]
        [MaxLength(250)]
        public string Text3 { get; set; }
        [Display(Name = "ตัวเลือกที่ 4")]
        [MaxLength(250)]
        public string Text4 { get; set; }
        [Display(Name = "ตัวเลือกที่ 5")]
        [MaxLength(250)]
        public string Text5 { get; set; }
        [Display(Name = "ตัวเลือกที่ 6")]
        [MaxLength(250)]
        public string Text6 { get; set; }
        [Display(Name = "ตัวเลือกที่ 7")]
        [MaxLength(250)]
        public string Text7 { get; set; }

        [MaxLength(250)]
        [Display(Name = "ตัวเลือกที่ 1")]
        public string TextEn1 { get; set; }
        [Display(Name = "ตัวเลือกที่ 2")]
        [MaxLength(250)]
        public string TextEn2 { get; set; }
        [Display(Name = "ตัวเลือกที่ 3")]
        [MaxLength(250)]
        public string TextEn3 { get; set; }
        [Display(Name = "ตัวเลือกที่ 4")]
        [MaxLength(250)]
        public string TextEn4 { get; set; }
        [Display(Name = "ตัวเลือกที่ 5")]
        [MaxLength(250)]
        public string TextEn5 { get; set; }
        [Display(Name = "ตัวเลือกที่ 6")]
        [MaxLength(250)]
        public string TextEn6 { get; set; }
        [Display(Name = "ตัวเลือกที่ 7")]
        [MaxLength(250)]
        public string TextEn7 { get; set; }

        [Display(Name = "คำอธิบาย")]
        [MaxLength(1000)]
        public string Description { get; set; }

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
