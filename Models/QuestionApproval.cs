using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace tuexamapi.Models
{
    public class QuestionApproval
    {
        [Key]
        public int ID { get; set; }
        public int QuestionID { get; set; }


        [Display(Name = "ผู้มีสิทธิ์กลั่นกรอง")]
        public int ApprovalCnt { get; set; }

        [Display(Name = "กลั่นกรองแล้ว")]
        public int ApprovedCnt { get; set; }

        [Display(Name = "ไม่ให้ผ่าน	")]
        public int RejectedCnt { get; set; }

        [Display(Name = "เวลาเปิดกลั่นกรอง")]
        public DateTime StartFrom { get; set; }

        [Display(Name = "เวลาปิดการกลั่นกรอง")]
        public DateTime EndFrom { get; set; }

        [Display(Name = "สถานะการกลั่นกรอง")]
        public QuestionApprovalType ApprovalStatus { get; set; }

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

        public virtual Question Question { get; set; }

    }
}
