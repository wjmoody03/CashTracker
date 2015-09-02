﻿using System;
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
            ofx = System.Web.HttpUtility.HtmlDecode(OFX);
        }

        public decimal GetOutstandingBalance()
        {
            //< LEDGERBAL >
            //< BALAMT > -10151.05
            //< DTASOF > 20150831080000.000[-4:EDT]
            //</ LEDGERBAL >
            var rx = new Regex("<LEDGERBAL>(.|\r\n)*?<\\/LEDGERBAL>");
            var match = rx.Match(ofx);
            var bal = decimal.Parse(getUnclosedOFXField("BALAMT", match.Value)) * -1;
            return bal;
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
            var rawDateString = getUnclosedOFXField("DTPOSTED", STMTTRN).Substring(0,8);
            t.DTPOSTED = DateTime.ParseExact(rawDateString, "yyyyMMdd", CultureInfo.CurrentCulture);
            t.FITID = getUnclosedOFXField("FITID", STMTTRN);
            t.NAME = getUnclosedOFXField("NAME", STMTTRN);
            t.TRNTYPE = getUnclosedOFXField("TRNTYPE", STMTTRN);
            t.TRNAMT = Decimal.Parse((Decimal.Parse(getUnclosedOFXField("TRNAMT", STMTTRN)) * (t.TRNTYPE == "DEBIT" ? -1 : 1)).ToString("#.00"));
            return t;
        }
        private string getUnclosedOFXField(string fieldName, string ofxSection)
        {
            return Regex.Match(ofxSection, string.Format("<{0}>(.|\r\n)*?<", fieldName))
                                .Value
                                .Trim()
                                .TrimEnd('<')
                                .Replace(string.Format("<{0}>", fieldName),"")
                                .Replace("\r\n","")
                                .Replace("\t","");
        }
    }
}