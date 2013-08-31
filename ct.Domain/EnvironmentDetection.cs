using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
namespace ct.Domain
{
    public interface IEnvironmentDetection
    {
        string AppSetting(string key);
        string configString(string name,string environment);
        string ConnectionString(string name);
        string CurrentEnvironment();
        string MachineName { get; }
        string MailServer { get; }
        int MailPort { get; }
        bool WriteEmailsToDisk { get; }
        string EmailPickupDirectoryLocation { get; }
        string SendEmailsFromAddress {get;}
        string SendEmailsFromName {get;}
        string ReplyToEmailAddress { get; }
        bool bundleScripts { get; }
        string ElmahLogPath { get; }
        string CustomLogPath { get; }

    }

    public class EnvironmentDetection : IEnvironmentDetection
    {

        public string MachineName { get; private set; }

        public EnvironmentDetection()
        {
            MachineName = Environment.MachineName;
        }
        public EnvironmentDetection(string machineName)
        {
            MachineName = machineName;
        }

        public string configString(string name, string environment)
        {
            return string.Format("{0}.{1}",environment,name);
        }

        public string CurrentEnvironment()
        {
            var value = ConfigurationManager.AppSettings[MachineName];
            if (string.IsNullOrEmpty(value))
                throw new NotImplementedException(string.Format("Cannot determine environment. Please add an environment name for {0} in the app settings of the web.config file.", MachineName));
            return value;
        }

        public string AppSetting(string key)
        {
            var setting= ConfigurationManager.AppSettings[configString(key,CurrentEnvironment())];
            if (string.IsNullOrWhiteSpace(setting))
            {
                setting = ConfigurationManager.AppSettings[configString(key, "DEFAULT")];
            }
            if (string.IsNullOrWhiteSpace(setting))
            {
                throw new NotImplementedException(string.Format("The app setting '{0}' has no value in this configuration, and it has no default value.", key));
            }
            
            return setting;
        }

        public string ConnectionString(string name)
        {
            var cs =  ConfigurationManager.ConnectionStrings[configString(name,CurrentEnvironment())];
            if (cs==null)
            {
                cs = ConfigurationManager.ConnectionStrings[configString(name, "DEFAULT")];
            }
            if (cs==null)
                throw new NotImplementedException(string.Format("The connection string setting '{0}' has no value in this configuration, and has no default value.", name));
            return cs.ConnectionString;
        }

        public string MailServer
        {
            get
            {
                return AppSetting("MailServer");
            }
        }
        public int MailPort
        {
            get
            {
                return int.Parse(AppSetting("MailPort"));
            }
        }
        public bool WriteEmailsToDisk
        {
            get
            {
                return bool.Parse(AppSetting("WriteEmailsToDisk"));
            }
        }

        public string SendEmailsFromAddress
        {
            get { return AppSetting("SendEmailsFromAddress"); }
        }

        public string SendEmailsFromName
        {
            get { return AppSetting("SendEmailsFromName"); }
        }

        public string EmailPickupDirectoryLocation
        {
            get { return AppSetting("EmailPickupDirectoryLocation"); }
        }

        public string ReplyToEmailAddress
        {
            get { return AppSetting("ReplyToEmailAddress"); }
        }

        public bool bundleScripts
        {
            get { return bool.Parse(AppSetting("bundleScripts")); }
        }

        public string ElmahLogPath
        {
            get { return AppSetting("ElmahLogPath"); }
        }

        public string CustomLogPath
        {
            get { return AppSetting("CustomLogPath"); }
        }

        public string SendErrorNotificationsToEmailAddress
        {
            get { return AppSetting("SendErrorNotificationsToEmailAddress"); }
        }

    }
}
