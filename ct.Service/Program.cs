using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Service
{
    class Program
    {
        static void Main(string[] args)
        {

            //computer has to be pre-verified so there are no security prompts
            //you have to install autoIt3
            //you have to install firefox. Chrome doesn't trust the ssl cert

            var selenium = new DefaultSelenium("localhost",4444,"*firefox","https://www.bankofamerica.com");
            selenium.Start();
            selenium.Open("/");
            selenium.WaitForPageToLoad("30000");
            selenium.Type("id=id","USERID");
            selenium.Select("id=stateselect", "label=Tennessee");
            selenium.Click("id=top-button");
            selenium.WaitForPageToLoad("30000");
            selenium.Type("id=tlpvt-passcode-input","PASSWORD");
            selenium.Click("id=passcode-confirm-sk-submit");
            selenium.WaitForPageToLoad("30000");
            selenium.Click("id=Checking Account");
            selenium.WaitForPageToLoad("30000");
            selenium.Click("link=Download");
            selenium.WaitForPageToLoad("30000");
            while (!selenium.IsElementPresent("link=Download Transactions")) { System.Threading.Thread.Sleep(5000); }
            selenium.Click("link=Download Transactions");

            var p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.FileName = "C:\\Program Files (x86)\\AutoIt3\\AutoIt3.exe";
            p.StartInfo.Arguments = "path to...\\ff.au3"; //included in project. you can use executing path
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            selenium.Stop();


        }
    }
}
