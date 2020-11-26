using System.Net;

namespace ffcrm.UserEmailService.Helper
{
    public class EmailTemplates
    {

        #region * RETRIEVE EMAIL TEMPLATES  *

        /// <summary>
        /// This function returns the template html string for 'WeeklyEmailTemplate.html'
        /// </summary>
        /// <returns>Returns an EmailTemplate html string</returns>
        public string GetWeeklyEmailTemplate()
        {
            //'WeeklySalesRepEmailTemplate
            const string templateUrl = "https://assets.firstfreight.com/templates/WeeklySalesRepEmailTemplate.html";
            var htmlTemplate = "";
            using (var client = new WebClient())
            {
                htmlTemplate = client.DownloadString(templateUrl);
            }
            return htmlTemplate;
        }

        #endregion


    }
}
