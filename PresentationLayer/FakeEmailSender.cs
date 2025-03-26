using Microsoft.AspNetCore.Identity.UI.Services;

namespace PresentationLayer
{
    public class FakeEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // You can log it, print to console, or just do nothing
            Console.WriteLine($"Fake email to {email}: {subject}");
            return Task.CompletedTask;
        }
    }
}
