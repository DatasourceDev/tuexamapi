using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace tuexamapi.Models
{
    public class TestApprovalStaff
    {
        [Key]
        public int ID { get; set; }
        public int TestApprovalID { get; set; }

        [Display(Name = "เจ้าหน้าที่")]
        public int StaffID { get; set; }

        [Display(Name = "สถานะการคัดเลือก")]
        public TestApprovalType TestApprovalType { get; set; }

        [Display(Name = "หมายเหตุ")]
        [MaxLength(1000)]
        public string Remark { get; set; }

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
        public virtual TestApproval TestApproval { get; set; }
        public virtual Staff Staff { get; set; }

    }
}
