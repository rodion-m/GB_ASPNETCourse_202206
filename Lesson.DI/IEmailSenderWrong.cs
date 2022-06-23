using MimeKit;

namespace Lesson.DI.Wrong;

public interface IEmailSender1
{
    void Send(MimeMessage message);
}