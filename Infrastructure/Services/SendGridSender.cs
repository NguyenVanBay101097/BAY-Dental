using ApplicationCore.Entities;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class SendGridSender: IMailSender
    {
        private readonly SendGridConfig _config;
        public SendGridSender(IOptions<SendGridConfig> config)
        {
            _config = config.Value;
        }

        public async Task SendEmail(TMTEmail email)
        {
            await SendMail(new List<TMTEmail>() { email });
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            var client = new SendGridClient(_config.ApiKey);
            var from = new EmailAddress(_config.From, "TPOS.VN");
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);

            msg.AddSubstitution("[%first_name%]", email);
            msg.AddSubstitution("[from_name]", "TPOS.VN");
            msg.AddSubstitution("[sender_name]", "TPOS.VN");
            msg.AddSubstitution("[Sender_Name]", "TPOS.VN");
            msg.AddSubstitution("[Sender_Address]", "54/35 Diệp Minh Châu, P. Tân Sơn Nhì, Q. Tân Phú");
            msg.AddSubstitution("[Sender_City]", "Hồ Chí Minh");
            msg.AddSubstitution("[Sender_State]", "Việt Nam");
            msg.AddSubstitution("[Sender_Zip]", "700000");

            msg.SetTemplateId(_config.TemplateId);

            try
            {
                var response = await client.SendEmailAsync(msg);
            }
            catch { }
        }

        public async Task SendEmailAsync(string email, string subject, string body, string name)
        {
            var client = new SendGridClient(_config.ApiKey);
            var from = new EmailAddress(_config.From, "TPOS.VN");
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);

            msg.AddSubstitution("[%first_name%]", name);
            msg.AddSubstitution("[from_name]", "TPOS.VN");
            msg.AddSubstitution("[sender_name]", "TPOS.VN");
            msg.AddSubstitution("[Sender_Name]", "TPOS.VN");
            msg.AddSubstitution("[Sender_Address]", "54/35 Diệp Minh Châu, P. Tân Sơn Nhì, Q. Tân Phú");
            msg.AddSubstitution("[Sender_City]", "Hồ Chí Minh");
            msg.AddSubstitution("[Sender_State]", "Việt Nam");
            msg.AddSubstitution("[Sender_Zip]", "700000");

            msg.SetTemplateId(_config.TemplateId);

            try
            {
                var response = await client.SendEmailAsync(msg);
            }
            catch { }
        }

        public async Task SendMail(List<TMTEmail> emails)
        {
            try
            {
                if (emails != null && emails.Count > 0)
                {
                    var client = new SendGridClient(_config.ApiKey);

                    foreach (var item in emails)
                    {
                        var from = new EmailAddress(item.EmailFrom, "TPOS.VN");
                        var to = new EmailAddress(item.EmailTo, item.NameTo);
                        var msg = MailHelper.CreateSingleEmail(from, to, item.Subject, item.Body, item.Body);

                        msg.AddSubstitution("[%first_name%]", item.NameTo);
                        msg.AddSubstitution("[from_name]", "TPOS.VN");
                        msg.AddSubstitution("[sender_name]", "TPOS.VN");
                        msg.AddSubstitution("[Sender_Name]", "TPOS.VN");
                        msg.AddSubstitution("[Sender_Address]", "54/35 Diệp Minh Châu, P. Tân Sơn Nhì, Q. Tân Phú");
                        msg.AddSubstitution("[Sender_City]", "Hồ Chí Minh");
                        msg.AddSubstitution("[Sender_State]", "Việt Nam");
                        msg.AddSubstitution("[Sender_Zip]", "700000");

                        msg.SetTemplateId(_config.TemplateId);

                        var response = await client.SendEmailAsync(msg);
                    }
                }
            }
            catch
            {
            }
        }
    }


}
