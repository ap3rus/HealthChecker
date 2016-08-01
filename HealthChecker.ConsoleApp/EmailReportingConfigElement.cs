using System.Configuration;

namespace HealthChecker.ConsoleApp
{
    public class EmailReportingConfigElement: ConfigurationElement
    {
        [ConfigurationProperty("subject", IsRequired = true)]
        public string Subject
        {
            get { return (string)this["subject"]; }
            set { this["subject"] = value; }
        }

        [ConfigurationProperty("bodyTemplate", IsRequired = true)]
        public string BodyTemplate
        {
            get { return (string)this["bodyTemplate"]; }
            set { this["bodyTemplate"] = value; }
        }

        [ConfigurationProperty("addressSeparator", IsRequired = true)]
        public string AddressSeparator
        {
            get { return (string)this["addressSeparator"]; }
            set { this["addressSeparator"] = value; }
        }

        [ConfigurationProperty("emailFrom", IsRequired = true)]
        public string EmailFrom
        {
            get { return (string)this["emailFrom"]; }
            set { this["emailFrom"] = value; }
        }

        [ConfigurationProperty("emailTo", IsRequired = true)]
        public string EmailTo
        {
            get { return (string)this["emailTo"]; }
            set { this["emailTo"] = value; }
        }

    }
}