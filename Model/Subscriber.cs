
using System;

namespace ffcrm.UserEmailService.Model
{
    public class Subscriber
    {
        public int SubscriberId { get; set; }
        public string City { get; set; }
        public string CompanyName { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string Country { get; set; }
        public string CrmAdminEmail { get; set; }
        public string DefaultReportCurrencyCode { get; set; }
        public string DefaultReportDateFormat { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string State { get; set; }
        public string SubscriberCode { get; set; }
        public string Zip { get; set; }
        public string SubDomain { get; set; }
        public string BaseColor { get; set; }
        public DateTime? MailChimpLastSync { get; set; }
        public DateTime? MailChimpLastCampaignSync { get; set; }
        public string MailChimpApiKey { get; set; }
        public string MailChimpListId { get; set; }
        public string MandrillApiKey { get; set; }
    }
}
