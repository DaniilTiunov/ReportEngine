using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MimeKit;

namespace ReportEngine.App.Dds.Remote
{
    public class DdsMailService
    {

        private readonly string _senderName = "ReportEngine";
        private readonly string _senderEmail = "5677890@mail.ru";

        private readonly string _recipientName = "Recipient Name";
        private readonly string _recipientEmail = "a.reva@etalon-chel.ru";

        private readonly string _emailSubject = "Test Email";

        private readonly string _smtpServer = "smtp.mail.ru";
        private readonly int _smtpPort = 465;

        private readonly string _mailApiKey = "fpsy25l7264oorfLDb6G";


        public async Task SendMessage(string message)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_senderName, _senderEmail));
                email.To.Add(new MailboxAddress(_recipientName, _recipientEmail));
                email.Subject = _emailSubject;
                email.Body = new TextPart("plain")
                {
                    Text = message
                };
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_smtpServer, _smtpPort, true);
                await smtp.AuthenticateAsync("5677890@mail.ru", _mailApiKey);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                Console.WriteLine("Письмо отправлено!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }







        public async Task GetLastMessage()
        {
            using var client = new ImapClient();
            try
            {
                await client.ConnectAsync("imap.mail.ru", 993, true);
                await client.AuthenticateAsync("5677890@mail.ru", _mailApiKey);

                var inbox = client.Inbox;
                await inbox.OpenAsync(MailKit.FolderAccess.ReadOnly);
              
                Console.WriteLine($"Всего писем в папке Входящие: {inbox.Count}");

                if (inbox.Count > 0)
                {
                    // Получаем индекс последнего письма
                    int lastIndex = inbox.Count - 1;

                    // Загружаем само письмо
                    var message = await inbox.GetMessageAsync(lastIndex);

                    Console.WriteLine($"Тема: {message.Subject}");
                    Console.WriteLine($"От: {message.From}");
                    Console.WriteLine($"Дата: {message.Date}");

                    // Текст письма (plain text)
                    Console.WriteLine($"Текст: {message.TextBody}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }

        public async Task GetUnreadMessages()
        {
            using var client = new ImapClient();
            try
            {
                await client.ConnectAsync("imap.mail.ru", 993, true);
                await client.AuthenticateAsync("5677890@mail.ru", _mailApiKey);

                var inbox = client.Inbox;
                await inbox.OpenAsync(MailKit.FolderAccess.ReadOnly);

                Console.WriteLine($"Всего писем в папке Входящие: {inbox.Count}");
                // 1. Получаем список UID непрочитанных писем
                // SearchQuery.NotSeen — это и есть фильтр "не прочитано"
                var uids = await inbox.SearchAsync(SearchQuery.NotSeen);

                Console.WriteLine($"Найдено непрочитанных: {uids.Count}");

                // 2. Проходим по каждому письму
                foreach (var uid in uids)
                {
                    var message = await inbox.GetMessageAsync(uid);

                    Console.WriteLine($"Тема: {message.Subject}");
                    Console.WriteLine($"От: {message.From}");

                    // Если нужно — помечаем как прочитанное прямо здесь
                    // await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            finally
            {
                await client.DisconnectAsync(true);
            }

        }
    }
}
