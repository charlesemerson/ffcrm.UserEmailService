using ffcrm.UserEmailService.Login;
using ffcrm.UserEmailService.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ffcrm.UserEmailService
{
    public class Emailer
    {
        public Emailer()
        {
        }

        public void RunEmailer()
        {
            var lastSunday = DateTime.UtcNow.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
            var nextSaturday = DateTime.UtcNow.AddDays(6 - (int)DateTime.UtcNow.DayOfWeek);

            //Date from is last Sunday at midnight (UTC).
            var dateFrom = new DateTime(lastSunday.Year, lastSunday.Month, lastSunday.Day, 0, 0, 0);

            //Date to is next Saturday at 23:59:59 (UTC).
            var dateTo = new DateTime(nextSaturday.Year, nextSaturday.Month, nextSaturday.Day, 23, 59, 59);

            var loginDb = new DbLoginDataContext(Utils.GetLoginConnection());

            //var globalUsers = loginDb.GlobalUsers.Where(x => x.EmailDigest.ToLower() == "weekly").OrderBy(x => x.FullName);
            var globalUsers = loginDb.GlobalUsers.OrderBy(x => x.FullName);

            foreach (var globalUser in globalUsers)
            {
                var listFinal = new List<Tuple<string, DateTime>>();

                var globalDealIds = GetGlobalDealIdsForGlobalUser(globalUser.GlobalUserId, globalUser.DataCenter);

                var proposals = GetProposals(dateFrom, dateTo, globalDealIds, globalUser.DataCenter);
                var contracts = GetContracts(dateFrom, dateTo, globalDealIds, globalUser.DataCenter);
                var decisions = GetDecisions(dateFrom, dateTo, globalDealIds, globalUser.DataCenter);
                var activities = GetActivities(dateFrom, dateTo, globalUser.GlobalUserId, globalUser.DataCenter);
                var birthdays = GetBirthdays(dateFrom, dateTo, globalUser.UserId, globalUser.DataCenter);

                if (proposals != null && proposals.Any())
                {
                    foreach (var proposal in proposals)
                    {
                        if (!string.IsNullOrWhiteSpace(proposal.DealName))
                        {
                            var company = "";

                            if (!string.IsNullOrWhiteSpace(proposal.CompanyName))
                            {
                                company = $"| Company: {proposal.CompanyName}";
                            }

                            var text = $"{proposal.DateProposalDue.Value:ddd, dd-MM-yy HH:mm} - Proposal Due for: {proposal.DealName} | {company}";

                            listFinal.Add(new Tuple<string, DateTime>(text, proposal.DateProposalDue.Value));
                        }
                    }
                }

                if (contracts != null && contracts.Any())
                {

                }

                if (decisions != null && decisions.Any())
                {

                }

                if (activities != null && activities.Any())
                {

                }

                if (birthdays != null && birthdays.Any())
                {

                }
            }
        }

        private List<int> GetGlobalDealIdsForGlobalUser(int globalUserId, string dataCenter)
        {
            var sharedContext = new DbSharedDataContext(Utils.GetSharedConnection(dataCenter));
            var globalDealIds = sharedContext.LinkGlobalDealGlobalUsers.Where(x => x.GlobalUserId == globalUserId).Select(x => x.GlobalDealId);

            if (globalDealIds != null && globalDealIds.Any())
            {
                return globalDealIds.ToList();
            }

            return new List<int>();
        }

        private List<GlobalDeal> GetProposals(DateTime dateFrom, DateTime dateTo, List<int> globalDealIds, string dataCenter)
        {
            var sharedContext = new DbSharedDataContext(Utils.GetSharedConnection(dataCenter));
            var globalDeals = sharedContext.GlobalDeals.Where(x => globalDealIds.Contains(x.DealIdGlobal) && x.DateProposalDue >= dateFrom && x.DateProposalDue <= dateTo);

            if (globalDeals != null && globalDeals.Any())
            {
                return globalDeals.ToList();
            }

            return new List<GlobalDeal>();
        }

        private List<GlobalDeal> GetContracts(DateTime dateFrom, DateTime dateTo, List<int> globalDealIds, string dataCenter)
        {
            var sharedContext = new DbSharedDataContext(Utils.GetSharedConnectionForDataCenter(dataCenter));
            var globalDeals = sharedContext.GlobalDeals.Where(x => globalDealIds.Contains(x.DealIdGlobal) && x.ContractEndDate >= dateFrom && x.ContractEndDate <= dateTo);

            if (globalDeals != null && globalDeals.Any())
            {
                return globalDeals.ToList();
            }

            return new List<GlobalDeal>();
        }

        private List<GlobalDeal> GetDecisions(DateTime dateFrom, DateTime dateTo, List<int> globalDealIds, string dataCenter)
        {
            var sharedContext = new DbSharedDataContext(Utils.GetSharedConnectionForDataCenter(dataCenter));
            var globalDeals = sharedContext.GlobalDeals.Where(x => globalDealIds.Contains(x.DealIdGlobal) && x.DecisionDate >= dateFrom && x.DecisionDate <= dateTo);

            if (globalDeals != null && globalDeals.Any())
            {
                return globalDeals.ToList();
            }

            return new List<GlobalDeal>();
        }

        private List<Activity> GetActivities(DateTime dateFrom, DateTime dateTo, int globalUserId, string dataCenter)
        {
            var sharedContext = new DbSharedDataContext(Utils.GetSharedConnectionForDataCenter(dataCenter));
            var activities = sharedContext.Activities.Where(x => x.UserIdGlobal == globalUserId && x.ActivityDate >= dateFrom && x.ActivityDate <= dateTo);

            if (activities != null && activities.Any())
            {
                return activities.ToList();
            }

            return new List<Activity>();
        }

        private List<Contact> GetBirthdays(DateTime dateFrom, DateTime dateTo, int userId, string dataCenter)
        {
            var listBirthdays = new List<Contact>();

            var context = new DbFirstFreightDataContext(Utils.GetConnectionForDataCenter(dataCenter));
            var contacts = context.Contacts.Where(x => x.ContactOwnerUserId == userId);

            if (contacts != null && contacts.Any())
            {
                foreach (var contact in contacts.ToList())
                {
                    if (Utils.IsBirthdayInRange(new DateTime(2000, contact.BirthdayMonth, contact.BirthdayDay, 0, 0, 0), dateFrom, dateTo))
                    {
                        listBirthdays.Add(contact);
                    }
                }
            }

            return listBirthdays;
        }
    }
}
