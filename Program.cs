using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace ffcrm.UserEmailService
{
    class Program
    {
        static void Main(string[] args)
        {
            var activityStartTime = DateTime.Now;
            Console.WriteLine("Start Time: " + activityStartTime);
            var returnMessage = "Start Time: " + activityStartTime + " </ br> ";
            // log file
            string logFilePath = @"C:\Logs\UserEmailService_" + DateTime.Now.ToString("dd-MMM-yyyy-hhmm") + "." + "txt";
            // run service
            var response = InitService(logFilePath);
            // log summary message
            var elapsedTime = DateTime.Now - activityStartTime;
            var logMessage = "Start Time: " + activityStartTime + " | End Time: " + DateTime.Now + " | Elapsed Time: " + elapsedTime + Environment.NewLine;
            new Utils().WriteToLog(logFilePath, logMessage);
            returnMessage += " End Time: " + DateTime.Now + " </ br> ";
            returnMessage += " Elapsed Time: " + elapsedTime + " </ br> ";
            returnMessage += response;
            Console.WriteLine("End Time: " + DateTime.Now);
            // Email log file
            SendEmail(returnMessage, logFilePath);
        }


        private static string InitService(string logFilePath)
        {
            var returnMessage = "";
            string connection;
            string dataCenter;

            // DEV
            connection = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";
            returnMessage += "DEV";
            dataCenter = "DEV";


            //*** YOUR CODE STARTS HERE  - CREATE SEPARATE CLASSES/ FUNCTIONS ETC APPROPRIATELY ****//

            return returnMessage;
        }


        private static void SendEmail(string emailMessage, string logFilePath)
        {
            MailMessage mailMsg = new MailMessage();
            // To
            mailMsg.To.Add(new MailAddress("charles@firstfreight.com", "Charles Emerson"));
            mailMsg.To.Add(new MailAddress("devseff01@gmail.com", "DEV SE"));
            // From
            mailMsg.From = new MailAddress("admin@firstfreight.com", "CRM Admin");
            // reply to
            mailMsg.ReplyToList.Add(new MailAddress("admin@firstfreight.com", "Admin"));
            // subject and multipart/alternative body
            mailMsg.Subject = "First Freight User Email Service";
            mailMsg.Body = emailMessage;
            mailMsg.IsBodyHtml = true;
            mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString
                (emailMessage, null, MediaTypeNames.Text.Html)
            );
            // init SendGrid smtp and send
            var sendGridUsername = "azure_4cc10694f17920369e1b030c642df6c7@azure.com";
            var credentials = new NetworkCredential(sendGridUsername, "Sendgrid#1350");
            var smtpClient = new SmtpClient("smtp.sendgrid.net")
            {
                Credentials = credentials,
                EnableSsl = true,
                Port = 587
            };
            try
            {
                smtpClient.Send(mailMsg);
            }
            catch (Exception ex)
            {
                var logMessage = "Error: SendEmail " + ex.Message;
                new Utils().WriteToLog(logFilePath, logMessage);
                Console.WriteLine(logMessage);
            }
        }


    }
}
