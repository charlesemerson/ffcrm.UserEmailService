using System;

namespace ffcrm.UserEmailService.Model
{
    public class UserActivity
    {
        public int UserActivityId { get; set; }
        public int AccountId { get; set; }
        public int ActivityId { get; set; }
        public int ContactId { get; set; }
        public DateTime DateAddedUtc { get; set; } // Set When Record Created
        public string Description { get; set; }
        public string ExtendedDescription { get; set; }
        public int LeadId { get; set; }
        public string PageCalledFrom { get; set; }
        public int OpportunityId { get; set; }
        public User CurrentSession { get; set; }
    }
}
