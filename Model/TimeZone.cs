using System;

namespace ffcrm.UserEmailService.Model
{

    public class TimeZone
    {
        public int TzId = 0;
        public string CityNames = "";
        public int CreatedById = 0;
        public DateTime ? CreatedDate = null;
        public string CreatedUserName = "";
        public DateTime ? DateDeleted = null;
        public bool Deleted = false;
        public int DeletedById = 0;
        public DateTime ? LastUpdate = null;
        public string TimeZoneName = "";
        public string TzAbbreviation = "";
        public string TzFullName = "";
        public string UpdateUserName = "";
        public int UpdateUserId = 0;
        public string UtcOffsetHours = "";
        public string UtcOffsetMinutes = "";
        public string UtcOffsetSeconds = "";
    }
}
