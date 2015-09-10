using ct.Business.OFX.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ct.Tests.Business.OFX
{
    [TestClass]
    public class ChaseParserTests
    {
        [TestMethod]
        public void chase_ofx_can_be_parsed()
        {
            var ofx = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("ct.Tests.Business.OFX.chase.ofx")).ReadToEnd();
            var cp = new OFXParser(ofx);
            var trans = cp.GetTransactions();
            //var tps = trans.Select(t => t.TRNTYPE).Distinct();
            //var c = trans.Where(t => t.TRNTYPE == "CREDIT").ToList();
            Assert.IsNotNull(trans);
        }
    }
}
