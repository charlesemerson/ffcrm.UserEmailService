
namespace ffcrm.UserEmailService.Model
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public string SalesRepName { get; set; }
        public string SalesRepCode { get; set; }
        public string StationCode { get; set; }   
        public int SubscriberId { get; set; }
        public int SalesRepId { get; set; }
        public int SalesManagerUserId { get; set; }
        
        // Sync Type - Outlook | Exchange | Office365 | Google
        public string SyncType { get; set; }
        public string Password { get; set; }
        // User Settings
        public string CountryCode { get; set; }
        public string CurrencyCode { get; set; }
        public string DateFormat { get; set; }
        public string DisplayLanguageCode { get; set; }
        public string ReportDateFormat { get; set; }
        public string UserTimeZone { get; set; }
        public string UtcOffset { get; set; }
        // User Activation | Deactivation
        public System.DateTime UserActivationDate { get; set; }
        public System.DateTime UserDeactivationDate { get; set; }
    }
}
