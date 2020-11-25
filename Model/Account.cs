using System;

namespace ffcrm.UserEmailService.Model
{
    public class Account
    {
        public int AccountId { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public bool Active { get; set; }
        public string Address { get; set; }
        public int AnnualRevenue { get; set; }
        public bool BusinessLost { get; set; }
        public DateTime? BusinessLostDate { get; set; }
        public bool BusinessReAcquired { get; set; }
        public DateTime? BusinessReAcquiredDate { get; set; }
        public string City { get; set; }
        public int CollectorId { get; set; }
        public string CollectorName { get; set; }
        public string Comments { get; set; }
        public string ControllingRepCode { get; set; }
        public string ControllingSalesRep { get; set; }
        public string Country { get; set; }
        public int CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedUserName { get; set; }
        public double CreditLimit { get; set; }
        public DateTime? DateDeleted { get; set; }
        public int DaysNoShipments { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public string DunsNumber { get; set; }
        public string Fax { get; set; }
        public DateTime? FirstShipmentDate { get; set; }
        public string HooversId { get; set; }
        public string IndustrySector { get; set; }
        public bool IsCustomer { get; set; }
        public DateTime? LastCreditUpdate { get; set; }
        public DateTime? LastShipmentDate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string MaintenanceRepCode { get; set; }
        public string MaintenanceSalesRep { get; set; }
        public int NewBusinessDays { get; set; }
        public DateTime? NewBusinessEndDate = null;
        public int NumberOfEmployees { get; set; }
        public int OriginatingUserId { get; set; }
        public string OriginatingUserName { get; set; }
        public string OriginSystem { get; set; }
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        public bool PrimaryAccount { get; set; }
        public int PrimaryAddressId { get; set; }
        public int RelatedLeadId { get; set; }
        public string SalesGroups { get; set; }
        public string SegmentCode { get; set; }
        public string ServiceRepCode { get; set; }
        public string ServiceRepName { get; set; }
        public string SicCode1 { get; set; }
        public string SicCode2 { get; set; }
        public string Source { get; set; }
        public int SrAssignedById { get; set; }
        public string SrAssignedByName { get; set; }
        public DateTime? SrAssignedDate { get; set; }
        public bool SrViewedAssignment { get; set; }
        public DateTime? SrViewedAssignedDate { get; set; }
        public string State { get; set; }
        public int SubscriberId { get; set; }
        public string Tags { get; set; }
        public int UpdateUserId { get; set; }
        public string UpdateUserName { get; set; }
        public string Website { get; set; }
        public string ZipCode { get; set; }
        public string ControllingSalesRepName { get; set; }
        public string MaintenanceSalesRepName { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public DateTime? NextActivityDate { get; set; }
        public int RevenueYtd { get; set; }
        public int RevenueLastPeriod { get; set; }
        public int FileUploadId { get; set; }
        public DateTime? FileUploadDate { get; set; }
        public string OriginSystemSalesRepCode { get; set; }
        public bool BusinessInJeopardy { get; set; }
        public DateTime? BusinessInJeopardyDate { get; set; }
        public string BusinessInJeopardyReason { get; set; }
        public string BusinessJeopardyActionPlan { get; set; }
        public string BusinessLostReason { get; set; }
        public string BusinessReAcquiredReason { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public int PrimaryContactId { get; set; }
    }
}