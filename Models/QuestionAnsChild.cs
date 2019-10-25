﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace tuexamapi.Models
{
    public class QuestionAnsChild
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name = "ลำดับ")]
        public int Order { get; set; }

        [Display(Name = "ตัวเลือก")]
        [MaxLength(500)]
        public string Choice { get; set; }

        [MaxLength(1000)]
        [Display(Name = "คำตอบ(ไทย)")]
        public string QuestionTh { get; set; }

        [MaxLength(1000)]
        [Display(Name = "คำตอบ(อังกฤษ)")]
        public string QuestionEn { get; set; }

        [MaxLength(1000)]
        [Display(Name = "คำอธิบาย")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "คะแนน")]
        public decimal Point { get; set; }


        [Display(Name = "ไฟล์คำถาม")]
        [MaxLength(500)]
        public string FileUrl { get; set; }

        [Required]
        [Display(Name = "คำถาม")]
        public int QuestionChildID { get; set; }

        public virtual QuestionChild QuestionChild { get; set; }


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
