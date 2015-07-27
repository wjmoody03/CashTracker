using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Domain.Models
{
    public class Download
    {
        public int DownloadID { get; set; }
        public int AccountID { get; set; }
        public DateTime DownloadTime { get; set; }
        public string FileName { get; set; }

        public Account Account { get; set; }
    }
}
