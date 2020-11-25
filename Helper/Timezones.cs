using ffcrm.UserEmailService.Model;
using ffcrm.UserEmailService.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ffcrm.UserEmailService.Helper
{
    public class Timezones
    {
        /// <summary>
        /// Converts Origin DateTime  to UTC
        /// Accounts for Daylight Savings Time
        /// </summary>
        /// <param name="session"></param>/param>
        /// <param name="originTimeZone"></param>
        /// <param name="originDateTime"></param>
        /// <returns>UTC DateTime</returns>
        public DateTime? ConvertOriginDateTimeToUtc(Session session, string originTimeZone, DateTime? originDateTime)
        {
            DateTime? originDateTimeConvertedToUtc = null;
            try
            {
                // Get Offset Hours from Origin TimeZone to UTC
                var offsetHoursFromTimeZone = GetUtcOffsetHoursFromTimeZone(originTimeZone, session, originDateTime);
                // Get Offset Seconds from Origin TimeZone to UTC
                var originUtcOffsetSeconds = Convert.ToInt32(ConvertUtcOffsetToSeconds(offsetHoursFromTimeZone));
                if (originDateTime != null)
                {
                    var originDateTimeValue = originDateTime.Value;
                    originDateTimeConvertedToUtc = originDateTimeValue.AddSeconds(originUtcOffsetSeconds);
                }
            }
            catch (Exception)
            {
                // Could Not Convert DateTime to UTC
                originDateTimeConvertedToUtc = null;
            }
            return originDateTimeConvertedToUtc;
        }

        /// <summary>
        /// Converts Date Time from Origin TimeZone to Destination TimeZone
        /// </summary>
        /// <param name="currentSession"></param>/param>
        /// <param name="originTimeZone"></param>
        /// <param name="originDate"></param>
        /// <param name="originTime"></param>
        /// <param name="destinationTimeZone"></param>
        /// <returns>DateTime or Null</returns>
        public DateTime? ConvertTimezoneDateTime(Session currentSession, string originTimeZone, string originDate, string originTime, string destinationTimeZone)
        {
            // Origin Date Time
            var originDateTime = DateTime.Parse(originDate + " " + originTime);
            var offsetHoursFromTimeZone = GetUtcOffsetHoursFromTimeZone(originTimeZone, currentSession);
            // Get Offset Seconds from Origin TimeZone to GMT
            var originGmtOffsetSeconds = Convert.ToInt32(ConvertUtcOffsetToSeconds(offsetHoursFromTimeZone));
            // Destination Date Time
            var destinationGmtOffset = GetUtcOffsetHoursFromTimeZone(destinationTimeZone, currentSession, originDateTime);
            var destinationGmtOffsetSeconds = Convert.ToInt32(ConvertUtcOffsetToSeconds(destinationGmtOffset));
            // Calc Seconds Difference between Origin and Destination
            var intSecondsDifference = 0;
            // ---------------------------------------------------------------------------
            // Origin >= 0 and Destination > 0
            if (originGmtOffsetSeconds >= 0 & destinationGmtOffsetSeconds > 0)
            {
                intSecondsDifference = originGmtOffsetSeconds - destinationGmtOffsetSeconds;
            }
            // Origin >= 0 and Destination < 0
            if (originGmtOffsetSeconds >= 0 & destinationGmtOffsetSeconds < 0)
            {
                intSecondsDifference = destinationGmtOffsetSeconds - originGmtOffsetSeconds;
            }
            // Origin < 0 and Destination >= 0
            if (originGmtOffsetSeconds < 0 & destinationGmtOffsetSeconds >= 0)
            {
                intSecondsDifference = -originGmtOffsetSeconds + destinationGmtOffsetSeconds;
            }
            // Origin < 0 and Destination < 0
            if (originGmtOffsetSeconds < 0 & destinationGmtOffsetSeconds < 0)
            {
                intSecondsDifference = -originGmtOffsetSeconds + destinationGmtOffsetSeconds;
            }
            // Convert Origin DateTime to Destination DateTime
            var originDateTimeConvertedToDestination = originDateTime.AddSeconds(intSecondsDifference);
            return originDateTimeConvertedToDestination;
        }

        /// <summary>
        /// Converts UTC Offset String (GMT +5:00) into Seconds
        /// </summary> 
        /// <param name="utcOffset"></param>
        /// <returns>Seconds (integer)</returns>
        public string ConvertUtcOffsetToSeconds(string utcOffset)
        {
            var utcOffsetSeconds = "";
            if (string.IsNullOrEmpty(utcOffset)) return utcOffsetSeconds;
            var sign = utcOffset.Substring(0, 1);
            var hours = utcOffset.Substring(1, 2);
            var minutes = utcOffset.Substring(4, 2);
            utcOffsetSeconds = sign + ((Convert.ToInt32(hours) * 60 * 60) + (Convert.ToInt32(minutes) * 60));
            return utcOffsetSeconds;
        }

        /// <summary>
        /// Converts Integer to Day of the Week
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <returns>Name of the Day (text)</returns>
        public string GetDayOfWeekFromNumber(int dayOfWeek)
        {
            var strDay = "";
            switch (dayOfWeek)
            {
                case 0:
                    strDay = "Sunday";
                    break;
                case 1:
                    strDay = "Monday";
                    break;
                case 2:
                    strDay = "Tuesday";
                    break;
                case 3:
                    strDay = "Wednesday";
                    break;
                case 4:
                    strDay = "Thursday";
                    break;
                case 5:
                    strDay = "Friday";
                    break;
                case 6:
                    strDay = "Saturday";
                    break;
            }
            return strDay;
        }

        /// <summary>
        /// Gets the UTC Offset for a User
        /// Also Accounts for Daylight Savings Time if Date is Given
        /// </summary>
        /// <param name="session"></param>/param>
        /// <param name="dateTimeIn"></param>
        /// <returns>UTC Offset (text)</returns>
        public string GetUserUtcOffset(Session session, DateTime? dateTimeIn = null)
        {
            if (session.UserId <= 0) return "";
            var timeZone = GetUserTimeZone(session.UserId, session);
            var tzd = GetUtcDetailsFromTimeZone(timeZone, session, dateTimeIn);
            var utcOffset = tzd.UtcOffsetHours;
            return utcOffset;
        }

        /// <summary>
        /// Get's the TimeZone for a User
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="session"></param>/param>
        /// <returns>TimeZone (text)</returns>
        public string GetUserTimeZone(int userId, Session session)
        {
            var strTimeZone = "";
            if (userId <= 0) return strTimeZone;

            var context = new DbFirstFreightDataContext(new Utils().GetConnectionForDataCenter());

            var user = context.Users.Where(x => x.UserId == userId).FirstOrDefault();

            if (user != null)
            {
                strTimeZone = Util.NullToEmpty(user.TimeZone);
            }

            return strTimeZone;
        }

        /// <summary>
        /// Get's the TimeZone Details for a User
        /// Accounts for Daylight Savings if Date is Given
        /// </summary>
        /// <param name="session"></param>/param>
        /// <param name="dateTimeIn"></param>
        /// <returns>TimeZoneDetails (object)</returns>
        public UserTimeZone GetUserTimeZoneDetails(Session session, DateTime? dateTimeIn = null)
        {
            var utzd = new UserTimeZone();
            if (session.SubscriberId <= 0) return utzd;
            if (session.UserId == 99999)
            {
                // If File Upload User - Use GMT
                utzd.TimeZone = "GMT - Greenwich Mean Time";
                var tzd = GetUtcDetailsFromTimeZone(utzd.TimeZone, session, dateTimeIn);
                utzd.TzAbbreviation = "GMT";
                utzd.TzFullName = Util.NullToEmpty(tzd.TzFullName);
                utzd.UtcOffsetHours = Util.NullToEmpty(tzd.UtcOffsetHours);
                utzd.UtcOffsetSeconds = ConvertUtcOffsetToSeconds(tzd.UtcOffsetHours);
                //utzd.UtcOffsetMinutes = Convert.ToInt32(Zz(tzd.UTC_OffsetSeconds, 0)) / 60;
            }
            else
            {
                var context = new DbFirstFreightDataContext(new Utils().GetConnectionForDataCenter());

                var user = context.Users.Where(x => x.UserId == session.UserId && x.SubscriberId == session.SubscriberId).FirstOrDefault();

                if (user != null)
                {
                    utzd.TimeZone = Util.NullToEmpty(user.TimeZone);
                    var tzd = GetUtcDetailsFromTimeZone(utzd.TimeZone, session, dateTimeIn);
                    utzd.TzAbbreviation = Util.NullToEmpty(tzd.TzAbbreviation);
                    utzd.TzFullName = Util.NullToEmpty(tzd.TzFullName);
                    utzd.UtcOffsetHours = Util.NullToEmpty(tzd.UtcOffsetHours);
                    utzd.UtcOffsetSeconds = ConvertUtcOffsetToSeconds(tzd.UtcOffsetHours);
                    //utzd.UtcOffsetMinutes = Convert.ToInt32(Zz(tzd.UTC_OffsetSeconds, 0)) / 60;
                }
            }
            return utzd;
        }

        /// <summary>
        /// Get's the TimeZone Details for a TimeZone
        /// Accounts for Daylight Savings if Date is Given
        /// </summary>
        /// <param name="timeZone"></param>
        /// <param name="session"></param>/param>
        /// <param name="dateTimeIn"></param>
        /// <returns>TimeZoneDetails (object)</returns>
        public Model.TimeZone GetUtcDetailsFromTimeZone(string timeZone, Session session, DateTime? dateTimeIn = null)
        {
            var tzd = new Model.TimeZone();
            var sharedContext = new DbSharedDataContext(new Utils().GetSharedConnection());

            if (!(dateTimeIn == null))
            {
                // Is Daylight Savings Time

                var timezoneDst = sharedContext.TimeZonesDaylightSavings.Where(x =>
                x.TimeZoneName == Util.CleanString(timeZone) &&
                x.DstStartDate <= dateTimeIn.Value &&
                x.DstEndDate >= dateTimeIn.Value)
                    .FirstOrDefault();

                if (timezoneDst != null)
                {
                    tzd.TzAbbreviation = Util.NullToEmpty(timezoneDst.TimeZoneName);
                    tzd.TzFullName = Util.NullToEmpty(timezoneDst.TimeZoneFullName);
                    tzd.UtcOffsetHours = Util.NullToEmpty(timezoneDst.UtcOffset);
                    tzd.UtcOffsetSeconds = ConvertUtcOffsetToSeconds(tzd.UtcOffsetHours);
                    //tzd.UtcOffsetMinutes = Convert.ToInt32(tzd.UtcOffsetSeconds) / 60;
                }
            }
            if (!string.IsNullOrEmpty(tzd.UtcOffsetHours)) return tzd;
            // Not Daylight Savings Time

            var timezone = sharedContext.TimeZones.Where(x => x.TimeZoneName == Util.CleanString(timeZone)).FirstOrDefault();

            if (timezone != null)
            {
                tzd.TzAbbreviation = Util.NullToEmpty(timezone.TimeZoneName);
                tzd.TzFullName = Util.NullToEmpty(timezone.TimeZoneName);
                tzd.UtcOffsetHours = Util.NullToEmpty(timezone.UtcOffset);
                tzd.UtcOffsetSeconds = ConvertUtcOffsetToSeconds(tzd.UtcOffsetHours);
                //tzd.UtcOffsetMinutes = (Nz(tzd.UtcOffsetSeconds, 0)) / 60;
            }

            return tzd;
        }

        /// <summary>
        /// Get's the TimeZone Details for a User
        /// Accounts for Daylight Savings if Date is Given
        /// </summary>
        /// <param name="timeZone"></param>
        /// <param name="session"></param>/param>
        /// <param name="dateTimeIn"></param>
        /// <returns>UTC Offset (text)</returns>
        public string GetUtcOffsetHoursFromDstTimeZone(string timeZone, Session session, DateTime? dateTimeIn)
        {
            // Returns Hours String from GMT
            var utcOffsetHours = "";
            // Check If this is a DST Offset (Daylight Savings)
            if (dateTimeIn == null) return utcOffsetHours;

            var sharedContext = new DbSharedDataContext(new Utils().GetSharedConnection());

            var timezoneDst = sharedContext.TimeZonesDaylightSavings.Where(x =>
               x.TimeZoneName == Util.CleanString(timeZone) &&
               x.DstStartDate >= dateTimeIn &&
               x.DstEndDate <= dateTimeIn)
                   .FirstOrDefault();

            if (timezoneDst != null)
            {
                utcOffsetHours = Util.NullToEmpty(timezoneDst.UtcOffset);
            }

            return utcOffsetHours;
        }

        /// <summary>
        /// Get's the UTC Offset Hours for a TimeZone
        /// Accounts for Daylight Savings if Date is Given
        /// </summary>
        /// <param name="timeZone"></param>
        /// <param name="session"></param>/param>
        /// <param name="dateTimeIn"></param>
        /// <returns>UTC Offset (text)</returns>
        public string GetUtcOffsetHoursFromTimeZone(string timeZone, Session session, DateTime? dateTimeIn = null)
        {
            var sharedContext = new DbSharedDataContext(new Utils().GetSharedConnection());

            //Returns Hours String from UTC / GMT
            var utcOffsetHours = "";
            if (!(dateTimeIn == null))
            {
                // Check if Daylight Savings Time is In Effect
                var timezoneDst = sharedContext.TimeZonesDaylightSavings.Where(x =>
                   x.TimeZoneName == Util.CleanString(timeZone) &&
                   x.DstStartDate <= dateTimeIn &&
                   x.DstEndDate >= dateTimeIn)
                       .FirstOrDefault();

                if (timezoneDst != null)
                {
                    utcOffsetHours = Util.NullToEmpty(timezoneDst.UtcOffset);
                }
            }

            if (!string.IsNullOrEmpty(utcOffsetHours)) return utcOffsetHours;

            var timezone = sharedContext.TimeZones.Where(x =>
               x.TimeZoneName == Util.CleanString(timeZone))
                   .FirstOrDefault();

            if (timezone != null)
            {
                utcOffsetHours = Util.NullToEmpty(timezone.UtcOffset);
            }

            return utcOffsetHours;
        }

        /// <summary>
        /// Populates Times in 30 Minute Intervals for Time Dropdown
        /// Used by TimeZone Calculator Widget
        /// </summary>
        /// <returns>Select Option Vales (text)</returns>
        public string PopulateTimesDropdown()
        {
            var html = "";
            html += "<option value='01:00 am'>01:00 am</option>";
            html += "<option value='01:30 am'>01:30 am</option>";
            html += "<option value='02:00 am'>02:00 am</option>";
            html += "<option value='02:30 am'>02:30 am</option>";
            html += "<option value='03:00 am'>03:00 am</option>";
            html += "<option value='03:30 am'>03:30 am</option>";
            html += "<option value='04:00 am'>04:00 am</option>";
            html += "<option value='04:30 am'>04:30 am</option>";
            html += "<option value='05:00 am'>05:00 am</option>";
            html += "<option value='05:30 am'>05:30 am</option>";
            html += "<option value='06:00 am'>06:00 am</option>";
            html += "<option value='06:30 am'>06:30 am</option>";
            html += "<option value='07:00 am'>07:00 am</option>";
            html += "<option value='07:30 am'>07:30 am</option>";
            html += "<option value='08:00 am'>08:00 am</option>";
            html += "<option value='08:30 am'>08:30 am</option>";
            html += "<option value='09:00 am'>09:00 am</option>";
            html += "<option value='09:00 am'>09:30 am</option>";
            html += "<option value='10:00 am'>10:00 am</option>";
            html += "<option value='10:30 am'>10:30 am</option>";
            html += "<option value='11:00 am'>11:00 am</option>";
            html += "<option value='11:30 am'>11:30 am</option>";
            html += "<option value='12:00 pm'>12:00 pm</option>";
            html += "<option value='12:30 pm'>12:30 pm</option>";
            html += "<option value='01:00 pm'>01:00 pm</option>";
            html += "<option value='01:30 pm'>01:30 pm</option>";
            html += "<option value='02:00 pm'>02:00 pm</option>";
            html += "<option value='02:30 pm'>02:30 pm</option>";
            html += "<option value='03:00 pm'>03:00 pm</option>";
            html += "<option value='03:30 pm'>03:30 pm</option>";
            html += "<option value='04:00 pm'>04:00 pm</option>";
            html += "<option value='04:30 pm'>04:30 pm</option>";
            html += "<option value='05:00 pm'>05:00 pm</option>";
            html += "<option value='05:30 pm'>05:30 pm</option>";
            html += "<option value='06:00 pm'>06:00 pm</option>";
            html += "<option value='06:30 pm'>06:30 pm</option>";
            html += "<option value='07:00 pm'>07:00 pm</option>";
            html += "<option value='07:30 pm'>07:30 pm</option>";
            html += "<option value='08:00 pm'>08:00 pm</option>";
            html += "<option value='08:30 pm'>08:30 pm</option>";
            html += "<option value='09:00 pm'>09:00 pm</option>";
            html += "<option value='09:00 pm'>09:30 pm</option>";
            html += "<option value='10:00 pm'>10:00 pm</option>";
            html += "<option value='10:30 pm'>10:30 pm</option>";
            html += "<option value='11:00 pm'>11:00 pm</option>";
            html += "<option value='11:30 pm'>11:30 pm</option>";
            html += "<option value='12:00 am'>12:00 am</option>";
            html += "<option value='12:30 am'>12:30 am</option>";
            return html;
        }

        /// <summary>
        /// Populates TimeZones for TimeZone Dropdowns
        /// </summary>
        /// <returns>Select Option Values (text)</returns>
        public string PopulateTimeZonesDropdown(Session session, string defaultTimeZone = "")
        {
            var sharedContext = new DbSharedDataContext(new Utils().GetSharedConnection());

            var timezones = sharedContext.TimeZones.OrderBy(x => x.UtcOffset).ThenBy(x=>x.TimeZoneName);

            var sb = new System.Text.StringBuilder();

            if (string.IsNullOrEmpty(defaultTimeZone))
            {
                defaultTimeZone = "UTC Standard Time";
            }
            if (timezones.Any())
            {
                foreach (var timezoneModel in timezones)
                {
                    var timeZone = Util.NullToEmpty(timezoneModel.TimeZoneName);
                    var utcOffsetHours = Util.NullToEmpty(timezoneModel.UtcOffset);
                    var cities = Util.NullToEmpty(timezoneModel.CityNames);

                    if (timeZone != defaultTimeZone)
                    {
                        sb.Append("<option value='" + timeZone + "'>GMT" + utcOffsetHours + " | " + timeZone + " | " + cities + "</option>");
                    }
                    else
                    {
                        sb.Append("<option value='" + timeZone + "' SELECTED>GMT" + utcOffsetHours + " | " + timeZone + " | " + cities + "</option>");
                    }
                }
            }

            var html = sb.ToString();
            return html;
        }

        /// <summary>
        /// Populates TimeZones for TimeZone Dropdowns
        /// </summary>
        /// <param name="session"></param>
        /// <param name="dateTimeUtc"></param>
        /// <returns>Local DateTime (text)</returns>
        public string RenderLocalDateTimeFromUtcForUser(Session session, DateTime? dateTimeUtc)
        {
            var dateTimeOut = "";
            if (dateTimeUtc == null)
            {
                //NOT Valid Date Time
                return "";
            }
            try
            {
                //Convert UTC DateTime to Local User DateTime
                //var currentUser = new User
                //    {
                //        UserId = session.UserId,
                //        SubscriberId = new Subscribers().GetSubscriberIdFromUserId(session.UserId)
                //    };
                var dateFormat = Util.GetReportDateFormatForUser(session);
                var utzd = GetUserTimeZoneDetails(session, dateTimeUtc);
                var utcOffsetSeconds = Convert.ToInt32(utzd.UtcOffsetSeconds);
                var dateTimeUtcValue = dateTimeUtc.Value;
                var userLocalDateTime = dateTimeUtcValue.AddSeconds(utcOffsetSeconds);
                if (Util.IsValidDate(userLocalDateTime))
                {
                    //Verify that the Return Value is Valid
                    dateFormat += "hh:mm tt" + " " + utzd.TzAbbreviation + " (GMT" + utzd.UtcOffsetHours + ":" + utzd.UtcOffsetMinutes + ")";
                    dateTimeOut = userLocalDateTime.ToString(dateFormat);
                }
            }
            catch (Exception ex)
            {
                // Log Error
                new Logging().LogWebAppError(new WebAppError
                {
                    ErrorCode = "22",
                    ErrorCallStack = ex.StackTrace,
                    ErrorDateTime = DateTime.UtcNow,
                    RoutineName = "RenderLocalDateTimeFromUtcForUser",
                    SubscriberId = session.SubscriberId,
                    ErrorMessage = "Unable to Convert UtcDateTime: " + dateTimeUtc + " to Local User DateTime",
                    UserId = session.UserId
                });
                dateTimeOut = "";
            }
            return dateTimeOut;
        }

    }
}
