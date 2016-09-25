using Microsoft.AspNet.Identity;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace AutoTrader.Service.Identity
{
    public class IdentityMessageService : IIdentityMessageService
    {
        private readonly IConfigurationSettings _configurationSettings;

        public IdentityMessageService(IConfigurationSettings configurationSettings)
        {
            if (configurationSettings == null) throw new ArgumentNullException(nameof(configurationSettings));

            _configurationSettings = configurationSettings;
        }

        public async Task SendAsync(IdentityMessage message)
        {
            await SendBySendGridAsync(message);
        }

        private async Task SendBySendGridAsync(IdentityMessage message)
        {
            dynamic sg = new SendGridAPIClient(_configurationSettings.SendGridApiKey);

            var from = new Email(_configurationSettings.SendGridEmailSender);
            var subject = message.Subject;
            var to = new Email(message.Destination);
            var content = new Content("text/html", message.Body);
            var mail = new Mail(from, subject, to, content);

            var templateId = _configurationSettings.ConfirmEmailTemplateId;
            mail.TemplateId = templateId;

            dynamic response = await sg.client.mail.send.post(requestBody: mail.Get());
        }
    }
}
