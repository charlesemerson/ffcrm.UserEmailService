using ffcrm.UserEmailService.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ffcrm.UserEmailService.Helper
{
    public class Metrics
    {

        //#region * Process SalesRepMetrics *

        public void UpdateSalesRepMetricsForAllActiveSalesReps(DateTime? startDate = null)
        {
            // ======================================================================================
            // MAIN CALLING ROUTINE
            // ======================================================================================
            // Creates or Updates 1 record for each SalesRep for each Day in tblSalesRepMetrics
            // ======================================================================================
            // Admin Session Details for Logging
            var session = new Session { UserId = 999, SubscriberId = 999 };
            // Iterate through all Active SalesReps

            var context = new DbFirstFreightDataContext(new Utils().GetConnectionForDataCenter());

            var salesReps =
                            (from sr in context.Users
                             join sb in context.Subscribers on sr.SubscriberId equals sb.SubscriberId
                             where sb.Active && !sr.Deleted && sb.SubscriberId == 223 //Apex for testing
                             orderby sr.UserId
                             select sr);

            if (salesReps.Any())
            {
                foreach (var user in salesReps)
                {
                    // Set SalesRep Variables
                    var salesRepCode = Util.NullToEmpty(user.SalesRepCode);
                    var salesRepName = Util.NullToEmpty(user.FullName);
                    var subscriberId = Util.NullToZero(user.SubscriberId);
                    var userId = Util.NullToZero(user.UserId);
                    // Process SalesRepMetrics for SalesRep
                    ProcessMetricsForSalesRep(subscriberId, userId, salesRepCode, salesRepName, session, startDate);
                }
            }
        }

        public int ProcessMetricsForSalesRep(int subscriberId, int userId, string salesRepCode, string salesRepName, Session session, DateTime? startDate = null)
        {
            var records = startDate != null ? ProcessSalesRepMetricsFromStartDate(subscriberId, userId, salesRepCode, salesRepName, startDate.Value, session) : ProcessSalesRepMetricsForDate(subscriberId, userId, salesRepCode, salesRepName, DateTime.UtcNow, session);
            return records;
        }

        public int ProcessSalesRepMetricsForDate(int subscriberId, int userId, string salesRepCode, string salesRepName, DateTime metricsDate, Session session)
        {
            throw new NotImplementedException();
            //var salesRepMetrics = GetSalesRepMetrics(subscriberId, userId, salesRepCode, salesRepName, metricsDate);
            //var recordexists = SalesRepMetricsRecordExists(subscriberId, userId, metricsDate, session);
            //var recordsProcessed = recordexists ? UpdateSalesRepMetrics(salesRepMetrics, session) : AddSalesRepMetrics(salesRepMetrics, session);
            //return recordsProcessed;
        }

        public int ProcessSalesRepMetricsFromStartDate(int subscriberId, int userId, string salesRepCode, string salesRepName, DateTime startDate, Session session)
        {
            var recordsProcessed = 0;
            var metricsDate = Convert.ToDateTime(startDate.ToShortDateString() + "00:00:01");
            var endDate = Convert.ToDateTime(DateTime.UtcNow.ToShortDateString() + "23:59:59");
            // Process All Days between startDate and endDate
            do
            {
                recordsProcessed += ProcessSalesRepMetricsForDate(subscriberId, userId, salesRepCode, salesRepName, metricsDate, session);
                metricsDate = metricsDate.AddDays(1);
            } while (metricsDate <= endDate);
            return recordsProcessed;
        }

        ////public bool SalesRepMetricsRecordExists(int subscriberId, int userId, DateTime metricsDate, Session session)
        ////{
        ////    var bln = false;
        ////    var startDateTime = metricsDate.ToShortDateString() + " 00:00:01";
        ////    var endDateTime = metricsDate.ToShortDateString() + " 23:59:59";
        ////    var sql = "SELECT SrmId FROM tblSalesRepMetrics ";
        ////    sql += "WHERE (SubscriberID = " + subscriberId + ") ";
        ////    sql += "AND (UserId = " + userId + ") ";
        ////    sql += "AND (CreatedDateUtc BETWEEN '" + startDateTime + "' AND '" + endDateTime + "') ";
        ////    var ds = new DbHelper().GetDataSet(sql, session);
        ////    var dt = ds.Tables[0];
        ////    if (dt.Rows.Count > 0)
        ////    {
        ////        bln = true;
        ////    }
        ////    dt.Dispose();
        ////    ds.Dispose();
        ////    return bln;
        ////}

        ////public SalesRepMetrics GetSalesRepMetrics(int subscriberId, int userId, string salesRepCode, string salesRepName, DateTime metricsDate)
        ////{
        ////    //TODO: Try Catch
        ////    var session = new Session { SubscriberId = 999, UserId = 9999 };
        ////    var srm = new SalesReps().GetSalesRepMetricsDetailsFromUserIdSubscriberId(userId, subscriberId, session);
        ////    var metricsShortDate = metricsDate.ToShortDateString();
        ////    var startDateTime = Convert.ToDateTime(metricsShortDate + " 00:00:01");
        ////    var endDateTime = Convert.ToDateTime(metricsShortDate + " 23:59:59");
        ////    var salesRepMetrics = new SalesRepMetrics
        ////    {
        ////        // Sales Rep Info
        ////        CountryCode = srm.CountryCode,
        ////        DistrictCode = srm.DistrictCode,
        ////        RegionCode = srm.RegionCode,
        ////        SalesRepCode = salesRepCode,
        ////        SalesRepName = salesRepName,
        ////        StationCode = srm.StationCode,
        ////        SubscriberId = subscriberId,
        ////        UserId = userId,
        ////        // Activity Metrics
        ////        ActiveAccounts = AccountsActiveCount(userId, subscriberId, session),
        ////        ActiveLeads = LeadsActiveCount(userId, subscriberId, session),
        ////        ActiveOpportunities = OpportunitiesActiveCount(userId, subscriberId, session),
        ////        Calls = GetNumberOfScheduledCalls(session, startDateTime, endDateTime, subscriberId, userId),
        ////        Logins = UserLoginsCount(userId, subscriberId, startDateTime, endDateTime, session),
        ////        Meetings = GetNumberOfMeetings(session, startDateTime, endDateTime, userId),
        ////        Wins = OpportunitiesWonCount(userId, subscriberId, startDateTime, endDateTime, session),
        ////        // Monetary Metrics
        ////        AnnualWinsProfitUsd = OpportunitiesAnnualWonProfit(userId, subscriberId, startDateTime, endDateTime, "USD", session),
        ////        AnnualWinsRevenueUsd = OpportunitiesAnnualWonRevenue(userId, subscriberId, startDateTime, endDateTime, "USD", session),
        ////        CurrentMonthActualProfitUsd = GetMonthlyActualProfit(userId, endDateTime, "USD", session),
        ////        CurrentMonthActualRevenueUsd = GetMonthlyActualRevenue(userId, endDateTime, "USD", session),
        ////        CurrentYearActualProfitUsd = GetYearlyActualProfit(userId, endDateTime, "USD", session),
        ////        CurrentYearActualRevenueUsd = GetYearlyActualRevenue(userId, endDateTime, "USD", session),
        ////        // Probably Need to Calculate These On-Demand
        ////        //InYearActiveOpportunitiesProfitUsd = OpportunitiesInYearProfit(userId, startDateTime, endDateTime, "USD", opportunityType, session, opportunityId),
        ////        //InYearActiveOpportunitiesRevenueUsd = OpportunitiesInYearRevenue(userId, startDateTime, endDateTime, "USD", opportunityType, session, opportunityId),
        ////        //InYearWinsProfitUsd = OpportunitiesInYearProfit(userId, startDateTime, endDateTime, "USD", opportunityType, session, opportunityId),
        ////        //InYearWinsRevenueUsd = OpportunitiesInYearRevenue(userId, startDateTime, endDateTime, "USD", opportunityType, session, opportunityId),
        ////        //WeightedAverageWinPercent = mtx
        ////    };
        ////    return salesRepMetrics;
        ////}

        ///// <summary>
        ///// This function will update metrics for sent reminders within last month
        ///// </summary>
        ////public void UpdateMetricsForReminders(string mandrilApiKey)
        ////{
        ////    // Admin Session Details for Logging
        ////    var session = new Session { UserId = 999, SubscriberId = 999 };
        ////    var sql = "SELECT ID,MandrilEmailId ";
        ////    sql += "FROM tblEmailSent ";
        ////    sql += "WHERE DateSent >= DATEADD(MONTH,-1, GETDATE()) ";
        ////    var ds = new DbHelper().GetDataSet(sql, session);
        ////    var dt = ds.Tables[0];
        ////    if (dt.Rows.Count <= 0) return;
        ////    for (var i = 0; i <= dt.Rows.Count - 1; i++)
        ////    {
        ////        var mandrillEmailId = Util.NullToEmpty(dt.Rows[i]["MandrilEmailId"]);
        ////        if (string.IsNullOrEmpty(mandrillEmailId)) continue;
        ////        var mandrillEmail = new Messages().GetMessageInfo(mandrillEmailId, mandrilApiKey);
        ////        if (mandrillEmail != null)
        ////        {
        ////            UpdateReminderMetrics(mandrillEmail, session);
        ////        }
        ////    }
        ////}

        //#endregion

        //#region * Add | Update SalesRepMetrics *

        ///// <summary>
        ///// Add SalesRepMetrics Record
        ///// </summary>
        ///// <param name="salesRepMetrics"></param>
        ///// <param name="session"></param>
        ////public int AddSalesRepMetrics(SalesRepMetrics salesRepMetrics, Session session)
        ////{
        ////    if (session == null)
        ////    {
        ////        // No Current Session
        ////        return 0;
        ////    }
        ////    // Set SQL Parameters
        ////    var parameters = new List<Parameter>();
        ////    // CountryCode
        ////    var param = new Parameter { Name = "@CountryCode", SqlType = SqlDbType.NVarChar, Size = 5, Value = Util.NullToEmpty(salesRepMetrics.CountryCode) };
        ////    parameters.Add(param);
        ////    // CurrencyCode
        ////    param = new Parameter { Name = "@CurrencyCode", SqlType = SqlDbType.NVarChar, Size = 5, Value = Util.NullToEmpty(salesRepMetrics.CurrencyCode) };
        ////    parameters.Add(param);
        ////    // DistrictCode
        ////    param = new Parameter { Name = "@DistrictCode", SqlType = SqlDbType.NVarChar, Size = 10, Value = Util.NullToEmpty(salesRepMetrics.DistrictCode) };
        ////    parameters.Add(param);
        ////    // RegionCode
        ////    param = new Parameter { Name = "@RegionCode", SqlType = SqlDbType.NVarChar, Size = 10, Value = Util.NullToEmpty(salesRepMetrics.RegionCode) };
        ////    parameters.Add(param);
        ////    // SalesRepCode
        ////    param = new Parameter { Name = "@SalesRepCode", SqlType = SqlDbType.NVarChar, Size = 10, Value = Util.NullToEmpty(salesRepMetrics.SalesRepCode) };
        ////    parameters.Add(param);
        ////    // SalesRepName
        ////    param = new Parameter { Name = "@SalesRepName", SqlType = SqlDbType.NVarChar, Size = 50, Value = Util.NullToEmpty(salesRepMetrics.SalesRepName) };
        ////    parameters.Add(param);
        ////    // StationCode
        ////    param = new Parameter { Name = "@StationCode", SqlType = SqlDbType.NVarChar, Size = 10, Value = Util.NullToEmpty(salesRepMetrics.StationCode) };
        ////    parameters.Add(param);
        ////    // SubscriberId
        ////    param = new Parameter { Name = "@SubscriberId", SqlType = SqlDbType.Int, Size = 4, Value = Util.NullToZero(salesRepMetrics.SubscriberId) };
        ////    parameters.Add(param);
        ////    // UserId
        ////    param = new Parameter { Name = "@UserId", SqlType = SqlDbType.Int, Size = 4, Value = Util.NullToZero(salesRepMetrics.UserId) };
        ////    parameters.Add(param);
        ////    // =========================================================================================
        ////    // Activity Metrics
        ////    // =========================================================================================
        ////    // ActiveAccounts
        ////    param = new Parameter { Name = "@ActiveAccounts", SqlType = SqlDbType.Int, Size = 4, Value = Util.NullToZero(salesRepMetrics.ActiveAccounts) };
        ////    parameters.Add(param);
        ////    // ActiveLeads
        ////    param = new Parameter { Name = "@ActiveLeads", SqlType = SqlDbType.Int, Size = 4, Value = Util.NullToZero(salesRepMetrics.ActiveLeads) };
        ////    parameters.Add(param);
        ////    // ActiveOpportunities
        ////    param = new Parameter { Name = "@ActiveOpportunities", SqlType = SqlDbType.Int, Size = 4, Value = Util.NullToZero(salesRepMetrics.ActiveOpportunities) };
        ////    parameters.Add(param);
        ////    // Calls
        ////    param = new Parameter { Name = "@Calls", SqlType = SqlDbType.Int, Size = 4, Value = Util.NullToZero(salesRepMetrics.Calls) };
        ////    parameters.Add(param);
        ////    // Logins
        ////    param = new Parameter { Name = "@Logins", SqlType = SqlDbType.Int, Size = 4, Value = Util.NullToZero(salesRepMetrics.Logins) };
        ////    parameters.Add(param);
        ////    // Meetings
        ////    param = new Parameter { Name = "@Meetings", SqlType = SqlDbType.Int, Size = 4, Value = Util.NullToZero(salesRepMetrics.Meetings) };
        ////    parameters.Add(param);
        ////    // Wins
        ////    param = new Parameter { Name = "@Wins", SqlType = SqlDbType.Int, Size = 4, Value = Util.NullToZero(salesRepMetrics.Wins) };
        ////    parameters.Add(param);
        ////    // =========================================================================================
        ////    // Monetary Metrics
        ////    // =========================================================================================
        ////    // AnnualWinsProfitUsd
        ////    param = new Parameter { Name = "@AnnualWinsProfitUsd", SqlType = SqlDbType.Float, Size = 8, Value = Util.NullToZero(salesRepMetrics.AnnualWinsProfitUsd) };
        ////    parameters.Add(param);
        ////    // AnnualWinsRevenueUsd
        ////    param = new Parameter { Name = "@AnnualWinsRevenueUsd", SqlType = SqlDbType.Float, Size = 8, Value = Util.NullToZero(salesRepMetrics.AnnualWinsRevenueUsd) };
        ////    parameters.Add(param);
        ////    // CurrentMonthActualProfitUsd
        ////    param = new Parameter { Name = "@CurrentMonthActualProfitUsd", SqlType = SqlDbType.Float, Size = 8, Value = Util.NullToZero(salesRepMetrics.CurrentMonthActualProfitUsd) };
        ////    parameters.Add(param);
        ////    // CurrentMonthActualRevenueUsd
        ////    param = new Parameter { Name = "@CurrentMonthActualRevenueUsd", SqlType = SqlDbType.Float, Size = 8, Value = Util.NullToZero(salesRepMetrics.CurrentMonthActualRevenueUsd) };
        ////    parameters.Add(param);
        ////    // CurrentYearActualProfitUsd
        ////    param = new Parameter { Name = "@CurrentYearActualProfitUsd", SqlType = SqlDbType.Float, Size = 8, Value = Util.NullToZero(salesRepMetrics.CurrentYearActualProfitUsd) };
        ////    parameters.Add(param);
        ////    // CurrentYearActualRevenueUsd
        ////    param = new Parameter { Name = "@CurrentYearActualRevenueUsd", SqlType = SqlDbType.Float, Size = 8, Value = Util.NullToZero(salesRepMetrics.CurrentYearActualRevenueUsd) };
        ////    parameters.Add(param);
        ////    // InYearActiveOpportunitiesProfitUsd
        ////    param = new Parameter { Name = "@InYearActiveOpportunitiesProfitUsd", SqlType = SqlDbType.Float, Size = 8, Value = Util.NullToZero(salesRepMetrics.InYearActiveOpportunitiesProfitUsd) };
        ////    parameters.Add(param);
        ////    // InYearActiveOpportunitiesRevenueUsd
        ////    param = new Parameter { Name = "@InYearActiveOpportunitiesRevenueUsd", SqlType = SqlDbType.Float, Size = 8, Value = Util.NullToZero(salesRepMetrics.InYearActiveOpportunitiesRevenueUsd) };
        ////    parameters.Add(param);
        ////    // InYearWinsProfitUsd
        ////    param = new Parameter { Name = "@InYearWinsProfitUsd", SqlType = SqlDbType.Float, Size = 8, Value = Util.NullToZero(salesRepMetrics.InYearWinsProfitUsd) };
        ////    parameters.Add(param);
        ////    // InYearWinsRevenueUsd
        ////    param = new Parameter { Name = "@InYearWinsRevenueUsd", SqlType = SqlDbType.Float, Size = 8, Value = Util.NullToZero(salesRepMetrics.InYearWinsRevenueUsd) };
        ////    parameters.Add(param);
        ////    // WeightedAverageWinPercent
        ////    param = new Parameter { Name = "@WeightedAverageWinPercent", SqlType = SqlDbType.Float, Size = 8, Value = Util.NullToZero(salesRepMetrics.WeightedAverageWinPercent) };
        ////    parameters.Add(param);
        ////    // SQL Statement
        ////    var sql = "INSERT INTO tblSalesRepMetrics (";
        ////    sql += "CountryCode, CreatedDateUtc, CurrencyCode, DistrictCode, LastUpdateUtc, ";
        ////    sql += "RegionCode, SalesRepCode, SalesRepName, StationCode, SubscriberId, UserId, ";
        ////    // Activity Metrics
        ////    sql += "ActiveAccounts, ActiveLeads, ActiveOpportunities,  Calls, ";
        ////    sql += "Logins, Meetings, Wins, ";
        ////    // Monetary Metrics
        ////    sql += "AnnualWinsProfitUsd, AnnualWinsRevenueUsd, ";
        ////    sql += "CurrentMonthActualProfitUsd, CurrentMonthActualRevenueUsd, ";
        ////    sql += "CurrentYearActualProfitUsd, CurrentYearActualRevenueUsd, ";
        ////    sql += "InYearActiveOpportunitiesProfitUsd, InYearActiveOpportunitiesRevenueUsd, ";
        ////    sql += "InYearWinsProfitUsd, InYearWinsRevenueUsd, WeightedAverageWinPercent";
        ////    sql += ") VALUES ( ";
        ////    sql += "@CountryCode, '" + DateTime.Now + "', @CurrencyCode, @DistrictCode, '" + DateTime.Now + "', ";
        ////    sql += "@RegionCode, @SalesRepCode, @SalesRepName, @StationCode, @SubscriberId, @UserId, ";
        ////    // Activity Metrics
        ////    sql += "@ActiveAccounts, @ActiveLeads, @ActiveOpportunities,  @Calls, ";
        ////    sql += "@Logins, @Meetings, @Wins, ";
        ////    // Monetary Metrics
        ////    sql += "@AnnualWinsProfitUsd, @AnnualWinsRevenueUsd, ";
        ////    sql += "@CurrentMonthActualProfitUsd, @CurrentMonthActualProfitUsd, ";
        ////    sql += "@CurrentYearActualProfitUsd, @CurrentYearActualRevenueUsd, ";
        ////    sql += "@InYearActiveOpportunitiesProfitUsd, @InYearActiveOpportunitiesRevenueUsd, ";
        ////    sql += "@InYearWinsProfitUsd, @InYearWinsRevenueUsd, @WeightedAverageWinPercent";
        ////    sql += ");";
        ////    // ===================================================================
        ////    var srmId = new DbHelper().AddRecordWithParamsGetId(sql, parameters, session);
        ////    if (srmId == 0)
        ////    {
        ////        // 1000 Errors are for Sales Rep Metrics
        ////        new Log().LogError("Error", 1001, "Could Not Add CRM SalesRepMetrics", "AddSalesRepMetrics", session);
        ////    }
        ////    return srmId;
        ////}

        ///// <summary>
        ///// Update SalesRepMetrics Record
        ///// </summary>
        ///// <param name="salesRepMetrics"></param>
        ///// <param name="session"></param>
        ////public int UpdateSalesRepMetrics(SalesRepMetrics salesRepMetrics, Session session)
        ////{
        ////    if (session == null)
        ////    {
        ////        // No Current Session
        ////        return 0;
        ////    }
        ////    // Set SQL Parameters
        ////    var parameters = new List<Parameter>();
        ////    // =========================================================================================
        ////    // Activity Metrics
        ////    // =========================================================================================
        ////    // ActiveAccounts
        ////    var param = new Parameter { Name = "@ActiveAccounts", SqlType = SqlDbType.Int, Size = 4, Value = Util.NullToZero(salesRepMetrics.ActiveAccounts) };
        ////    parameters.Add(param);
        ////    // ActiveLeads
        ////    param = new Parameter { Name = "@ActiveLeads", SqlType = SqlDbType.Int, Size = 4, Value = Util.NullToZero(salesRepMetrics.ActiveLeads) };
        ////    parameters.Add(param);
        ////    // ActiveOpportunities
        ////    param = new Parameter { Name = "@ActiveOpportunities", SqlType = SqlDbType.Int, Size = 4, Value = Util.NullToZero(salesRepMetrics.ActiveOpportunities) };
        ////    parameters.Add(param);
        ////    // Calls
        ////    param = new Parameter { Name = "@Calls", SqlType = SqlDbType.Int, Size = 4, Value = Util.NullToZero(salesRepMetrics.Calls) };
        ////    parameters.Add(param);
        ////    // Logins
        ////    param = new Parameter { Name = "@Logins", SqlType = SqlDbType.Int, Size = 4, Value = Util.NullToZero(salesRepMetrics.Logins) };
        ////    parameters.Add(param);
        ////    // Meetings
        ////    param = new Parameter { Name = "@Meetings", SqlType = SqlDbType.Int, Size = 4, Value = Util.NullToZero(salesRepMetrics.Meetings) };
        ////    parameters.Add(param);
        ////    // Wins
        ////    param = new Parameter { Name = "@Wins", SqlType = SqlDbType.Int, Size = 4, Value = Util.NullToZero(salesRepMetrics.Wins) };
        ////    parameters.Add(param);
        ////    // =========================================================================================
        ////    // Monetary Metrics
        ////    // =========================================================================================
        ////    // AnnualWinsProfitUsd
        ////    param = new Parameter { Name = "@AnnualWinsProfitUsd", SqlType = SqlDbType.Float, Size = 8, Value = Util.NullToZero(salesRepMetrics.AnnualWinsProfitUsd) };
        ////    parameters.Add(param);
        ////    // AnnualWinsRevenueUsd
        ////    param = new Parameter { Name = "@AnnualWinsRevenueUsd", SqlType = SqlDbType.Float, Size = 8, Value = Util.NullToZero(salesRepMetrics.AnnualWinsRevenueUsd) };
        ////    parameters.Add(param);
        ////    // CurrentMonthActualProfitUsd
        ////    param = new Parameter { Name = "@CurrentMonthActualProfitUsd", SqlType = SqlDbType.Float, Size = 8, Value = Util.NullToZero(salesRepMetrics.CurrentMonthActualProfitUsd) };
        ////    parameters.Add(param);
        ////    // CurrentMonthActualRevenueUsd
        ////    param = new Parameter { Name = "@CurrentMonthActualRevenueUsd", SqlType = SqlDbType.Float, Size = 8, Value = Util.NullToZero(salesRepMetrics.CurrentMonthActualRevenueUsd) };
        ////    parameters.Add(param);
        ////    // CurrentYearActualProfitUsd
        ////    param = new Parameter { Name = "@CurrentYearActualProfitUsd", SqlType = SqlDbType.Float, Size = 8, Value = Util.NullToZero(salesRepMetrics.CurrentYearActualProfitUsd) };
        ////    parameters.Add(param);
        ////    // CurrentYearActualRevenueUsd
        ////    param = new Parameter { Name = "@CurrentYearActualRevenueUsd", SqlType = SqlDbType.Float, Size = 8, Value = Util.NullToZero(salesRepMetrics.CurrentYearActualRevenueUsd) };
        ////    parameters.Add(param);
        ////    // InYearActiveOpportunitiesProfitUsd
        ////    param = new Parameter { Name = "@InYearActiveOpportunitiesProfitUsd", SqlType = SqlDbType.Float, Size = 8, Value = Util.NullToZero(salesRepMetrics.InYearActiveOpportunitiesProfitUsd) };
        ////    parameters.Add(param);
        ////    // InYearActiveOpportunitiesRevenueUsd
        ////    param = new Parameter { Name = "@InYearActiveOpportunitiesRevenueUsd", SqlType = SqlDbType.Float, Size = 8, Value = Util.NullToZero(salesRepMetrics.InYearActiveOpportunitiesRevenueUsd) };
        ////    parameters.Add(param);
        ////    // InYearWinsProfitUsd
        ////    param = new Parameter { Name = "@InYearWinsProfitUsd", SqlType = SqlDbType.Float, Size = 8, Value = Util.NullToZero(salesRepMetrics.InYearWinsProfitUsd) };
        ////    parameters.Add(param);
        ////    // InYearWinsRevenueUsd
        ////    param = new Parameter { Name = "@InYearWinsRevenueUsd", SqlType = SqlDbType.Float, Size = 8, Value = Util.NullToZero(salesRepMetrics.InYearWinsRevenueUsd) };
        ////    parameters.Add(param);
        ////    // WeightedAverageWinPercent
        ////    param = new Parameter { Name = "@WeightedAverageWinPercent", SqlType = SqlDbType.Float, Size = 8, Value = Util.NullToZero(salesRepMetrics.WeightedAverageWinPercent) };
        ////    parameters.Add(param);
        ////    // SQL Statement
        ////    var sql = "UPDATE tblSalesRepMetrics SET ";
        ////    sql += "LastUpdateUtc = '" + DateTime.Now + "', ";
        ////    // Activity Metrics
        ////    sql += "ActiveAccounts = @ActiveAccounts, ";
        ////    sql += "ActiveLeads = @ActiveLeads, ";
        ////    sql += "ActiveOpportunities = @ActiveOpportunities, ";
        ////    sql += "Calls = @Calls, ";
        ////    sql += "Logins = @Logins, ";
        ////    sql += "Meetings = @Meetings, ";
        ////    sql += "Wins = @Wins, ";
        ////    // Monetary Metrics
        ////    sql += "AnnualWinsProfitUsd = @AnnualWinsProfitUsd, ";
        ////    sql += "AnnualWinsRevenueUsd = @AnnualWinsRevenueUsd, ";
        ////    sql += "CurrentMonthActualProfitUsd = @CurrentMonthActualProfitUsd, ";
        ////    sql += "CurrentMonthActualRevenueUsd = @CurrentMonthActualRevenueUsd, ";
        ////    sql += "CurrentYearActualProfitUsd = @CurrentYearActualProfitUsd, ";
        ////    sql += "CurrentYearActualRevenueUsd = @CurrentYearActualRevenueUsd, ";
        ////    sql += "InYearActiveOpportunitiesProfitUsd = @InYearActiveOpportunitiesProfitUsd, ";
        ////    sql += "InYearActiveOpportunitiesRevenueUsd = @InYearActiveOpportunitiesRevenueUsd, ";
        ////    sql += "InYearWinsProfitUsd = @InYearWinsProfitUsd, ";
        ////    sql += "InYearWinsRevenueUsd = @InYearWinsRevenueUsd, ";
        ////    sql += "WeightedAverageWinPercent = @WeightedAverageWinPercent ";
        ////    // ===================================================================
        ////    var records = new DbHelper().UpdateRecordWithParams(sql, parameters, session);
        ////    if (records == 0)
        ////    {
        ////        // 1000 Errors are for Sales Rep Metrics
        ////        new Log().LogError("Error", 1002, "Could Not Update CRM SalesRepMetrics", "UpdateSalesRepMetrics", session);
        ////    }
        ////    return records;
        ////}

        //#endregion

        //#region * Accounts *

        ////public int AccountsActiveCount(int userId, int subscriberId, Session session)
        ////{
        ////    var activeAccounts = 0;
        ////    var salesRepCode = new SalesReps().GetSalesRepCodeFromUserId(userId, session);
        ////    if (session.SubscriberId <= 0) return activeAccounts;
        ////    var sql = "SELECT Count(ID) AS ActiveAccounts ";
        ////    sql += "FROM tblAccounts ";
        ////    sql += "WHERE (SubscriberID = " + subscriberId + ") ";
        ////    sql += "AND (ControllingSalesRep = '" + salesRepCode + "') ";
        ////    sql += "AND (Deleted = 0) ";
        ////    sql += "AND (Active = 1) ";
        ////    var ds = new DbHelper().GetDataSet(sql, session);
        ////    var dt = ds.Tables[0];
        ////    if (dt.Rows.Count > 0)
        ////    {
        ////        activeAccounts = Util.NullToZero(dt.Rows[0]["ActiveAccounts"]);
        ////    }
        ////    dt.Dispose();
        ////    ds.Dispose();
        ////    return activeAccounts;
        ////}

        //#endregion

        //#region * Activities *

        //public int GetNumberOfScheduledCalls(Session session, DateTime startDate, DateTime endDate, int subscriberId, int userId = 0)
        //{
        //    var callsCount = 0;
        //    var validDates = Util.ValidStartEndDates(startDate, endDate);
        //    if (!validDates || (subscriberId <= 0)) return callsCount;
        //    var sql = "SELECT Count(ID) AS CallsCount ";
        //    sql += "FROM tblActivities ";
        //    sql += "WHERE (SubscriberID = " + subscriberId + ") ";
        //    sql += "AND (ActivityTypeCode = 'C') ";
        //    sql += "AND (CreatedDate BETWEEN '" + startDate.ToString("yyyy-MM-dd") + "' AND '" + endDate.ToString("yyyy-MM-dd") + "') ";
        //    // If UserId Passed
        //    if (session.UserId > 0)
        //    {
        //        sql += "AND (CreatedByID = " + userId + ") ";
        //    }
        //    var ds = new DbHelper().GetDataSet(sql, session);
        //    var dt = ds.Tables[0];
        //    if (dt.Rows.Count > 0)
        //    {
        //        callsCount = int.Parse(dt.Rows[0]["CallsCount"].ToString());
        //    }
        //    dt.Dispose();
        //    ds.Dispose();
        //    return callsCount;
        //}

        //public int GetNumberOfLoggedCalls(Session session, DateTime startDate, DateTime endDate, int subscriberId, int userId = 0)
        //{
        //    var logsCount = 0;
        //    var validDates = Util.ValidStartEndDates(startDate, endDate);
        //    if (!validDates || (subscriberId <= 0)) return logsCount;
        //    var sql = "SELECT Count(ID) AS LogsCount ";
        //    sql += "FROM tblActivities ";
        //    sql += "WHERE (SubscriberID = " + subscriberId + ") ";
        //    sql += "AND (ActivityTypeCode = 'L') ";
        //    sql += "AND (CreatedDate BETWEEN '" + startDate.ToString("yyyy-MM-dd") + "' AND '" + endDate.ToString("yyyy-MM-dd") + "') ";
        //    // If UserId Passed
        //    if (session.UserId > 0)
        //    {
        //        sql += "AND (CreatedByID = " + userId + ") ";
        //    }
        //    var ds = new DbHelper().GetDataSet(sql, session);
        //    var dt = ds.Tables[0];
        //    if (dt.Rows.Count > 0)
        //    {
        //        logsCount = int.Parse(dt.Rows[0]["LogsCount"].ToString());
        //    }
        //    dt.Dispose();
        //    ds.Dispose();
        //    return logsCount;
        //}

        //public int GetNumberOfMeetings(Session session, DateTime startDate, DateTime endDate, int subscriberId, int userId = 0)
        //{
        //    var meetingsCount = 0;
        //    var validDates = Util.ValidStartEndDates(startDate, endDate);
        //    if (!validDates || (subscriberId <= 0)) return meetingsCount;
        //    var sql = "SELECT Count(ID) AS MeetingsCount ";
        //    sql += "FROM tblActivities ";
        //    sql += "WHERE (SubscriberID = " + subscriberId + ") ";
        //    sql += "AND (ActivityTypeCode = 'M') ";
        //    sql += "AND (CreatedDate BETWEEN '" + startDate.ToString("yyyy-MM-dd") + "' AND '" + endDate.ToString("yyyy-MM-dd") + "') ";
        //    // If UserId Passed
        //    if (session.UserId > 0)
        //    {
        //        sql += "AND (CreatedByID = " + userId + ") ";
        //    }
        //    var ds = new DbHelper().GetDataSet(sql, session);
        //    var dt = ds.Tables[0];
        //    if (dt.Rows.Count > 0)
        //    {
        //        meetingsCount = int.Parse(dt.Rows[0]["MeetingsCount"].ToString());
        //    }
        //    dt.Dispose();
        //    ds.Dispose();
        //    return meetingsCount;
        //}

        //public int GetNumberOfNotes(Session session, DateTime startDate, DateTime endDate, int subscriberId, int userId = 0)
        //{
        //    var notesCount = 0;
        //    var validDates = Util.ValidStartEndDates(startDate, endDate);
        //    if (!validDates || (subscriberId <= 0)) return notesCount;
        //    var sql = "SELECT Count(ID) AS NotesCount ";
        //    sql += "FROM tblActivities ";
        //    sql += "WHERE (SubscriberID = " + subscriberId + ") ";
        //    sql += "AND (ActivityTypeCode = 'N') ";
        //    sql += "AND (CreatedDate BETWEEN '" + startDate.ToString("yyyy-MM-dd") + "' AND '" + endDate.ToString("yyyy-MM-dd") + "') ";
        //    // If UserId Passed
        //    if (session.UserId > 0)
        //    {
        //        sql += "AND (CreatedByID = " + userId + ") ";
        //    }
        //    var ds = new DbHelper().GetDataSet(sql, session);
        //    var dt = ds.Tables[0];
        //    if (dt.Rows.Count > 0)
        //    {
        //        notesCount = int.Parse(dt.Rows[0]["NotesCount"].ToString());
        //    }
        //    dt.Dispose();
        //    ds.Dispose();
        //    return notesCount;
        //}

        //public int GetNumberOfTasks(Session session, DateTime startDate, DateTime endDate, int subscriberId, int userId = 0)
        //{
        //    var tasksCount = 0;
        //    var validDates = Util.ValidStartEndDates(startDate, endDate);
        //    if (!validDates || (subscriberId <= 0)) return tasksCount;
        //    var sql = "SELECT Count(ID) AS TasksCount ";
        //    sql += "FROM tblActivities ";
        //    sql += "WHERE (SubscriberID = " + subscriberId + ") ";
        //    sql += "AND (ActivityTypeCode = 'N') ";
        //    sql += "AND (CreatedDate BETWEEN '" + startDate.ToString("yyyy-MM-dd") + "' AND '" + endDate.ToString("yyyy-MM-dd") + "') ";
        //    // If UserId Passed
        //    if (session.UserId > 0)
        //    {
        //        sql += "AND (CreatedByID = " + userId + ") ";
        //    }
        //    var ds = new DbHelper().GetDataSet(sql, session);
        //    var dt = ds.Tables[0];
        //    if (dt.Rows.Count > 0)
        //    {
        //        tasksCount = int.Parse(dt.Rows[0]["TasksCount"].ToString());
        //    }
        //    dt.Dispose();
        //    ds.Dispose();
        //    return tasksCount;
        //}

        //#endregion

        //#region * Freight Transactions *

        //public double GetProfitForSalesRepCode(string salesRepCode, DateTime startDate, DateTime endDate, string targetCurrencyCode, Session session)
        //{
        //    double dblProfit = 0;
        //    if (session.SubscriberId <= 0) return dblProfit;
        //    var sql = "SELECT SUM(Revenue_Total) AS TotalRevenue, SUM(Expense) AS TotalExpense , CurrencyCode ";
        //    sql += "FROM tblFreightTransactions ";
        //    sql += "WHERE (SubscriberID = " + session.SubscriberId + ") ";
        //    sql += "AND (ControllingSalesRep = '" + salesRepCode + "' OR MaintenanceSalesRep = '" + salesRepCode + "') ";
        //    sql += "AND (InvoiceDate BETWEEN '" + startDate.ToString("yyyy-MM-dd") + "' AND '" + endDate.ToString("yyyy-MM-dd") + "') ";
        //    sql += "GROUP BY CurrencyCode ";
        //    var ds = new DbHelper().GetDataSet(sql, session);
        //    var dt = ds.Tables[0];
        //    if (dt.Rows.Count > 0)
        //    {
        //        for (var i = 0; i <= dt.Rows.Count - 1; i++)
        //        {
        //            var sourceCurrencyCode = Util.NullToEmpty(dt.Rows[i]["CurrencyCode"]);
        //            double sourceProfit = Util.NullToZero(dt.Rows[i]["TotalRevenue"]) - Util.NullToZero(dt.Rows[0]["TotalExpense"]);
        //            if (sourceCurrencyCode != targetCurrencyCode)
        //            {
        //                // Currency Conversion from Source to Target
        //                var cxr = new CurrencyExchange();
        //                var targetProfit = cxr.GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, targetCurrencyCode, sourceProfit, DateTime.Now, session);
        //                dblProfit += targetProfit;
        //            }
        //            else
        //            {
        //                dblProfit += sourceProfit;
        //            }
        //        }
        //    }
        //    dt.Dispose();
        //    ds.Dispose();
        //    return dblProfit;
        //}

        //public double GetRevenueForSalesRepCode(string salesRepCode, DateTime startDate, DateTime endDate, string targetCurrencyCode, Session session)
        //{
        //    double dblRevenue = 0;
        //    if (session.SubscriberId <= 0) return dblRevenue;
        //    var sql = "SELECT SUM(Revenue_Total) AS TotalRevenue, CurrencyCode ";
        //    sql += "FROM tblFreightTransactions ";
        //    sql += "WHERE (SubscriberID = " + session.SubscriberId + ") ";
        //    sql += "AND (ControllingSalesRep = '" + salesRepCode + "' OR MaintenanceSalesRep = '" + salesRepCode + "') ";
        //    sql += "AND (InvoiceDate BETWEEN '" + startDate.ToString("yyyy-MM-dd") + "' AND '" + endDate.ToString("yyyy-MM-dd") + "') ";
        //    sql += "GROUP BY CurrencyCode ";
        //    var ds = new DbHelper().GetDataSet(sql, session);
        //    var dt = ds.Tables[0];
        //    if (dt.Rows.Count > 0)
        //    {
        //        for (var i = 0; i <= dt.Rows.Count - 1; i++)
        //        {
        //            var sourceCurrencyCode = Util.NullToEmpty(dt.Rows[i]["CurrencyCode"]);
        //            double sourceRevenue = Util.NullToZero(dt.Rows[i]["TotalRevenue"]);
        //            if (sourceCurrencyCode != targetCurrencyCode)
        //            {
        //                // Currency Conversion from Source to Target
        //                var cxr = new CurrencyExchange();
        //                var targetRevenue = cxr.GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, targetCurrencyCode, sourceRevenue, DateTime.Now, session);
        //                dblRevenue += targetRevenue;
        //            }
        //            else
        //            {
        //                dblRevenue += sourceRevenue;
        //            }
        //        }
        //    }
        //    dt.Dispose();
        //    ds.Dispose();
        //    return dblRevenue;
        //}

        //public DateRange GetMonthDateRange(DateTime asOfDate)
        //{
        //    var dateRange = new DateRange();
        //    var month = asOfDate.Month;
        //    var year = asOfDate.Year;
        //    dateRange.StartDate = new DateTime(year, month, 1);
        //    dateRange.EndDate = asOfDate;
        //    return dateRange;
        //}

        //public DateRange GetYearDateRange(DateTime asOfDate)
        //{
        //    var dateRange = new DateRange();
        //    var year = asOfDate.Year;
        //    dateRange.StartDate = new DateTime(year, 1, 1);
        //    dateRange.EndDate = asOfDate;
        //    return dateRange;
        //}

        //public double GetMonthlyActualProfit(int userId, DateTime asOfDate, string targetCurrencyCode, Session session)
        //{
        //    double profit = 0;
        //    if (session.SubscriberId <= 0) return profit;
        //    var salesRepCode = new SalesReps().GetSalesRepCodeFromUserId(userId, session);
        //    if (string.IsNullOrEmpty(salesRepCode)) return profit;
        //    var dateRange = GetMonthDateRange(asOfDate);
        //    profit = GetProfitForSalesRepCode(salesRepCode, dateRange.StartDate, dateRange.EndDate, targetCurrencyCode, session);
        //    return profit;
        //}

        //public double GetMonthlyActualRevenue(int userId, DateTime asOfDate, string targetCurrencyCode, Session session)
        //{
        //    double dblRevenue = 0;
        //    if (session.SubscriberId <= 0) return dblRevenue;
        //    var salesRepCode = new SalesReps().GetSalesRepCodeFromUserId(userId, session);
        //    if (string.IsNullOrEmpty(salesRepCode)) return dblRevenue;
        //    var dateRange = GetMonthDateRange(asOfDate);
        //    dblRevenue = GetRevenueForSalesRepCode(salesRepCode, dateRange.StartDate, dateRange.EndDate, targetCurrencyCode, session);
        //    return dblRevenue;
        //}

        //public double GetYearlyActualProfit(int userId, DateTime asOfDate, string targetCurrencyCode, Session session)
        //{
        //    double dblProfit = 0;
        //    if (session.SubscriberId <= 0) return dblProfit;
        //    var salesRepCode = new SalesReps().GetSalesRepCodeFromUserId(userId, session);
        //    if (string.IsNullOrEmpty(salesRepCode)) return dblProfit;
        //    var dateRange = GetYearDateRange(asOfDate);
        //    dblProfit = GetProfitForSalesRepCode(salesRepCode, dateRange.StartDate, dateRange.EndDate, targetCurrencyCode, session);
        //    return dblProfit;
        //}

        //public double GetYearlyActualRevenue(int userId, DateTime asOfDate, string targetCurrencyCode, Session session)
        //{
        //    double dblRevenue = 0;
        //    if (session.SubscriberId <= 0) return dblRevenue;
        //    var salesRepCode = new SalesReps().GetSalesRepCodeFromUserId(userId, session);
        //    if (string.IsNullOrEmpty(salesRepCode)) return dblRevenue;
        //    var dateRange = GetYearDateRange(asOfDate);
        //    dblRevenue = GetRevenueForSalesRepCode(salesRepCode, dateRange.StartDate, dateRange.EndDate, targetCurrencyCode, session);
        //    return dblRevenue;
        //}

        //#endregion

        //#region * Leads *

        ///// <summary>
        ///// This function gets the total active leads count
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="subscriberId"></param>
        ///// <param name="session"></param>
        ///// <returns></returns>
        //public int LeadsActiveCount(int userId, int subscriberId, Session session)
        //{
        //    var activeLeads = 0;
        //    var salesRepCode = new SalesReps().GetSalesRepCodeFromUserId(userId, session);
        //    if (session.SubscriberId <= 0) return activeLeads;
        //    var sql = "SELECT Count(ID) AS ActiveLeads ";
        //    sql += "FROM tblLeads ";
        //    sql += "WHERE (SubscriberID = " + subscriberId + ") ";
        //    sql += "AND (AssignedSalesRepCode = '" + salesRepCode + "') ";
        //    sql += "AND (LeadStatusCode <> 'U') ";
        //    // Unassigned
        //    sql += "AND (LeadStatusCode <> 'C') ";
        //    // Converted
        //    sql += "AND (LeadStatusCode <> 'L') ";
        //    // Lost
        //    sql += "AND (LeadStatusCode <> 'D') ";
        //    // Disqualified
        //    sql += "AND (LeadStatusCode <> 'W') ";
        //    // Won
        //    sql += "AND (LeadStatusCode <> 'S') ";
        //    // Secured
        //    sql += "AND (Deleted = 0) ";
        //    var ds = new DbHelper().GetDataSet(sql, session);
        //    var dt = ds.Tables[0];
        //    if (dt.Rows.Count > 0)
        //    {
        //        activeLeads = Util.NullToZero(dt.Rows[0]["ActiveLeads"]);
        //    }
        //    dt.Dispose();
        //    ds.Dispose();
        //    return activeLeads;
        //}

        ///// <summary>
        ///// This function returns the newly cretaed leads count
        ///// </summary>
        ///// <param name="session"></param>
        ///// <param name="startDate"></param>
        ///// <param name="endDate"></param>
        ///// <returns></returns>
        //public int GetLeadsCreatedCount(Session session, DateTime startDate, DateTime endDate)
        //{
        //    var newLeads = 0;
        //    var strSalesRepCode = new SalesReps().GetSalesRepCodeFromUserId(session.UserId, session);
        //    if (session.SubscriberId <= 0) return newLeads;
        //    var sql = "SELECT Count(ID) AS NewLeads ";
        //    sql += "FROM tblLeads ";
        //    sql += "WHERE (SubscriberID = " + session.SubscriberId + ") ";
        //    sql += "AND (AssignedSalesRepCode = '" + strSalesRepCode + "') ";
        //    sql += "AND (CreatedDate BETWEEN '" + startDate.ToString("yyyy-MM-dd") + "' AND '" + endDate.ToString("yyyy-MM-dd") + "') ";
        //    sql += "AND (Deleted = 0) ";
        //    var ds = new DbHelper().GetDataSet(sql, session);
        //    var dt = ds.Tables[0];
        //    if (dt.Rows.Count > 0)
        //    {
        //        newLeads = Util.NullToZero(dt.Rows[0]["NewLeads"]);
        //    }
        //    dt.Dispose();
        //    ds.Dispose();
        //    return newLeads;
        //}

        ///// <summary>
        ///// This function returns the number of lost leads count
        ///// </summary>
        ///// <param name="session"></param>
        ///// <param name="startDate"></param>
        ///// <param name="endDate"></param>
        ///// <returns></returns>
        //public int GetLeadsLostCount(Session session, DateTime startDate, DateTime endDate)
        //{
        //    var lostLeads = 0;
        //    var strSalesRepCode = new SalesReps().GetSalesRepCodeFromUserId(session.UserId, session);
        //    if (session.SubscriberId <= 0 || string.IsNullOrEmpty(strSalesRepCode)) return lostLeads;
        //    var sql = "SELECT Count(ID) AS LostLeads ";
        //    sql += "FROM tblLeads ";
        //    sql += "WHERE (SubscriberID = " + session.SubscriberId + ") ";
        //    sql += "AND (AssignedSalesRepCode = '" + strSalesRepCode + "') ";
        //    sql += "AND (DateLost BETWEEN '" + startDate.ToString("yyyy-MM-dd") + "' AND '" + endDate.ToString("yyyy-MM-dd") + "') ";
        //    sql += "AND (Deleted = 0) ";
        //    var ds = new DbHelper().GetDataSet(sql, session);
        //    var dt = ds.Tables[0];
        //    if (dt.Rows.Count > 0)
        //    {
        //        lostLeads = Util.NullToZero(dt.Rows[0]["LostLeads"]);
        //    }
        //    dt.Dispose();
        //    ds.Dispose();
        //    return lostLeads;
        //}

        ///// <summary>
        ///// This function returns the number of recieved leads count
        ///// </summary>
        ///// <param name="session"></param>
        ///// <param name="startDate"></param>
        ///// <param name="endDate"></param>
        ///// <returns></returns>
        //public int GetLeadsRecievedCount(Session session, DateTime startDate, DateTime endDate)
        //{
        //    var recievedLeads = 0;
        //    if (session.SubscriberId <= 0) return recievedLeads;
        //    var sql = "SELECT Count(ID) AS RecievedLeads ";
        //    sql += "FROM tblLeads ";
        //    sql += "WHERE (SubscriberID = " + session.SubscriberId + ") ";
        //    sql += "AND (ReferredToUserID = " + session.UserId + ") ";
        //    sql += "AND (ReferralDate BETWEEN '" + startDate.ToString("yyyy-MM-dd") + "' AND '" + endDate.ToString("yyyy-MM-dd") + "') ";
        //    sql += "AND (Deleted = 0) ";
        //    var ds = new DbHelper().GetDataSet(sql, session);
        //    var dt = ds.Tables[0];
        //    if (dt.Rows.Count > 0)
        //    {
        //        recievedLeads = Util.NullToZero(dt.Rows[0]["RecievedLeads"]);
        //    }
        //    dt.Dispose();
        //    ds.Dispose();
        //    return recievedLeads;
        //}

        ///// <summary>
        ///// This function returns the number of sent leads count
        ///// </summary>
        ///// <param name="session"></param>
        ///// <param name="startDate"></param>
        ///// <param name="endDate"></param>
        ///// <returns></returns>
        //public int GetLeadsSentCount(Session session, DateTime startDate, DateTime endDate)
        //{
        //    var sentLeads = 0;
        //    if (session.SubscriberId <= 0) return sentLeads;
        //    var sql = "SELECT Count(ID) AS SentLeads ";
        //    sql += "FROM tblLeads ";
        //    sql += "WHERE (SubscriberID = " + session.SubscriberId + ") ";
        //    sql += "AND (ReferredByUserID = " + session.UserId + ") ";
        //    sql += "AND (ReferralDate BETWEEN '" + startDate.ToString("yyyy-MM-dd") + "' AND '" + endDate.ToString("yyyy-MM-dd") + "') ";
        //    sql += "AND (Deleted = 0) ";
        //    var ds = new DbHelper().GetDataSet(sql, session);
        //    var dt = ds.Tables[0];
        //    if (dt.Rows.Count > 0)
        //    {
        //        sentLeads = Util.NullToZero(dt.Rows[0]["SentLeads"]);
        //    }
        //    dt.Dispose();
        //    ds.Dispose();
        //    return sentLeads;
        //}


        ///// <summary>
        ///// This function returns the number of leads won count
        ///// </summary>
        ///// <param name="session"></param>
        ///// <param name="startDate"></param>
        ///// <param name="endDate"></param>
        ///// <returns></returns>
        //public int GetLeadsWonCount(Session session, DateTime startDate, DateTime endDate)
        //{
        //    var wonLeads = 0;
        //    var strSalesRepCode = new SalesReps().GetSalesRepCodeFromUserId(session.UserId, session);
        //    if (session.SubscriberId <= 0 || string.IsNullOrEmpty(strSalesRepCode)) return wonLeads;
        //    var sql = "SELECT Count(ID) AS WonLeads ";
        //    sql += "FROM tblLeads ";
        //    sql += "WHERE (SubscriberID = " + session.SubscriberId + ") ";
        //    sql += "AND (ConvertedDate is not null) ";
        //    sql += "AND (AssignedSalesRepCode = '" + strSalesRepCode + "') ";
        //    sql += "AND (ConvertedDate BETWEEN '" + startDate.ToString("yyyy-MM-dd") + "' AND '" + endDate.ToString("yyyy-MM-dd") + "') ";
        //    sql += "AND (Deleted = 0) ";
        //    var ds = new DbHelper().GetDataSet(sql, session);
        //    var dt = ds.Tables[0];
        //    if (dt.Rows.Count > 0)
        //    {
        //        wonLeads = Util.NullToZero(dt.Rows[0]["WonLeads"]);
        //    }
        //    dt.Dispose();
        //    ds.Dispose();
        //    return wonLeads;
        //}

        //#endregion

        //#region * Opportunities *

        //public int OpportunitiesActiveCount(int userId, int subscriberId, Session session)
        //{
        //    var intActiveOpportunities = 0;
        //    if (session.SubscriberId <= 0) return intActiveOpportunities;
        //    var sql = "SELECT Count(ID) AS ActiveOpportunities ";
        //    sql += "FROM tblOpportunities ";
        //    sql += "WHERE (SubscriberID = " + subscriberId + ") ";
        //    sql += "AND (OpportunityOwnerID = " + userId + ") ";
        //    sql += "AND (Deleted = 0) ";
        //    sql += "AND (Won = 0) ";
        //    sql += "AND (Lost = 0) ";
        //    sql += "AND (SalesStage NOT LIKE '%Stalled%') ";
        //    var ds = new DbHelper().GetDataSet(sql, session);
        //    var dt = ds.Tables[0];
        //    if (dt.Rows.Count > 0)
        //    {
        //        intActiveOpportunities = Util.NullToZero(dt.Rows[0]["ActiveOpportunities"]);
        //    }
        //    dt.Dispose();
        //    ds.Dispose();
        //    return intActiveOpportunities;
        //}

        //public double OpportunitiesActiveProfit(int userId, int subscriberId, string targetCurrencyCode, Session session)
        //{
        //    double profit = 0;
        //    if (subscriberId <= 0) return profit;
        //    var sql = "SELECT Revenue, CurrencyCode, ";
        //    sql += "GrossProfitPercent ";
        //    sql += "FROM tblOpportunities ";
        //    sql += "WHERE (SubscriberID = " + subscriberId + ") ";
        //    sql += "AND (OpportunityOwnerID = " + userId + ") ";
        //    sql += "AND (Won = 0) ";
        //    sql += "AND (Lost = 0) ";
        //    sql += "AND (Deleted = 0) ";
        //    sql += "AND (SalesStage NOT LIKE '%Stalled%') ";
        //    var ds = new DbHelper().GetDataSet(sql, session);
        //    var dt = ds.Tables[0];
        //    if (dt.Rows.Count > 0)
        //    {
        //        for (var i = 0; i <= dt.Rows.Count - 1; i++)
        //        {
        //            var sourceCurrencyCode = Util.NullToEmpty(dt.Rows[i]["CurrencyCode"]);
        //            double sourceRevenue = Util.NullToZero(dt.Rows[i]["Revenue"]);
        //            double profitPercent = Util.NullToZero(dt.Rows[i]["GrossProfitPercent"]);
        //            var sourceProfit = sourceRevenue * profitPercent / 100;
        //            if (sourceCurrencyCode != targetCurrencyCode)
        //            {
        //                //Currency Conversion from Source to Target
        //                var cxr = new CurrencyExchange();
        //                var targetProfit = cxr.GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, targetCurrencyCode, sourceProfit, DateTime.Now, session);
        //                profit += targetProfit;
        //            }
        //            else
        //            {
        //                profit += sourceProfit;
        //            }
        //        }
        //    }
        //    dt.Dispose();
        //    ds.Dispose();
        //    return profit;
        //}

        //public double OpportunitiesActiveRevenue(int userId, int subscriberId, string targetCurrencyCode, Session session)
        //{
        //    double dblRevenue = 0;
        //    if (subscriberId <= 0) return dblRevenue;
        //    var sql = "SELECT Revenue, CurrencyCode ";
        //    sql += "FROM tblOpportunities ";
        //    sql += "WHERE (SubscriberID = " + subscriberId + ") ";
        //    sql += "AND (OpportunityOwnerID = " + userId + ") ";
        //    sql += "AND (Won = 0) ";
        //    sql += "AND (Lost = 0) ";
        //    sql += "AND (Deleted = 0) ";
        //    sql += "AND (SalesStage NOT LIKE '%Stalled%') ";
        //    var ds = new DbHelper().GetDataSet(sql, session);
        //    var dt = ds.Tables[0];
        //    if (dt.Rows.Count > 0)
        //    {
        //        for (var i = 0; i <= dt.Rows.Count - 1; i++)
        //        {
        //            var sourceCurrencyCode = Util.NullToEmpty(dt.Rows[i]["CurrencyCode"]);
        //            double sourceRevenue = Util.NullToZero(dt.Rows[i]["Revenue"]);
        //            if (sourceCurrencyCode != targetCurrencyCode)
        //            {
        //                // Currency Conversion from Source to Target
        //                var cxr = new CurrencyExchange();
        //                var targetRevenue = cxr.GetCalculatedCurrencyExchangeValue(sourceCurrencyCode,
        //                    targetCurrencyCode, sourceRevenue, DateTime.Now, session);
        //                dblRevenue += targetRevenue;
        //            }
        //            else
        //            {
        //                dblRevenue += sourceRevenue;
        //            }
        //        }
        //    }
        //    dt.Dispose();
        //    ds.Dispose();
        //    return dblRevenue;
        //}

        //public int OpportunitiesCreatedCount(int userId, int subscriberId, DateTime startDate, DateTime endDate, Session session)
        //{
        //    var newOpportunities = 0;
        //    if (subscriberId <= 0) return newOpportunities;
        //    var sql = "SELECT Count(ID) AS NewOpportunities ";
        //    sql += "FROM tblOpportunities ";
        //    sql += "WHERE (SubscriberID = " + subscriberId + ") ";
        //    sql += "AND (OpportunityOwnerID = " + userId + ") ";
        //    sql += "AND (CreatedDate BETWEEN '" + startDate.ToString("yyyy-MM-dd") + "' AND '" + endDate.ToString("yyyy-MM-dd") + "') ";
        //    sql += "AND (Deleted = 0) ";
        //    var ds = new DbHelper().GetDataSet(sql, session);
        //    var dt = ds.Tables[0];
        //    if (dt.Rows.Count > 0)
        //    {
        //        newOpportunities = Util.NullToZero(dt.Rows[0]["NewOpportunities"]);
        //    }
        //    dt.Dispose();
        //    ds.Dispose();
        //    return newOpportunities;
        //}

        //public int OpportunitiesWonCount(int userId, int subscriberId, DateTime startDate, DateTime endDate, Session session)
        //{
        //    var opportunitiesWon = 0;
        //    if (subscriberId <= 0) return opportunitiesWon;
        //    var sql = "SELECT Count(ID) AS OpportunitiesWon ";
        //    sql += "FROM tblOpportunities ";
        //    sql += "WHERE (SubscriberID = " + subscriberId + ") ";
        //    sql += "AND (OpportunityOwnerID = " + userId + ") ";
        //    // Date Range
        //    sql += "AND (DateWon BETWEEN '" + startDate.ToString("yyyy-MM-dd") + "' AND '" + endDate.ToString("yyyy-MM-dd") + "') ";
        //    sql += "AND (Won = 1) ";
        //    sql += "AND (Deleted = 0) ";
        //    var ds = new DbHelper().GetDataSet(sql, session);
        //    var dt = ds.Tables[0];
        //    if (dt.Rows.Count > 0)
        //    {
        //        opportunitiesWon = Util.NullToZero(dt.Rows[0]["OpportunitiesWon"]);
        //    }
        //    dt.Dispose();
        //    ds.Dispose();
        //    return opportunitiesWon;
        //}

        //public double OpportunitiesAnnualWonProfit(int userId, int subscriberId, DateTime startDate, DateTime endDate, string targetCurrencyCode, Session session)
        //{
        //    double dblProfit = 0;
        //    if (subscriberId <= 0) return dblProfit;
        //    var sql = "SELECT Revenue, GrossProfitPercent, CurrencyCode, DateWon ";
        //    sql += "FROM tblOpportunities ";
        //    sql += "WHERE (SubscriberID = " + subscriberId + ") ";
        //    sql += "AND (OpportunityOwnerID = " + userId + ") ";
        //    sql += "AND (DateWon BETWEEN '" + startDate.ToString("yyyy-MM-dd") + "' AND '" + endDate.ToString("yyyy-MM-dd") + "') ";
        //    sql += "AND (Won = 1) ";
        //    sql += "AND (Deleted = 0) ";
        //    var ds = new DbHelper().GetDataSet(sql, session);
        //    var dt = ds.Tables[0];
        //    if (dt.Rows.Count > 0)
        //    {
        //        for (var i = 0; i <= dt.Rows.Count - 1; i++)
        //        {
        //            var currencyExchangeDate = Util.GetDateTime(dt.Rows[i]["DateWon"]);
        //            var sourceCurrencyCode = Util.NullToEmpty(dt.Rows[i]["CurrencyCode"]);
        //            double sourceRevenue = Util.NullToZero(dt.Rows[i]["Revenue"]);
        //            double profitPercent = Util.NullToZero(dt.Rows[i]["GrossProfitPercent"]);
        //            var sourceProfit = sourceRevenue * profitPercent / 100;
        //            if (sourceCurrencyCode != targetCurrencyCode)
        //            {
        //                // Currency Conversion from Source to Target
        //                var cxr = new CurrencyExchange();
        //                var targetProfit = cxr.GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, targetCurrencyCode, sourceProfit, currencyExchangeDate, session);
        //                dblProfit += targetProfit;
        //            }
        //            else
        //            {
        //                dblProfit += sourceProfit;
        //            }
        //        }
        //    }
        //    dt.Dispose();
        //    ds.Dispose();
        //    return dblProfit;
        //}

        //public double OpportunitiesAnnualWonRevenue(int userId, int subscriberId, DateTime startDate, DateTime endDate, string targetCurrencyCode, Session session)
        //{
        //    double dblWonRevenue = 0;
        //    if (subscriberId <= 0) return dblWonRevenue;
        //    var sql = "";
        //    sql += "SELECT Revenue, CurrencyCode, DateWon ";
        //    sql += "FROM tblOpportunities ";
        //    sql += "WHERE (SubscriberID = " + subscriberId + ") ";
        //    sql += "AND (OpportunityOwnerID = " + userId + ") ";
        //    sql += "AND (Won = 1) ";
        //    sql += "AND (Deleted = 0) ";
        //    // Date Range
        //    sql += "AND (DateWon BETWEEN '" + startDate.ToString("yyyy-MM-dd") + "' AND '" + endDate.ToString("yyyy-MM-dd") + "') ";
        //    var ds = new DbHelper().GetDataSet(sql, session);
        //    var dt = ds.Tables[0];
        //    if (dt.Rows.Count > 0)
        //    {
        //        for (var i = 0; i <= dt.Rows.Count - 1; i++)
        //        {
        //            var currencyExchangeDate = Util.GetDateTime(dt.Rows[i]["DateWon"]);
        //            var sourceCurrencyCode = Util.NullToEmpty(dt.Rows[i]["CurrencyCode"]);
        //            double sourceRevenue = Util.NullToZero(dt.Rows[i]["Revenue"]);
        //            if (sourceCurrencyCode != targetCurrencyCode)
        //            {
        //                //Currency Conversion from Source to Target
        //                var cxr = new CurrencyExchange();
        //                var targetRevenue = cxr.GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, targetCurrencyCode, sourceRevenue, currencyExchangeDate, session);
        //                dblWonRevenue += targetRevenue;
        //            }
        //            else
        //            {
        //                dblWonRevenue += sourceRevenue;
        //            }
        //        }
        //    }
        //    dt.Dispose();
        //    ds.Dispose();
        //    return dblWonRevenue;
        //}

        //public double OpportunitiesInYearProfit(int userId, int subscriberId, DateTime dateFrom, DateTime dateTo, string targetCurrencyCode, string opportunityType, Session session, int opportunityId = 0)
        //{
        //    double dblInYearProfitTotal = 0;
        //    // Calculate Profit Total for Current Year - Pro-rate based upon EstimatedStartDate
        //    if (subscriberId <= 0) return dblInYearProfitTotal;
        //    // TODO: Sort out DateTime and Nulls for ALL Metrics functions
        //    var year = Convert.ToString(DateTime.Now.Year);
        //    var endDate = "12/31/" + year;
        //    var startDate = "10/01/" + year;
        //    var yearEndDate = Convert.ToDateTime(endDate);
        //    var yearStartDate = Convert.ToDateTime(startDate);
        //    var sql = "SELECT Revenue, GrossProfitPercent, EstimatedStartDate, DateWon, CurrencyCode ";
        //    sql += "FROM tblOpportunities ";
        //    sql += "WHERE (SubscriberID = " + subscriberId + ") ";
        //    sql += "AND (OpportunityOwnerID = " + userId + ") ";
        //    sql += "AND (EstimatedStartDate >= '" + yearStartDate + "') ";
        //    sql += "AND (Deleted = 0) ";
        //    // Date Range
        //    sql += "AND (EstimatedStartDate BETWEEN '" + dateFrom + "' AND '" + dateTo + "') ";
        //    if (opportunityId != 0)
        //    {
        //        sql += "AND (ID = " + opportunityId + ") ";
        //    }
        //    var ds = new DbHelper().GetDataSet(sql, session);
        //    var dt = ds.Tables[0];
        //    if (dt.Rows.Count > 0)
        //    {
        //        for (var i = 0; i <= dt.Rows.Count - 1; i++)
        //        {
        //            // Calculate Pro-rate Percentage
        //            // EstimatedStartDate is Mandatory
        //            // TODO: Trap for Null EstimatedStartDate - Just in Case
        //            var estimatedStartDate = (DateTime)dt.Rows[i]["EstimatedStartDate"];
        //            DateTime? currencyExchangeDate = null;
        //            switch (opportunityType)
        //            {
        //                case "Active":
        //                    currencyExchangeDate = DateTime.Now;
        //                    break;
        //                case "Lost":
        //                    currencyExchangeDate = Util.GetDateTime(dt.Rows[i]["DateLost"]);
        //                    break;
        //                case "Stalled":
        //                    currencyExchangeDate = DateTime.Now;
        //                    break;
        //                case "Won":
        //                    currencyExchangeDate = Util.GetDateTime(dt.Rows[i]["DateWon"]);
        //                    break;
        //            }
        //            // EstimatedStartDate can not be blank
        //            double proratePercentage;
        //            if (estimatedStartDate == null)
        //            {
        //                proratePercentage = 1;
        //            }
        //            else
        //            {
        //                var timeSpan = yearEndDate.Subtract(estimatedStartDate);
        //                var daysLeftInCurrentYear = timeSpan.Days + 1;
        //                // If estimated start date is in last year - use 365 days
        //                if (daysLeftInCurrentYear > 365)
        //                {
        //                    daysLeftInCurrentYear = 365;
        //                }
        //                // ReSharper disable PossibleLossOfFraction
        //                proratePercentage = daysLeftInCurrentYear / 365;
        //                // ReSharper restore PossibleLossOfFraction
        //            }
        //            var sourceCurrencyCode = Util.NullToEmpty(dt.Rows[i]["CurrencyCode"]);
        //            double sourceRevenue = Util.NullToZero(dt.Rows[i]["Revenue"]);
        //            double profitPercent = Util.NullToZero(dt.Rows[i]["GrossProfitPercent"]);
        //            var inYearSourceProfit = sourceRevenue * (profitPercent / 100) * proratePercentage;
        //            if (sourceCurrencyCode != targetCurrencyCode)
        //            {
        //                //Currency Conversion from Source to Target
        //                var cxr = new CurrencyExchange();
        //                var inYearTargetProfit = cxr.GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, targetCurrencyCode, inYearSourceProfit, currencyExchangeDate, session);
        //                dblInYearProfitTotal += inYearTargetProfit;
        //            }
        //            else
        //            {
        //                dblInYearProfitTotal += inYearSourceProfit;
        //            }
        //        }
        //    }
        //    dt.Dispose();
        //    ds.Dispose();
        //    return dblInYearProfitTotal;
        //}

        //public double OpportunitiesInYearRevenue(int userId, int subscriberId, DateTime dateFrom, DateTime dateTo, string targetCurrencyCode, string opportunityType, Session session, int opportunityId = 0)
        //{
        //    double inYearRevenueTotal = 0;
        //    // Calculate Revenue Total for Current Year - Pro-rate based upon EstimatedStartDate
        //    if (subscriberId <= 0) return inYearRevenueTotal;
        //    var year = Convert.ToString(DateTime.Now.Year);
        //    var endDate = "12/31/" + year;
        //    var startDate = "10/01/" + year;
        //    var yearEndDate = Convert.ToDateTime(endDate);
        //    var yearStartDate = Convert.ToDateTime(startDate);
        //    var sql = "SELECT Revenue, EstimatedStartDate, DateWon, DateLost, CurrencyCode ";
        //    sql += "FROM tblOpportunities ";
        //    sql += "WHERE (SubscriberID = " + subscriberId + ") ";
        //    sql += "AND (OpportunityOwnerID = " + userId + ") ";
        //    sql += "AND (EstimatedStartDate >= '" + yearStartDate + "') ";
        //    sql += "AND (Deleted = 0) ";
        //    // Date Range
        //    sql += "AND (EstimatedStartDate BETWEEN '" + dateFrom + "' AND '" + dateTo + "') ";
        //    // Opportunity Id
        //    if (opportunityId != 0)
        //    {
        //        sql += "AND (ID = " + opportunityId + ") ";
        //    }
        //    var ds = new DbHelper().GetDataSet(sql, session);
        //    var dt = ds.Tables[0];
        //    if (dt.Rows.Count > 0)
        //    {
        //        for (var i = 0; i <= dt.Rows.Count - 1; i++)
        //        {
        //            // Calculate Pro-rate Percentage
        //            var estimatedStartDate = (DateTime)(dt.Rows[i]["EstimatedStartDate"]);
        //            DateTime? currencyExchangeDate = null;
        //            switch (opportunityType)
        //            {
        //                case "Active":
        //                    currencyExchangeDate = DateTime.Now;
        //                    break;
        //                case "Lost":
        //                    currencyExchangeDate = Util.GetDateTime(dt.Rows[i]["DateLost"]);
        //                    break;
        //                case "Stalled":
        //                    currencyExchangeDate = DateTime.Now;
        //                    break;
        //                case "Won":
        //                    currencyExchangeDate = Util.GetDateTime(dt.Rows[i]["DateWon"]);
        //                    break;
        //            }
        //            // EstimatedStartDate can not be blank
        //            double proratePercentage;
        //            if (estimatedStartDate == null)
        //            {
        //                proratePercentage = 0;
        //            }
        //            else
        //            {
        //                var tsTimeSpan = yearEndDate.Subtract(estimatedStartDate);
        //                var daysLeftInCurrentYear = tsTimeSpan.Days + 1;
        //                // If estimated start date is in last year - use 365 days
        //                if (daysLeftInCurrentYear > 365)
        //                {
        //                    daysLeftInCurrentYear = 365;
        //                }
        //                // ReSharper disable PossibleLossOfFraction
        //                proratePercentage = daysLeftInCurrentYear / 365;
        //                // ReSharper restore PossibleLossOfFraction
        //            }
        //            var sourceCurrencyCode = Util.NullToEmpty(dt.Rows[i]["CurrencyCode"]);
        //            double sourceRevenue = Util.NullToZero(dt.Rows[i]["Revenue"]);
        //            if (sourceCurrencyCode != targetCurrencyCode)
        //            {
        //                //Currency Conversion from Source to Target
        //                var cxr = new CurrencyExchange();
        //                var targetRevenue = cxr.GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, targetCurrencyCode, sourceRevenue, currencyExchangeDate, session);
        //                // Prorate
        //                var inYearRevenue = targetRevenue * proratePercentage;
        //                inYearRevenueTotal += inYearRevenue;
        //            }
        //            else
        //            {
        //                // Prorate
        //                var inYearRevenue = sourceRevenue * proratePercentage;
        //                inYearRevenueTotal += inYearRevenue;
        //            }
        //        }
        //    }
        //    dt.Dispose();
        //    ds.Dispose();
        //    return inYearRevenueTotal;
        //}

        //#endregion

        //#region * User Activity *

        //public int GetActiveSubscriberUsers(int subscriberId, Session session)
        //{
        //    var activeUserCount = 0;
        //    if (subscriberId <= 0) return activeUserCount;
        //    var sql = "SELECT COUNT(ID) AS ActiveUsers ";
        //    sql += "From tblUsers ";
        //    sql += "WHERE (SubscriberID = " + subscriberId + ") ";
        //    sql += "AND (Deleted = 0) ";
        //    sql += "AND (Active = 1) ";
        //    sql += "AND (LoginEnabled = 1) ";
        //    sql += "AND (AdminUser = 0) ";
        //    var ds = new DbHelper().GetDataSet(sql, session);
        //    var dt = ds.Tables[0];
        //    if (dt.Rows.Count > 0)
        //    {
        //        activeUserCount = Util.NullToZero(dt.Rows[0]["ActiveUsers"]);
        //    }
        //    dt.Dispose();
        //    ds.Dispose();
        //    return activeUserCount;
        //}

        //public int UserLoginsCount(int userId, int subscriberId, DateTime startDate, DateTime endDate, Session session)
        //{
        //    var userLoginsCount = 0;
        //    if (session.SubscriberId <= 0) return userLoginsCount;
        //    var sql = "SELECT Count(ID) AS Logins ";
        //    sql += "FROM tblUserTracking ";
        //    sql += "WHERE (SubscriberID = " + subscriberId + ") ";
        //    sql += "AND (UserID = " + userId + ") ";
        //    sql += "AND (Description = 'Login') ";
        //    sql += "AND (DateAdded BETWEEN '" + startDate.ToString("yyyy-MM-dd") + "' AND '" + endDate.ToString("yyyy-MM-dd") + "') ";
        //    var ds = new DbHelper().GetDataSet(sql, session);
        //    var dt = ds.Tables[0];
        //    if (dt.Rows.Count > 0)
        //    {
        //        userLoginsCount = Util.NullToZero(dt.Rows[0]["Logins"]);
        //    }
        //    dt.Dispose();
        //    ds.Dispose();
        //    return userLoginsCount;
        //}

        //#endregion

        //#region * Update SalesRep Reminder Metrics *

        ///// <summary>
        ///// This function updates reminder email metrics
        ///// </summary>
        ///// <param name="mandrillEmail"></param>
        ///// <param name="session"></param>
        ///// <returns>records updsted count</returns>
        //public int UpdateReminderMetrics(MandrillEmail mandrillEmail, Session session)
        //{
        //    var recordsUpdated = 0;
        //    if (session.SubscriberId <= 0) return recordsUpdated;
        //    var sql = "UPDATE tblEmailSent ";
        //    sql += "SET Opens = " + mandrillEmail.Opens + ", ";
        //    sql += "Clicks = " + mandrillEmail.Clicks + ", ";
        //    sql += "LastMetricsSyncedDate = '" + mandrillEmail.StatsUpdatedTime.ToString("yyyy/MM/dd HH:mm") + "' ";
        //    sql += "WHERE (MandrilEmailId ='" + mandrillEmail.Id + "') ";
        //    recordsUpdated = new DbHelper().ExecuteQuery(sql, session);
        //    return recordsUpdated;
        //}
        //#endregion

    }
}
