using System;

namespace Micro.Mails.Exceptions
{
    public class EmailSendingFailureException : Exception
    {
        public EmailSendingFailureException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
