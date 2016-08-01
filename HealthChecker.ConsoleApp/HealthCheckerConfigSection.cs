using System;
using System.Collections.Specialized;
using System.Configuration;

namespace HealthChecker.ConsoleApp
{
    public class HealthCheckerConfigSection: ConfigurationSection
    {
        [ConfigurationProperty("siteUri", IsRequired = true)]
        public Uri SiteUri
        {
            get { return (Uri) this["siteUri"]; }
            set { this["siteUri"] = value; }
        }

        [ConfigurationProperty("stateFileName", IsRequired = true)]
        public string StateFileName
        {
            get { return (string)this["stateFileName"]; }
            set { this["stateFileName"] = value; }
        }

        [ConfigurationProperty("addresses", IsRequired = true)]
        public string Addresses
        {
            get { return (string)this["addresses"]; }
            set { this["addresses"] = value; }
        }

        [ConfigurationProperty("emailReporting", IsRequired = true)]
        public EmailReportingConfigElement EmailReporting
        {
            get { return (EmailReportingConfigElement)this["emailReporting"]; }
            set { this["emailReporting"] = value; }
        }
    }
}