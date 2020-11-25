using System;

namespace ffcrm.UserEmailService.Model
{
    public class SalesRep
    {
        public int SalesRepId { get; set; }
        public int SubscriberId { get; set; }
        public bool Active { get; set; }
        public string CountryCode { get; set; }
        public int CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedUserName { get; set; }
        public DateTime? DateDeleted { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public string DistrictCode { get; set; }
        public DateTime? LastBudgetUpdate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string RegionCode { get; set; }
        public int SalesManagerUserId { get; set; }
        public string SalesGroups { get; set; }
        public string SalesRepCode { get; set; }
        public string SalesRepName { get; set; }
        public string StationCode { get; set; }
        public int UpdateUserId { get; set; }
        public string UpdateUserName { get; set; }
        public int UserId { get; set; }
    }
}
