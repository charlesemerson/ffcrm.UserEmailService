using ffcrm.UserEmailService.Login;
using System;
using System.IO;
using System.Linq;

namespace ffcrm.UserEmailService
{
    public class Utils
    {
        public static string GetConnectionForDataCenter(string dataCenter = "")
        {
            // default connection
            var connectionString = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";
            switch (dataCenter.ToLower().Trim())
            {
                case "dev":
                    connectionString = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";
                    break;
                //case "emea":
                //    connectionString = "Data Source=ffemea.database.windows.net;Initial Catalog=CRM_EMEA;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                //    break;
                //case "hkg":
                //    connectionString = "Data Source=ffhkg.database.windows.net;Initial Catalog=CRM_HKG;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                //    break;
                //case "usa":
                //    connectionString = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_US;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                    break;
            }
            return connectionString;
        }

        public static bool IsBirthdayInRange(DateTime birthday, DateTime dateFrom, DateTime dateTo)
        {
            var temp = birthday.AddYears(dateFrom.Year - birthday.Year);

            if (temp < dateFrom)
                temp = temp.AddYears(1);

            return birthday <= dateTo && temp >= dateFrom && temp <= dateTo;
        }

        public static string GetDataCenter(int subscriberId)
        {
            var subscriberDataCenter = new DbLoginDataContext(GetLoginConnection())
                                                .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                .Select(t => t.DataCenter).FirstOrDefault();
            return subscriberDataCenter;
        }

        public static string GetLoginConnection(string dataCenter = "")
        {
            string loginConnection;
            switch (dataCenter)
            {
                case "DEV":
                    loginConnection = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test_Security;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";
                    break;
                default:
                    //loginConnection = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_Security;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                    loginConnection = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test_Security;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";

                    break;
            }

            return loginConnection;
        }

        public static string GetSharedConnection(string dataCenter = "")
        {
            string sharedConnectionString;
            switch (dataCenter)
            {
                case "DEV":
                    sharedConnectionString =
                        "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test_Shared;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";
                    break;
                default:
                    //  sharedConnectionString =  "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_Shared;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                    sharedConnectionString =
                        "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test_Shared;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";
                    break;
            }

            return sharedConnectionString;
        }

        public static string GetSharedConnectionForDataCenter(string dataCenter = "")
        {
            // default connection
            if (dataCenter == null)
                dataCenter = "";

            var connectionString = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test_Shared;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";
            switch (dataCenter.ToLower().Trim())
            {
                case "dev":
                    connectionString = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test_Shared;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";
                    break;
                //case "emea":
                //    connectionString = "Data Source=ffemea.database.windows.net;Initial Catalog=CRM_Shared;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                //    break;
                //case "hkg":
                //    connectionString = "Data Source=ffhkg.database.windows.net;Initial Catalog=CRM_Shared;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                //    break;
                //case "usa":
                //    connectionString = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_Shared;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                //    break;
            }
            return connectionString;
        }

        public string WriteToLog(string logFilePath, string logMessage)
        {
            var returnMessage = "";
            FileInfo logFileInfo = new FileInfo(logFilePath);
            DirectoryInfo logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
            if (!logDirInfo.Exists) logDirInfo.Create();

            using (FileStream fileStream = new FileStream(logFilePath, FileMode.Append))
            {
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    writer.WriteLine(logMessage + Environment.NewLine);
                }
            }
            return returnMessage;
        }
    }
}
