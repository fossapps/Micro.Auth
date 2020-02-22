namespace Micro.Mails
{
    public class EmailUrlBuilder
    {
        private readonly EmailUrlConfig _config;

        public EmailUrlBuilder(EmailUrlConfig config)
        {
            _config = config;
        }

        public string BuildActivationUrl(string activationToken)
        {
            return $"{_config.AuthenticationUrlEndpoint}/account/activate/{activationToken}";
        }

        public string BuildPasswordResetFormUrl(string activationToken)
        {
            return $"{_config.AuthenticationUrlEndpoint}/account/reset/{activationToken}";
        }
    }
}
