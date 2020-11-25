namespace ffcrm.UserEmailService.Model
{
    public class Currency
    {
        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyFormat { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencySymbol { get; set; }
        public System.DateTime? LastUpdate { get; set; }
        public string SubUnitName { get; set; }
        public decimal SubUnitRatio { get; set; }
        public string UnitName { get; set; }
        public int UpdateUserId { get; set; }
        public string UpdateUserName { get; set; }
    }
}
