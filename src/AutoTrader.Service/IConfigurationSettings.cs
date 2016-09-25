namespace AutoTrader.Service
{
    public interface IConfigurationSettings
    {
        string SendGridApiKey { get; }
        string SendGridEmailSender { get; }
        string FrontEndUrl { get; }
        string ResetPasswordTemplateId { get; }
        string ConfirmEmailTemplateId { get; }
    }
}
