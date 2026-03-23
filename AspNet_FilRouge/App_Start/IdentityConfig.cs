// Identity services are now configured in Program.cs using ASP.NET Core DI
// This file is kept for reference only

namespace AspNet_FilRouge
{
    public class EmailService
    {
        public Task SendAsync(string destination, string subject, string body)
        {
            return Task.CompletedTask;
        }
    }

    public class SmsService
    {
        public Task SendAsync(string destination, string body)
        {
            return Task.CompletedTask;
        }
    }
}
