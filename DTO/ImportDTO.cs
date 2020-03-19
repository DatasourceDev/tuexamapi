using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tuexamapi.Models;

namespace tuexamapi.DTO
{
    public class ChooseDTO
    {
        public string choose { get; set; }
        public string examid { get; set; }
        public string update_by { get; set; }

    }
    public class ImageDTO
    {
        public string image { get; set; }
        public string update_by { get; set; }
    }
    public class ImportExamRegisterDTO
    {
        public ImportDTO fileupload { get; set; }
        public string examid { get; set; }
        public string subjectname { get; set; }
        public string update_by { get; set; }
        public AttitudeAnsType? attanstype { get; set; }

    }
    public class ImportQuestionDTO
    {
        public ImportDTO fileupload { get; set; }
        public string questionid { get; set; }
        public string update_by { get; set; }
        public Nullable<DateTime> update_on { get; set; }

    }
    public class ImportAnswerDTO
    {
        public ImportDTO fileupload { get; set; }
        public string tresultstudentid { get; set; }
        public string update_by { get; set; }
        public Nullable<DateTime> update_on { get; set; }

    }
    public class ImportDTO
    {
        public string filename { get; set; }
        public string filetype { get; set; }
        public string value { get; set; }

    }
}
