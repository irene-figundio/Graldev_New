using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSite.Model;


namespace  WebSite.Model
{
    public class Utils
    {
        public const string SECTION_APP_INFO = "AppInfo";
        public const string SECTION_COOKIE_SETTINGS = "CookieSettings";
        public const string SECTION_MAIL_SETTINGS = "MailSettings";
        public const string SECTION_MAIL_TEMPLATE_INFO = "MailTemplateInfo";
        public readonly IOptions<MailSettings> _mailSettings;
        public readonly MailSettings mail;
        public const string IT_LANGUAGE = "it";
        public const string EN_LANGUAGE = "en";

        public MailHelper.MailHelper _mailHelper { get; set; }   

        public Utils(MailSettings options)
        {
            mail = options;
           /* MailSettings mail = new MailSettings
            {
                SMTP = options.Value.SMTP,
                From = options.Value.From,
                Footer = options.Value.Footer,
                Sender = options.Value.Sender
            };*/
            _mailHelper = new MailHelper.MailHelper
            {
                FromEmail = mail.From,
                FromEmailPwd = mail.SMTP.Password,
                Host = mail.SMTP.Server,
                Port = mail.SMTP.Port,
                EnableSSL = false,
                SenderName =mail.Sender
            };
        }
          
        

        private static Random random = new Random();



        public static string GenerateTestCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
