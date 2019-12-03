using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using tuexamapi.Models;

namespace tuexamapi.DTO
{
    public class ExamDTO
    {
        [Key]
        public int ID { get; set; }
      
        public string ExamCode { get; set; }

      
        public string ExamDate { get; set; }

        public ExamPeriod ExamPeriod { get; set; }

        public ExamTestType ExamTestType { get; set; }

        public StatusType Status { get; set; }

        public int SubjectGroupID { get; set; }
       
        public int SubjectID { get; set; }

        public int? TestID { get; set; }
        
        public string Create_By { get; set; }

      
        public string Update_By { get; set; }

    }
}
