using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Business.OFX.Models
{
    public class Transaction
    {
        //<STMTTRN>
        //<TRNTYPE>DEBIT
        //<DTPOSTED>20150706120000[0:GMT]
        //<TRNAMT>-79.85
        //<FITID>2015070624071055186158114069354
        //<NAME>PIZZERIA PULCINELLA
        //</STMTTRN>

        public string FITID { get; set; }
        public string TRNTYPE { get; set; }
        public decimal TRNAMT { get; set; }
        public DateTime DTPOSTED { get; set; }
        public string NAME { get; set; }
        public string MEMO { get; set; }
    }

}
