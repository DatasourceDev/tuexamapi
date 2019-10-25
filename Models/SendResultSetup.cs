using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tuexamapi.Models
{
    public class SendResultSetup
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "อีเมล")]
        public bool SendByEmail { get; set; }
        [Display(Name = "ไปรษณีย์")]
        public bool SendByPost { get; set; }
        [Display(Name = "อื่นๆ")]
        public bool Other { get; set; }
        [Display(Name = "ข้อความ")]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "กลุ่มวิชา")]
        public int SubjectGroupID { get; set; }
        [Required]
        [Display(Name = "วิชา")]
        public int SubjectID { get; set; }

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
