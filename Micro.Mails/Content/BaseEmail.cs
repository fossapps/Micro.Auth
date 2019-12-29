using System;
using System.IO;
using System.Threading.Tasks;

namespace Micro.Mails.Content
{
    public abstract class BaseEmail
    {
        protected readonly Sender MailConfig;
        protected BaseEmail(Sender sender)
        {
            MailConfig = sender;
        }

        protected async static Task<string> GetTemplateByNameAsync(string name, string extension)
        {
            try
            {
                return await File.ReadAllTextAsync($"./Content/Templates/{name}.{extension}");
            }
            catch (DirectoryNotFoundException)
            {
                return await File.ReadAllTextAsync($"../Micro.Mails/Content/Templates/{name}.{extension}");
            }
        }

        protected Task<string> GetHtmlTemplateAsync()
        {
            return GetTemplateByNameAsync(GetType().Name, "html");
        }

        protected Task<string> GetTextTemplateAsync()
        {
            return GetTemplateByNameAsync(GetType().Name, "txt");
        }
    }
}
