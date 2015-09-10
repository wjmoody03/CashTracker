using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ct.Business.OFX.Request
{
    public class CreditCardTransactionRequest
    {
        static string ofxRequestBodyFormat = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("ct.Business.OFX.Request.CreditCardTransactionRequestBodyFormat.txt")).ReadToEnd();
        
        public static string GetOFX(string BankURL, string UserName, string Password, string AccountNumber, DateTime? StartDate = null)
        {
            if(StartDate== null)
            {
                StartDate = new DateTime(1970, 1, 1);
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BankURL);
            request.ContentType = "application/x-ofx";
            request.UserAgent = "OFX Tool";
            request.Headers.Set(HttpRequestHeader.CacheControl, "no-cache");
            var body = ofxRequestBodyFormat.Replace("{{UserName}}", UserName)
                                    .Replace("{{RequestDate}}", DateTime.Now.ToString("yyyyMMddhhmmss"))
                                    .Replace("{{Password}}", Password)
                                    .Replace("{{AccountNumber}}", AccountNumber)
                                    .Replace("{{StartDate}}", ((DateTime)StartDate).ToString("yyyyMMdd"));
            request.Method = "POST";
            request.ServicePoint.Expect100Continue = false;

            byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
            request.ContentLength = postBytes.Length;
            Stream stream = request.GetRequestStream();
            stream.Write(postBytes, 0, postBytes.Length);
            stream.Close();
            var response = (HttpWebResponse)request.GetResponse();
            string ofx = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return ofx;
        }

    }
}
