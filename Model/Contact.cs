using System;

namespace ffcrm.UserEmailService.Model
{
    public class Contact
    {
        // ContactID in CRM
        public int ContactId { get; set; }
        // Sync ContactID
        public string SyncContactId { get; set; }
        // SubscriberId
        public int SubscriberId { get; set; }
        // Birthday
        public int BirthdayDay { get; set; }
        public string BirthdayMonth { get; set; }
        public int BirthdayYear { get; set; }
        // Business Details
        public string BusinessAddress { get; set; }
        public string BusinessCity { get; set; }
        public string BusinessCountry { get; set; }
        public string BusinessPhone { get; set; }
        public string BusinessPhone2 { get; set; }
        public string BusinessState { get; set; }
        public string BusinessZipCode { get; set; }
        // Created Details
        public int CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedUserName { get; set; }
        public DateTime? DateDeleted { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        // Entity Details
        public int EntityId { get; set; }
        public string EntityType { get; set; }
        public int EntityTypeId { get; set; }
        // File Upload Details
        public int FileUploadId { get; set; }
        public DateTime? FileUploadDate { get; set; }
        // Contact Details
        public string ChildrenNames { get; set; }
        public string Comments { get; set; } 
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string ContactNameSource { get; set; }
        public int ContactOwnerId { get; set; }
        public string ContactType { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Fax { get; set; }
        public string Hobbies { get; set; }
        public bool HolidayCards { get; set; }
        public string MobilePhone { get; set; }
        public bool NewsLetter { get; set; }
        public string OriginSystem { get; set; }
        public string RelatedContacts { get; set; }
        public string Salutation { get; set; }
        public string Source { get; set; }
        public string SpouseName { get; set; }
        public string Tags { get; set; }
        public string Title { get; set; }
        public string Website { get; set; }
        // Home Details
        public string HomeAddress { get; set; }
        public string HomeCity { get; set; }
        public string HomeCountry { get; set; }
        public string HomePhone { get; set; }
        public string HomeState { get; set; }
        public string HomeZipCode { get; set; }
        // Update Details
        public DateTime? LastUpdate { get; set; }
        public int UpdateUserId { get; set; }
        public string UpdateUserName { get; set; }
    }
}

