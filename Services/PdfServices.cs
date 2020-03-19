using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tuexamapi.Code.PdfServices
{
    public class PdfServices:IPdfServices
    {
        private string result;
        public string DecodeBase64BitString(string encodedString)
        {
            try
            {
                System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();

                byte[] todecodeByte = Convert.FromBase64String(encodedString);
                int charCount = utf8Decode.GetCharCount(todecodeByte, 0, todecodeByte.Length);
                char[] decodedChar = new char[charCount];
                utf8Decode.GetChars(todecodeByte, 0, todecodeByte.Length, decodedChar, 0);
                result = new String(decodedChar);
                return result;
            }
            catch (Exception e)
            {
                result = "Error in Encoded String " + e.Message;
                return result;
            }
        }

        public string GetHtmlTemplate()
        {
            throw new NotImplementedException();
        }
    }
}
