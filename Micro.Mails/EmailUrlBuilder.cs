namespace Micro.Mails
{
    public class EmailUrlBuilder
    {
        private readonly string _baseUrl;

        public EmailUrlBuilder(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public string BuildActivationUrl(string activationToken)
        {
            return $"{_baseUrl}/account/activate/{activationToken}";
        }

        public string BuildPasswordResetFormUrl(string activationToken)
        {
            return $"{_baseUrl}/account/reset/{activationToken}";
        }
    }
}
