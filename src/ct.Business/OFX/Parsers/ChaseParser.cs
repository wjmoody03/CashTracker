using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ct.Business.OFX.Parsers
{
    public class ChaseParser
    {
        string ofx;

        public ChaseParser(string OFX)
        {
            ofx = OFX;
        }
        public IEnumerable<OFX.Models.Transaction> GetTransactions()
        {
            var rx = new Regex("<STMTTRN>(.|\r\n)*?<\\/STMTTRN>");
            var matches= rx.Matches(ofx);
            var transactions = new List<OFX.Models.Transaction>();
            foreach(Match m in matches)
            {
                transactions.Add(parseTransaction(m.Value));
            }
            return transactions;
        }
        private OFX.Models.Transaction parseTransaction(string STMTTRN)
        {
            //<STMTTRN>
            //<TRNTYPE>DEBIT
            //<DTPOSTED>20150706120000[0:GMT]
            //<TRNAMT>-79.85
            //<FITID>2015070624071055186158114069354
            //<NAME>PIZZERIA PULCINELLA
            //</STMTTRN>
            
            var t = new OFX.Models.Transaction();
            var rawDateString = getTransactionField("DTPOSTED", STMTTRN).Substring(0,8);
            t.DTPOSTED = DateTime.ParseExact(rawDateString, "yyyyMMdd", CultureInfo.CurrentCulture);
            t.FITID = getTransactionField("FITID", STMTTRN);
            t.NAME = getTransactionField("NAME", STMTTRN);
            t.TRNTYPE = getTransactionField("TRNTYPE", STMTTRN);
            t.TRNAMT = Decimal.Parse(getTransactionField("TRNAMT", STMTTRN));
            return t;
        }
        private string getTransactionField(string fieldName, string STMTTRN)
        {
            return Regex.Match(STMTTRN, string.Format("<{0}>(.|\r\n)*?<", fieldName))
                                .Value
                                .Trim()
                                .TrimEnd('<')
                                .Replace(string.Format("<{0}>", fieldName),"")
                                .Replace("\r\n","")
                                .Replace("\t","");
        }
    }
}
