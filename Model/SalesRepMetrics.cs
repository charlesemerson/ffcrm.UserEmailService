
namespace ffcrm.UserEmailService.Model
{
    public class SalesRepMetrics
    {
        public int SrmId { get; set; }
        public System.DateTime CreatedDateUtc { get; set; }
        public string CountryCode { get; set; }
        public string CurrencyCode { get; set; }
        public string DistrictCode { get; set; }
        public System.DateTime? LastUpdateUtc { get; set; }
        public string RegionCode { get; set; }
        public string SalesRepCode { get; set; }
        public string SalesRepName { get; set; }
        public string StationCode { get; set; }
        public int SubscriberId { get; set; }
        public int UserId { get; set; }
        // Activity Metrics
        public int ActiveAccounts { get; set; }
        public int ActiveLeads { get; set; }
        public int ActiveOpportunities { get; set; }
        public int Calls { get; set; }
        public int Logins { get; set; }
        public int Meetings { get; set; }
        public int Wins { get; set; }
        // Monetary Metrics
        public double AnnualWinsProfitUsd { get; set; }
        public double AnnualWinsRevenueUsd { get; set; }
        public double CurrentMonthActualProfitUsd { get; set; }
        public double CurrentMonthActualRevenueUsd { get; set; }
        public double CurrentYearActualProfitUsd { get; set; }
        public double CurrentYearActualRevenueUsd { get; set; }
        public double InYearActiveOpportunitiesProfitUsd { get; set; }
        public double InYearActiveOpportunitiesRevenueUsd { get; set; }
        public double InYearWinsProfitUsd { get; set; }
        public double InYearWinsRevenueUsd { get; set; }
        public double WeightedAverageWinPercent { get; set; }
    }
}
