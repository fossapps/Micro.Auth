namespace Micro.Mails
{
    public class Sender
    {
        public string From { set; get; }
        public string Name { set; get; }
    }
    public class Smtp
    {
        public string Host { set; get; }
        public int Port { set; get; }
        public string User { set; get; }
        public string Password { set; get; }
    }
}
