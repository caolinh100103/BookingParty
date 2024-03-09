using System.Net.Mail;
using BusinessLogicLayer.Interfaces;
using Microsoft.Extensions.Configuration;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using System.Net;
using MailKit.Security;
using MimeKit;

namespace BusinessLogicLayer;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task SendEmailAsync(string email, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_configuration["SmtpSettings:SenderName"],_configuration["SmtpSettings:SenderEmail"]));
        message.To.Add(new MailboxAddress(null, email));
        message.Subject = subject;

        var builder = new BodyBuilder();
        builder.HtmlBody = body;
        message.Body = builder.ToMessageBody();
        var port = _configuration["SmtpSettings:Port"];
        var portInt = int.Parse(port);
        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(_configuration["SmtpSettings:Host"], portInt, SecureSocketOptions.Auto);
            await client.AuthenticateAsync(_configuration["SmtpSettings:SenderEmail"],_configuration["SmtpSettings:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}