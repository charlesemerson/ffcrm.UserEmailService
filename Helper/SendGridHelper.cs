using ffcrm.UserEmailService.Models;
using SendGrid;
using System;
using System.IO;
using System.Linq;
using SendGrid.Helpers.Mail;
using ffcrm.UserEmailService.Shared;

namespace ffcrm.UserEmailService.Helper
{
    public class SendGridHelper
    {
        private static readonly string SENDGRID_API_KEY = "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";
        private static readonly string SENDGRID_MESSAGE_ID = "X-Message-Id";

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
                var client = new SendGridClient(SENDGRID_API_KEY);
                var from = new SendGrid.Helpers.Mail.EmailAddress(request.Sender.EmailAddress, request.Sender.Name);
                var to = new SendGrid.Helpers.Mail.EmailAddress(recipient.EmailAddress);
                var subject = request.Subject;
                if (!string.IsNullOrEmpty(recipient.Name))
                    to.Name = recipient.Name;
                var htmlContent = request.HtmlBody;

                // create sendgrid message
                var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);

                // set reply to email address
                if (!string.IsNullOrEmpty(request.ReplyToEmail))
                    msg.ReplyTo = new SendGrid.Helpers.Mail.EmailAddress(recipient.EmailAddress);

                // bind attachments
                if (request.Attachments != null)
                {
                    foreach (var mailAttachment in request.Attachments)
                    {
                        var sendGridAttachment = GetSendGridAttachment(mailAttachment);
                        if (sendGridAttachment != null)
                        {
                            msg.Attachments.Add(sendGridAttachment);
                        }
                    }
                }

                // send email and wait for the response
                var response = client.SendEmailAsync(msg).Result;

                // get message id from headers
                var headers = response.Headers;
                var messageId = headers.GetValues(SENDGRID_MESSAGE_ID).FirstOrDefault();
                request.MessageId = messageId;


            }
            catch (Exception ex)
            {
                request.IsError = true;
                request.Error = ex.ToString();
            }

            // add email record
            AddEmailRecord(request, recipient);
        }



        public int AddEmailRecord(SendEmailRequest request, Recipient recipient)
        {
            var sharedConnection = Utils.GetSharedConnection(request.DataCenter);
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var currentDate = DateTime.Now;
            var currentUTCDate = DateTime.UtcNow;
            var email = new Email
            {
                DateSent = currentDate,
                Subject = request.Subject,
                EmailBodyHtml = request.HtmlBody,
                FromName = request.Sender.Name,
                FromEmailAddress = request.Sender.EmailAddress,
                ToEmail = recipient.EmailAddress,
                UtcSentDateTime = currentUTCDate,
                ReplyToEmailAddress = request.ReplyToEmail,
                Error = request.Error,
                IsError = request.IsError,
                MessageId = request.MessageId

            };
            sharedContext.Emails.InsertOnSubmit(email);
            sharedContext.SubmitChanges();
            return email.EmailId;
        }



        private Attachment GetSendGridAttachment(System.Net.Mail.Attachment attachment)
        {
            using (var stream = new MemoryStream())
            {
                try
                {
                    attachment.ContentStream.CopyTo(stream);
                    return new Attachment()
                    {
                        Disposition = "attachment",
                        Type = attachment.ContentType.MediaType,
                        Filename = attachment.Name,
                        ContentId = attachment.ContentId,
                        Content = Convert.ToBase64String(stream.ToArray())
                    };
                }
                finally
                {
                    stream.Close();
                }
            }
        }

    }
}
