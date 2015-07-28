﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct.Domain
{
    public class CashTrackerConfigurationManager
    {
        public static string GoogleClientID
        {
            get { return ConfigurationManager.AppSettings["GoogleClientID"]; }
        }

        public static string GoogleClientSecret
        {
            get { return ConfigurationManager.AppSettings["GoogleClientSecret"]; }
        }

    }
}
