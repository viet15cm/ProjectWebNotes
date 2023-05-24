
namespace ProjectWebNotes.Services.MailServices
{
    public interface ISendMailServices
    {
        Task SendMail(MailContent content);
        Task<bool> SendMailAsync(MailContent content);
    }
}
