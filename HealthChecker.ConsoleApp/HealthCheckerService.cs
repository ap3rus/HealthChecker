using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace HealthChecker.ConsoleApp
{
    public class HealthCheckerService
    {
        public void Run()
        {
            var config = (HealthCheckerConfigSection)System.Configuration.ConfigurationManager.GetSection("healthCheckerConfig");

            ConcurrentDictionary<string, bool> result = GetPreviousRunState(config.StateFileName);
            var failedAddresses = new ConcurrentBag<string>();
            Parallel.ForEach(config.Addresses.Split(','), address =>
            {
                bool previouslyAvailable;
                bool available = IsUrlAvailable(config.SiteUri, address);
                if (!available && result.TryGetValue(address, out previouslyAvailable) && !previouslyAvailable)
                {
                    failedAddresses.Add(address);
                }

                result[address] = available;
            });

            SaveCurrentRunState(config.StateFileName, result);
            if (failedAddresses.Count > 0)
            {
                ReportFailedAddresses(config.SiteUri, failedAddresses, config.EmailReporting);
            }
        }

        private ConcurrentDictionary<string, bool> GetPreviousRunState(string fileName)
        {
            var serializer = new JavaScriptSerializer();
            try
            {
                return serializer.Deserialize<ConcurrentDictionary<string, bool>>(File.ReadAllText(fileName));
            }
            catch
            {
                Trace.TraceWarning("Could not load state from file {0}, creating default.", fileName);
                return new ConcurrentDictionary<string, bool>();
            }
        }

        private void SaveCurrentRunState(string fileName, ConcurrentDictionary<string, bool> result)
        {
            var serializer = new JavaScriptSerializer();
            try
            {
                File.WriteAllText(fileName, serializer.Serialize(result));
                Trace.TraceInformation("New health state is saved to store.");
            }
            catch (Exception ex)
            {
                Trace.TraceError("Could not save state to the file {0}. Exception details: {1}.", fileName, ex);
            }
        }

        private void ReportFailedAddresses(Uri uri, ConcurrentBag<string> failedAddresses, EmailReportingConfigElement emailReportingConfig)
        {
            Trace.TraceWarning("{0} address failure(s) detected, generating report.", failedAddresses.Count);
            string body;
            try
            {
                body = string.Format(emailReportingConfig.BodyTemplate, uri,
                    string.Join(emailReportingConfig.AddressSeparator, failedAddresses.Select(a => a.ToString())));

            }
            catch (Exception ex)
            {
                Trace.TraceError("Could not format report body, please check configuration. Exception details: {0}", ex);
                return;
            }

            try
            {
                using (var mail = new MailMessage(emailReportingConfig.EmailFrom, emailReportingConfig.EmailTo, emailReportingConfig.Subject, body))
                using (var client = new SmtpClient())
                    client.Send(mail);
                Trace.TraceInformation("Reporting completed.");
            }
            catch (Exception ex)
            {
                Trace.TraceError("Could not send report email. Exception details: {0}", ex);
            }
        }

        private bool IsUrlAvailable(Uri uri, string address)
        {
            var uriWithIp = new Uri(string.Format("{0}://{1}{2}", uri.Scheme, address, uri.PathAndQuery));
            var hostname = uri.Host;
            var request = WebRequest.Create(uriWithIp);
            if (!(request is HttpWebRequest))
            {
                return false;
            }

            var webRequest = (HttpWebRequest)request;
            webRequest.Host = hostname;
            webRequest.Method = "HEAD";

            try
            {
                var response = (HttpWebResponse)webRequest.GetResponse();
                return (int)response.StatusCode < 400;
            }
            catch
            {
                return false;
            }
        }
    }
}