using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ffcrm.UserEmailService.Model
{
    public class Session
    {
        // Used for User Session - placed into localStorge after Login
        public string CountryCode { get; set; }
        public string CurrencyCode { get; set; }
        public string DateFormat { get; set; }
        public string DisplayLanguageCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ReportDateFormat { get; set; }
        public string SalesRepCode { get; set; }
        public int SubscriberId { get; set; }
        public string Theme { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserTimeZone { get; set; }
        public string UtcOffset { get; set; }
    }
}
