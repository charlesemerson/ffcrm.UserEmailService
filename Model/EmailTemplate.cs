using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ffcrm.UserEmailService.Model
{
    public class EmailTemplate
    {
        public string EmailSignature { get; set; }
        public int EmailTemplateId { get; set; }
        public int EmailTypeId { get; set; }
        public string EmailTypeText { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public int ReminderDays { get; set; }
        public int SubscriberId { get; set; }
        public string TemplateBody { get; set; }
        public string TemplateSubject { get; set; }
        public int TotalRecordsCount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string WeeklyEmailIncludes { get; set; }
        public string UserRole { get; set; }
        public string SendOnDay { get; set; }
        public DateTime? SendAtTime { get; set; }
        public string Tags { get; set; }
    }

    public class LoginFailedUsersAndTemplate
    {
        public List<User> Users { get; set; }
        public EmailTemplate EmailTemplate { get; set; }
    }


    public class PreviousLoginFailedEmailSentUsers
    {
        public EmailTemplate EmailTemplate { get; set; }
        public List<SentReminder> SentReminderList { get; set; }
    }


    public class EmailTemplateCcManagerType
    {
        public int EmailTemplateId { get; set; }
        public string ManagerType { get; set; }
    }
}
