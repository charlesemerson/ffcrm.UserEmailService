using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ffcrm.UserEmailService.Model
{
    public class SentReminder
    {
        public string CcEmails { get; set; }
        public int Clicks { get; set; }
        public DateTime DateSent { get; set; }
        public int EmailTemplateRef { get; set; }
        public String HtmlBody { get; set; }
        public string MandrillEmailId { get; set; }
        public int Opens { get; set; }
        public int ReminderSentId { get; set; }
        public int SubscriberId { get; set; }
        public string ToEmail { get; set; }
        public int UserId { get; set; }
    }
}
