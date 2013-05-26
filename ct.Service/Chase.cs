using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ct.Service
{
    public class Chase
    {
        public void MakeRequests()
        {
            HttpWebResponse response;

            if (Request_ofx_chase_com(out response))
            {
                Console.Write(new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd());
                response.Close();
                Console.Read();
            }
        }

        private bool Request_ofx_chase_com(out HttpWebResponse response)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://ofx.chase.com/");

                request.ContentType = "application/x-ofx";
                request.UserAgent = "OFX Tool";
                request.Headers.Set(HttpRequestHeader.CacheControl, "no-cache");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"OFXHEADER:100
DATA:OFXSGML
VERSION:102
SECURITY:NONE
ENCODING:USASCII
CHARSET:1252
COMPRESSION:NONE
OLDFILEUID:NONE
NEWFILEUID:NONE

<OFX>
<SIGNONMSGSRQV1>
<SONRQ>
<DTCLIENT>20130525060910
<USERID>USERID
<USERPASS>PASSSWORD
<LANGUAGE>ENG<FI>
 <ORG>B1
 <FID>10898
 </FI>
<APPID>QWIN
<APPVER>2000
</SONRQ>
</SIGNONMSGSRQV1>



<CREDITCARDMSGSRQV1>
<CCSTMTTRNRQ>
<TRNUID> madeuptranid
<CCSTMTRQ>
<CCACCTFROM>
<ACCTID> credit card number
</CCACCTFROM>
<INCTRAN>
<DTSTART>19700101
<INCLUDE>Y
</INCTRAN>
</CCSTMTRQ>
</CCSTMTTRNRQ>
</CREDITCARDMSGSRQV1>

</OFX>";
                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
                request.ContentLength = postBytes.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Close();

                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError) response = (HttpWebResponse)e.Response;
                else return false;
            }
            catch (Exception)
            {
                if (response != null) response.Close();
                return false;
            }

            return true;
        }


    }
}
