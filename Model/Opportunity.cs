using System;

namespace ffcrm.UserEmailService.Model
{
    public class OpportunityDetail
    {
        public int OpportunityId { get; set; }
        public string AccountCode { get; set; }
        public int AccountId { get; set; }
        public bool Active { get; set; }
        public double Cbms { get; set; }
        public string Commodity { get; set; }
        public string Competitor { get; set; }
        public int ContactId { get; set; }
        public string ContractLength { get; set; }
        public string ControlledBy { get; set; }
        public int CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedUserName { get; set; }
        public string CurrencyCode { get; set; }
        public decimal CurrencyExchangeRate { get; set; }
        public decimal CurrentRevenue { get; set; }
        public string CustomerReferenceNumber { get; set; }
        public DateTime? DateDeleted { get; set; }
        public DateTime? DateLost { get; set; }
        public DateTime? DateReceived { get; set; }
        public DateTime? DateWon { get; set; }
        public string DecisionMaking { get; set; }
        public string DecisionTimeFrame { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public string DeletedUserName { get; set; }
        public string DestinationPortCodes { get; set; }
        public string Direction { get; set; }
        public DateTime? DueDate { get; set; }
        public int EntityId { get; set; }
        public string EntityName { get; set; }
        public string EntityType { get; set; }
        public int EntityTypeId { get; set; }
        public decimal EstimatedProfit { get; set; }
        public DateTime? EstimatedStartDate { get; set; }
        public DateTime? ExpectedDecisionDate { get; set; }
        public double Feus { get; set; }
        public decimal ForecastPercentage { get; set; }
        public decimal GrossProfitPercent { get; set; }
        public string IndustrySector { get; set; }
        public bool InterOffice { get; set; }
        public string InterOfficeStationCode { get; set; }
        public double KGs { get; set; }
        public DateTime? LastUpdate { get; set; }
        public int LeadId { get; set; }
        public bool Lost { get; set; }
        public string NextStep { get; set; }
        public string OpportunityDescription { get; set; }
        public string OpportunityName { get; set; }
        public string OpportunityNumber { get; set; }
        public int OpportunityOwnerId { get; set; }
        public string OpportunityType { get; set; }
        public string OriginPortCodes { get; set; }
        public string PrimaryClientContact { get; set; }
        public int PrimaryContactId { get; set; }
        public string ReasonWonLost { get; set; }
        public string Region { get; set; }
        public string RequestType { get; set; }
        public decimal Revenue { get; set; }
        public decimal RevenueBase { get; set; }
        public string RevenueRange { get; set; }
        public string SalesStage { get; set; }
        public string Services { get; set; }
        public string ShippingFrequency { get; set; }
        public string Status { get; set; }
        public string Tags { get; set; }
        public string TerminalCode { get; set; }
        public double Teus { get; set; }
        public string TradeLanes { get; set; }
        public int UpdateUserId { get; set; }
        public string UpdateUserName { get; set; }
        public bool Won { get; set; }
        public decimal WonExchangeRate { get; set; }
        public DateTime? NextActivityDate { get; set; }
        public string CompanyName { get; set; }
    }
}
