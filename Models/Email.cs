using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ffcrm.UserEmailService.Models
{
    public class Recipient
    {
        public string EmailAddress { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public int ContactId { get; set; }
    }

    public class SendEmailRequest
    {
        public int SubscriberId { get; set; }
        public string Subject { get; set; }
        public int DealId { get; set; }
        public Recipient Sender { get; set; }
        public List<Recipient> UserRecipients { get; set; }
        public List<Recipient> ContactRecipients { get; set; }
        public List<Recipient> OtherRecipients { get; set; }
        public string HtmlBody { get; set; }
        public List<System.Net.Mail.Attachment> Attachments { get; set; }
        public string ReplyToEmail { get; set; }
        public string MessageId { get; set; }
        public bool IsError { get; internal set; }
        public string Error { get; internal set; }
        public string DataCenter { get; internal set; }
    }
}
