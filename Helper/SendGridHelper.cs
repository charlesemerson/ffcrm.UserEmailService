using ffcrm.UserEmailService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace ffcrm.UserEmailService.Helper
{
    public class SendGridHelper
    {

        /// <summary>
        /// This function send the email using sendgrid API
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool SendEmail(SendEmailRequest request)
        {
            // send to users
            if (request.UserRecipients != null)
                foreach (var recipient in request.UserRecipients)
                {
                    SendEmail(recipient, request);
                }

            // send to contact recipient
            if (request.ContactRecipients != null)
                foreach (var recipient in request.ContactRecipients)
                {
                    SendEmail(recipient, request);
                }

            // TODO: direct email to non-CRM User or Contact ??

            // send to other recipient 
            if (request.OtherRecipients != null)
                foreach (var recipient in request.OtherRecipients)
                {
                    SendEmail(recipient, request);
                }

            return true;
        }


        private void SendEmail(Recipient recipient, SendEmailRequest request)
        {
            try
            {
                MailMessage mailMsg = new MailMessage();
                // To
                mailMsg.To.Add(new MailAddress(recipient.EmailAddress, recipient.EmailAddress));
                // From
                mailMsg.From = new MailAddress(request.Sender.EmailAddress, request.Sender.Name);

                if (!string.IsNullOrEmpty(request.ReplyToEmail))
                {
                    //mailMsg.ReplyToList = new MailAddressCo(request.ReplyToEmail);
                    mailMsg.ReplyToList.Add(request.ReplyToEmail);
                }

                // Subject and multipart/alternative Body
                mailMsg.Subject = request.Subject;
                string html = request.HtmlBody;
                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

                // Init SmtpClient and send
                SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("azure_4cc10694f17920369e1b030c642df6c7@azure.com", "Sendgrid#1350");
                smtpClient.Credentials = credentials;

                if (request.Attachments != null)
                {
                    foreach (var att in request.Attachments)
                    {
                        mailMsg.Attachments.Add(att);
                    }
                }

                smtpClient.Send(mailMsg);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
