using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ct.Data.Repositories
{
    public static class EmbeddedSQL
    {
        public static string SQL(string FileName)
        {
            return new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format("ct.Data.sql.{0}.sql",FileName))).ReadToEnd();
        }
    }
}
