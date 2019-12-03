using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tuexamapi.DTO
{
    public class ImportExamRegisterDTO
    {
        public ImportDTO fileupload { get; set; }
        public string examid { get; set; }

    }
    public class ImportDTO
    {
        public string filename { get; set; }
        public string filetype { get; set; }
        public string value { get; set; }

    }
}
