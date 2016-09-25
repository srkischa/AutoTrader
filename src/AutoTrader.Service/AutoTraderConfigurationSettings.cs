using System.Configuration;

namespace AutoTrader.Service
{
    public class AutoTraderConfigurationSettings : IConfigurationSettings
    {
        public AutoTraderConfigurationSettings()
        {
            SendGridApiKey = ConfigurationManager.AppSettings["SendGridApiKey"];
            SendGridEmailSender = ConfigurationManager.AppSettings["SendGridEmailSender"];
            FrontEndUrl = ConfigurationManager.AppSettings["FrontEndUrl"];
            ConfirmEmailTemplateId = ConfigurationManager.AppSettings["ConfirmEmailTemplateId"];
            ResetPasswordTemplateId = ConfigurationManager.AppSettings["ResetPasswordTemplateId"];
        }

        public string ConfirmEmailTemplateId { get; private set; }

        public string FrontEndUrl { get; private set; }

        public string ResetPasswordTemplateId { get; private set; }

        public string SendGridApiKey { get; private set; }

        public string SendGridEmailSender { get; private set; }
    }
}
