using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using ffcrm.UserEmailService.Model;
using ffcrm.UserEmailService.Shared;

namespace ffcrm.UserEmailService.Helper
{
    public static class Util
    {
        public enum SyncSource
        {
            Exchange,
            Google,
            O365,
            MailChimp
        }

        /// <summary>
        /// Convert Base64 String to Hex String
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns>Hex String</returns>
        public static String Base64StringToHexString(String base64String)
        {
            var bytes = Convert.FromBase64String(base64String);
            var sbHexString = new StringBuilder();
            foreach (var t in bytes)
            {
                sbHexString.Append(t.ToString("X2"));
            }
            return sbHexString.ToString();
        }

        /// <summary>
        /// Return Value or False if object is null
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True/False</returns>
        public static bool CheckBoolean(object value)
        {
            try
            {
                if (value == DBNull.Value) return false;
                if (value == null) return false;
                var result = Convert.ToBoolean(value);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Sanitize String for SQL Statements and Parameters
        /// </summary>
        /// <param name="textIn"></param>
        /// <returns>String</returns>
        public static string CleanString(string textIn)
        {
            var textOut = textIn.Replace("'", "''");
            textOut = textOut.Replace("<", "" + (char)60 + "");
            textOut = textOut.Replace(">", "" + (char)62 + "");
            return textOut;
        }

        /// <summary>
        /// Sanitize String
        /// </summary>
        /// <param name="textIn"></param>
        /// <returns>String</returns>
        public static string Cs(string textIn)
        {
            var textOut = textIn.Replace("'", "''");
            textOut = textOut.Replace("<", "" + (char)60 + "");
            textOut = textOut.Replace(">", "" + (char)62 + "");
            return textOut;
        }

        public static string CurrentLocalDateTime(Session session)
        {
            var utcDateTime = DateTime.UtcNow;
            var utzd = new Timezones().GetUserTimeZoneDetails(session, DateTime.Now);
            var utcOffsetSeconds = Convert.ToInt32(utzd.UtcOffsetSeconds);
            var localDateTime = utcDateTime.AddSeconds(utcOffsetSeconds);
            return localDateTime.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Format Bit
        /// </summary>
        /// <param name="bln"></param>
        /// <returns>1 or 0</returns>
        public static int FormatBit(bool bln)
        {
            var intOutput = bln ? 1 : 0;
            return intOutput;
        }

        /// <summary>
        /// DateTime Nullable Function
        /// </summary>
        /// <param name="objDateTime"></param>
        /// <returns>dateTime or null</returns>
        public static DateTime? GetDateTime(object objDateTime)
        {
            DateTime? dateTime = null;
            if (objDateTime == null) return null;
            if (objDateTime.ToString() != "")
            {
                // dateTime = Convert.ToDateTime(objDateTime);
                //avoid "String was not recognized as a valid DateTime" issue by using tryParse
                DateTime dt;
                var isDateTimeValid = DateTime.TryParse(objDateTime.ToString(), out dt);
                dateTime = isDateTimeValid ? dt : (DateTime?)null;
            }
            return dateTime;
        }

        /// <summary>
        /// Date Nullable Function
        /// </summary>
        /// <param name="objDateTime"></param>
        /// <returns>shortDate or null</returns>
        public static string GetDate(object objDateTime)
        {
            var shortDate = "";
            if (objDateTime == null) return null;
            if (objDateTime.ToString() != "")
            {
                DateTime? dateTime = Convert.ToDateTime(objDateTime);
                shortDate = dateTime.Value.ToShortDateString();
            }
            return shortDate;
        }

        /// <summary>
        /// Get Elapsed Time
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns>String</returns>
        public static string GetElapsedTime(DateTime? startTime, DateTime? endTime)
        {
            var elapsedTime = "00:00:00";
            if (endTime == null || startTime == null) return elapsedTime;
            var span = endTime.Value.Subtract(startTime.Value);
            var hours = PadString(Convert.ToString(span.Hours), 2, "0");
            var minutes = PadString(Convert.ToString(span.Minutes), 2, "0");
            var seconds = PadString(Convert.ToString(span.Seconds), 2, "0");
            elapsedTime = hours + ":" + minutes + ":" + seconds;
            return elapsedTime;
        }

        /// <summary>
        /// Convert Null Object to Empty String
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>String</returns>
        public static string NullToEmpty(object obj)
        {
            var returnString = "";
            if (obj != null)
            {
                returnString = obj.ToString();
            }
            return returnString;
        }

        /// <summary>
        /// Convert Null Object to Zero
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Integer</returns>
        public static int NullToZero(object obj)
        {
            var returnInteger = 0;
            if (obj == null) return returnInteger;
            try
            {
                returnInteger = Convert.ToInt32(obj);
            }
            catch (Exception)
            {
                returnInteger = 0;
            }
            return returnInteger;
        }

        /// <summary>
        /// Pad String
        /// </summary>
        /// <param name="textIn"></param>
        /// <param name="length"></param>
        /// <param name="padChar"></param>
        /// <returns>String</returns>
        public static string PadString(string textIn, int length, string padChar)
        {
            // Pad string number with zeros
            var textOut = "";
            for (var i = 1; i <= length; i++)
            {
                if (textIn.Length < length)
                {
                    textOut = padChar + textIn;
                }
                else
                {
                    textOut = textIn;
                }
            }
            return textOut;
        }

        /// <summary>
        /// Encrypt String
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Encrypted Base64 String</returns>
        public static string ProtectData(string data)
        {
            var bytes = Encoding.Unicode.GetBytes(data);
            var protectedData = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(protectedData);
        }

        /// <summary>
        /// Convert String to Byte Array
        /// </summary>
        /// <param name="hex"></param>
        /// <returns>Byte Array</returns>
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        /// <summary>
        /// Decrypt Data to String
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Unicode String</returns>
        public static string UnprotectData(string data)
        {
            var bytes = Convert.FromBase64String(data);
            var unprotectedData = ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);
            return Encoding.Unicode.GetString(unprotectedData);
        }

        /// <summary>
        /// Determine Whether CRM or O365 Record is Most Recent
        /// </summary>
        /// <param name="crmLastModified"></param>
        /// <param name="syncSourceLastModified"></param>
        /// <param name="syncSource"></param>
        /// <returns>CRM | O365 | None (text)</returns>
        public static string IsCrmOrSyncLastestVersion(DateTime? crmLastModified, DateTime? syncSourceLastModified, SyncSource syncSource = SyncSource.O365)
        {
            var updateType = "None";
            if (crmLastModified == null)
            {
                // CRM NO LastModified
                if (syncSourceLastModified != null)
                {
                    // O365 HAS LastModified - CRM NO LastModified
                    updateType = GetUpdateType(syncSource);
                }
            }
            else
            {
                if (syncSourceLastModified == null)
                {
                    // CRM HAS LastModified - O365 NO LastModified
                    updateType = "CRM";
                }
                else
                {
                    // BOTH have LastModified - See Whether CRM or O365 is Latest Version
                    updateType = syncSourceLastModified > crmLastModified ? GetUpdateType(syncSource) : "CRM";
                }
            }
            return updateType;
        }

        public static bool IsValidDate(DateTime? dateTimeIn)
        {
            var bln = false;
            try
            {
                var format = new CultureInfo("en-US", true);
                var dateTimeValue = dateTimeIn.ToString();
                var dateTimeUs = DateTime.Parse(dateTimeValue, format);
                if (dateTimeUs.Year > 0)
                {
                    bln = true;
                }
            }
            catch (Exception)
            {
                bln = false;
            }
            return bln;
        }

        static string GetUpdateType(SyncSource syncSource)
        {
            switch (syncSource)
            {
                case SyncSource.O365:
                    return "O365";
                case SyncSource.Google:
                    return "Google";
                case SyncSource.Exchange:
                    return "Exchange";
                default:
                    return "None";
            }
        }

        /// <summary>
        /// Validate Email
        /// </summary>
        /// <param name="emailAddress">Email Address to Validate</param>
        /// <returns>True for valid email, False for invalid</returns>
        public static bool ValidateEmail(string emailAddress)
        {
            try
            {
                const string pattern = "[A-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[A-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-z0-9](?:[A-z0-9-]*[a-z0-9])?\\.)+[A-z0-9](?:[A-z0-9-]*[A-z0-9])?";
                var emailAddressMatch = Regex.Match(emailAddress, pattern);
                return emailAddressMatch.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Verify that Start and End Dates for Date Range are Valid
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>True / False</returns>
        public static bool ValidStartEndDates(DateTime? startDate, DateTime? endDate)
        {
            var validDates = false;
            try
            {
                var dateDiff = endDate - startDate;
                if (dateDiff > TimeSpan.FromSeconds(10))
                {
                    validDates = true;
                }
            }
            catch (Exception)
            {
                validDates = false;
            }
            return validDates;
        }

        public static string GetReportDateFormatForUser(Session session)
        {
            var reportDateFormat = "";
            if (session.SubscriberId > 0)
            {
                if (session.UserId == 99999)
                {
                    //File Upload User
                    reportDateFormat = "MMM-dd-yyyy";
                }
                else
                {
                    var sql = "SELECT ReportDateFormat FROM tblUsers ";
                    sql += "WHERE (ID = " + session.SubscriberId + ") ";
                    sql += "AND (SubscriberID = " + session.UserId + ") ";
                    var ds = new DbHelper().GetDataSet(sql, session);
                    var dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        reportDateFormat = NullToEmpty(dt.Rows[0]["ReportDateFormat"]);
                    }
                    dt.Dispose();
                    ds.Dispose();
                }
            }
            if (reportDateFormat.Length == 0)
            {
                reportDateFormat = "MMM-dd-yyyy";
            }
            return reportDateFormat;
        }

        public static string RenderReportDateForUserSession(object objDate, Session session)
        {
            var reportDateOut = "";
            var dateFormat = GetReportDateFormatForUser(session);
            if (objDate == null | ReferenceEquals(objDate, DBNull.Value))
            {
                reportDateOut = "";
            }
            else
            {
                try
                {
                    var reportDate = Convert.ToDateTime(objDate);
                    reportDateOut = reportDate.ToString(dateFormat);
                }
                catch (Exception ex)
                {
                    // Log Error
                    new Logging().LogWebAppError(new WebAppError
                    {
                        ErrorCode = "25",
                        ErrorCallStack = ex.StackTrace,
                        ErrorDateTime = DateTime.UtcNow,
                        RoutineName = "RenderReportDate",
                        SubscriberId = session.SubscriberId,
                        ErrorMessage = "Date Error: " + objDate,
                        UserId = session.UserId
                    });
                }
            }
            return reportDateOut;
        }

        public static string RenderReportDateForUserId(object objDate, int userId)
        {
            var reportDateOut = "";
            var subscriberId = new Subscribers().GetSubscriberIdFromUserId(userId);
            var session = new Session
            {
                UserId = userId,
                SubscriberId = subscriberId
            };
            var dateFormat = GetReportDateFormatForUser(session);
            if (objDate == null | ReferenceEquals(objDate, DBNull.Value))
            {
                reportDateOut = "";
            }
            else
            {
                try
                {
                    var reportDate = Convert.ToDateTime(objDate);
                    dateFormat = "dd MMM yyyy";
                    reportDateOut = reportDate.ToString(dateFormat);
                }
                catch (Exception ex)
                {
                    // Log Error
                    new Logging().LogWebAppError(new WebAppError
                    {
                        ErrorCode = "25",
                        ErrorCallStack = ex.StackTrace,
                        ErrorDateTime = DateTime.UtcNow,
                        RoutineName = "RenderReportDate",
                        SubscriberId = session.SubscriberId,
                        ErrorMessage = "Date Error: " + objDate,
                        UserId = session.UserId
                    });
                }
            }
            return reportDateOut;
        }

        public static string RenderReportDateTime(object objDateTime, Session session)
        {
            var reportDateTimeOut = "";
            var dateFormat = GetReportDateFormatForUser(session);
            if (objDateTime == null)
            {
                return reportDateTimeOut;
            }
            try
            {
                var reportDate = Convert.ToDateTime(objDateTime);
                reportDateTimeOut = reportDate.ToString(dateFormat + " hh:mm tt");

            }
            catch (Exception ex)
            {
                // Log Error
                new Logging().LogWebAppError(new WebAppError
                {
                    ErrorCode = "25",
                    ErrorCallStack = ex.StackTrace,
                    ErrorDateTime = DateTime.UtcNow,
                    RoutineName = "RenderReportDateTime",
                    SubscriberId = session.SubscriberId,
                    ErrorMessage = "Date Error: " + objDateTime,
                    UserId = session.UserId
                });
            }
            return reportDateTimeOut;
        }

        public static string RenderCreatedLastUpdateHtml(int createdUserId,
                                                        DateTime? createdDateUtc,
                                                        string createdUserName,
                                                        int updatedUserId,
                                                        DateTime? lastUpdateUtc,
                                                        string updatedUserName,
                                                        Session session)
        {
            var html = "";
            //Created Date
            if (Convert.ToString(createdDateUtc) != "#12:00:00 AM#")
            {
                // TODO: createdUserId vs Session
                var createdDateLocalUserDataEntry = new Timezones().RenderLocalDateTimeFromUtcForUser(session, createdDateUtc);
                if (!string.IsNullOrEmpty(createdDateLocalUserDataEntry))
                {
                    html += "      <ul>";
                    html += "        <li>";
                    html += "            <label>Created</label>";
                    html += "           " + createdDateLocalUserDataEntry;
                    if (!string.IsNullOrEmpty(createdUserName))
                    {
                        html += "            by " + createdUserName;
                    }
                    html += "        </li>";
                    html += "      </ul>";
                }
            }
            //Last Update
            if (lastUpdateUtc == null) return html;
            var lastUpdatedLocalUserDataEntry = new Timezones().RenderLocalDateTimeFromUtcForUser(session, lastUpdateUtc);
            // TODO: createdUserId vs Session
            if (string.IsNullOrEmpty(lastUpdatedLocalUserDataEntry)) return html;
            html += "      <ul>";
            html += "        <li> ";
            html += "            <label>Updated</label>";
            html += "           " + lastUpdatedLocalUserDataEntry;
            if (!string.IsNullOrEmpty(updatedUserName))
            {
                html += "            by " + updatedUserName;
            }
            html += "        </li>";
            html += "      </ul>";
            return html;
        }

        public static int GetRowCount(string view, string searchCriteria, Session session)
        {
            var rowCount = 0;
            var sql = "SELECT COUNT(*) AS 'RowCount' ";
            sql += "FROM " + view + " ";
            sql += "WHERE (SubscriberID = " + session.SubscriberId + ") ";
            sql += "AND (Deleted = 0) ";
            sql += searchCriteria;
            var ds = new DbHelper().GetDataSet(sql, session);
            var dt = ds.Tables[0];
            if (dt.Rows.Count > 0)
            {
                rowCount = NullToZero(dt.Rows[0]["RowCount"]);
            }
            dt.Dispose();
            dt.Dispose();
            return rowCount;
        }

        public static double CastToDouble(object obj)
        {
            double returnDouble = 0;
            if (obj == null) return returnDouble;
            try
            {
                returnDouble = Convert.ToDouble(obj);
            }
            catch (Exception)
            {
                returnDouble = 0;
            }
            return returnDouble;
        }

    }
}
