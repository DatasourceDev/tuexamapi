using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tuexamapi.Code.PdfServices
{
    interface IPdfServices
    {
        string DecodeBase64BitString(string encodedString);
        string GetHtmlTemplate();
    }
}
