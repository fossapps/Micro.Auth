namespace Micro.Mails.Content
{
    public class MailBuilder
    {
        private readonly Sender _sender;

        public MailBuilder(Sender sender)
        {
            _sender = sender;
        }

        public ActivationEmail ActivationEmail()
        {
            return new ActivationEmail(_sender);
        }

        public ForgotPasswordEmail ForgotPasswordEmail()
        {
            return new ForgotPasswordEmail(_sender);
        }
    }
}
