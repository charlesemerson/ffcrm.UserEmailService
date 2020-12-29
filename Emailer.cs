using ffcrm.UserEmailService.Helper;
using ffcrm.UserEmailService.Login;
using ffcrm.UserEmailService.Models;
using ffcrm.UserEmailService.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ffcrm.UserEmailService
{
    public class Emailer
    {

        public void RunEmailer()
        {
            var lastSunday = DateTime.UtcNow.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
            var nextSaturday = DateTime.UtcNow.AddDays(6 - (int)DateTime.UtcNow.DayOfWeek);

            //Date from is last Sunday at midnight (UTC).
            var dateFrom = new DateTime(lastSunday.Year, lastSunday.Month, lastSunday.Day, 0, 0, 0);

            //Date to is next Saturday at 23:59:59 (UTC).
            var dateTo = new DateTime(nextSaturday.Year, nextSaturday.Month, nextSaturday.Day, 23, 59, 59);

            var loginDb = new DbLoginDataContext(Utils.GetLoginConnection());

            var globalUsers = loginDb.GlobalUsers.Where(x => x.EmailDigest.ToLower() == "weekly").OrderBy(x => x.FullName);

            foreach (var globalUser in globalUsers)
            {
                var listUpcoming = new List<Tuple<string, DateTime>>();
                var listOverdue = new List<Tuple<string, DateTime>>();

                var globalDealIds = GetGlobalDealIdsForGlobalUser(globalUser.GlobalUserId, globalUser.DataCenter);

                var proposalsUpcoming = GetProposals(dateFrom, dateTo, globalDealIds, globalUser.DataCenter);
                var contractsUpcoming = GetContracts(dateFrom, dateTo, globalDealIds, globalUser.DataCenter);
                var decisionsUpcoming = GetDecisions(dateFrom, dateTo, globalDealIds, globalUser.DataCenter);
                var activitiesUpcoming = GetActivities(dateFrom, dateTo, globalUser.GlobalUserId, globalUser.DataCenter);
                var tasksUpcoming = GetTasks(dateFrom, dateTo, globalUser.GlobalUserId, globalUser.DataCenter);
                var birthdaysUpcoming = GetBirthdays(dateFrom, dateTo, globalUser.UserId, globalUser.DataCenter);

                var proposalsOverdue = GetProposals(new DateTime(2010, 1, 1), DateTime.UtcNow, globalDealIds, globalUser.DataCenter);
                var contractsOverdue = GetContracts(new DateTime(2010, 1, 1), DateTime.UtcNow, globalDealIds, globalUser.DataCenter);
                var decisionsOverdue = GetDecisions(new DateTime(2010, 1, 1), DateTime.UtcNow, globalDealIds, globalUser.DataCenter);
                var tasksOverdue = GetTasks(new DateTime(2010, 1, 1), DateTime.UtcNow, globalUser.GlobalUserId, globalUser.DataCenter);

                if (proposalsUpcoming != null && proposalsUpcoming.Any())
                {
                    foreach (var proposal in proposalsUpcoming)
                    {
                        if (!string.IsNullOrWhiteSpace(proposal.DealName))
                        {
                            var company = "";

                            if (!string.IsNullOrWhiteSpace(proposal.CompanyName))
                            {
                                company = $"| Company: {proposal.CompanyName}";
                            }

                            var text = $"{proposal.DateProposalDue.Value:ddd, dd-MMM-yy} - Proposal Due for: {proposal.DealName} {company}";

                            listUpcoming.Add(new Tuple<string, DateTime>(text, proposal.DateProposalDue.Value));
                        }
                    }
                }

                if (contractsUpcoming != null && contractsUpcoming.Any())
                {
                    foreach (var contract in contractsUpcoming)
                    {
                        if (!string.IsNullOrWhiteSpace(contract.DealName))
                        {
                            var company = "";

                            if (!string.IsNullOrWhiteSpace(contract.CompanyName))
                            {
                                company = $"| Company: {contract.CompanyName}";
                            }

                            var text = $"{contract.ContractEndDate.Value:ddd, dd-MMM-yy} - Contract ending for: {contract.DealName} {company}";

                            listUpcoming.Add(new Tuple<string, DateTime>(text, contract.ContractEndDate.Value));
                        }
                    }
                }

                if (decisionsUpcoming != null && decisionsUpcoming.Any())
                {
                    foreach (var decision in decisionsUpcoming)
                    {
                        if (!string.IsNullOrWhiteSpace(decision.DealName))
                        {
                            var company = "";

                            if (!string.IsNullOrWhiteSpace(decision.CompanyName))
                            {
                                company = $"| Company: {decision.CompanyName}";
                            }

                            var text = $"{decision.DecisionDate.Value:ddd, dd-MMM-yy} - Decision Due for: {decision.DealName} {company}";

                            listUpcoming.Add(new Tuple<string, DateTime>(text, decision.DecisionDate.Value));
                        }
                    }
                }

                if (activitiesUpcoming != null && activitiesUpcoming.Any())
                {
                    foreach (var activity in activitiesUpcoming)
                    {
                        var company = "";
                        var contacts = "";
                        var deal = "";

                        if (!string.IsNullOrWhiteSpace(activity.ContactNames))
                        {
                            contacts = $"with {activity.ContactNames}";
                        }

                        if (!string.IsNullOrWhiteSpace(activity.CompanyName))
                        {
                            company = $"| Company: {activity.CompanyName}";
                        }

                        var dealName = GetDealNameFromActivity(globalUser.DataCenter, activity.ActivityId);

                        if (!string.IsNullOrWhiteSpace(dealName))
                        {
                            deal = $"| Deal: {dealName}";
                        }

                        var text = $"{activity.ActivityDate:ddd, dd-MMM-yy HH:mm} - {activity.CategoryName} {contacts} {company} {deal}";

                        listUpcoming.Add(new Tuple<string, DateTime>(text, activity.ActivityDate));
                    }
                }

                if (tasksUpcoming != null && tasksUpcoming.Any())
                {
                    foreach (var task in tasksUpcoming)
                    {
                        if (!string.IsNullOrWhiteSpace(task.TaskName))
                        {
                            var company = "";
                            var deal = "";

                            if (!string.IsNullOrWhiteSpace(task.CompanyName))
                            {
                                company = $"| Company: {task.CompanyName}";
                            }

                            var dealName = GetDealNameFromActivity(globalUser.DataCenter, task.ActivityId);

                            if (!string.IsNullOrWhiteSpace(dealName))
                            {
                                deal = $"| Deal: {dealName}";
                            }

                            var text = $"{task.DueDate.Value:ddd, dd-MMM-yy} - Task Due: {task.TaskName} {company} {deal}";

                            listUpcoming.Add(new Tuple<string, DateTime>(text, task.DueDate.Value));
                        }
                    }
                }

                if (birthdaysUpcoming != null && birthdaysUpcoming.Any())
                {
                    foreach (var birthday in birthdaysUpcoming)
                    {
                        if (!string.IsNullOrWhiteSpace(birthday.ContactName))
                        {
                            var company = "";

                            if (!string.IsNullOrWhiteSpace(birthday.CompanyName))
                            {
                                company = $"| Company: {birthday.CompanyName}";
                            }

                            var birthdayYear = dateFrom.Year;

                            if (birthday.BirthdayMonth == dateTo.Month)
                            {
                                birthdayYear = dateTo.Year;
                            }

                            var birthdayDate = new DateTime(birthdayYear, birthday.BirthdayMonth, birthday.BirthdayDay);

                            var text = $"{birthdayDate:ddd, dd-MMM-yy} - Birthday for: {birthday.ContactName} {company}";

                            listUpcoming.Add(new Tuple<string, DateTime>(text, birthdayDate));
                        }
                    }
                }

                if (proposalsOverdue != null && proposalsOverdue.Any())
                {
                    foreach (var proposal in proposalsOverdue)
                    {
                        if (!string.IsNullOrWhiteSpace(proposal.DealName))
                        {
                            var company = "";

                            if (!string.IsNullOrWhiteSpace(proposal.CompanyName))
                            {
                                company = $"| Company: {proposal.CompanyName}";
                            }

                            var text = $"{proposal.DateProposalDue.Value:ddd, dd-MMM-yy} - Proposal Past Due for: {proposal.DealName} {company}";

                            listOverdue.Add(new Tuple<string, DateTime>(text, proposal.DateProposalDue.Value));
                        }
                    }
                }

                if (contractsOverdue != null && contractsOverdue.Any())
                {
                    foreach (var contract in contractsOverdue)
                    {
                        if (!string.IsNullOrWhiteSpace(contract.DealName))
                        {
                            var company = "";

                            if (!string.IsNullOrWhiteSpace(contract.CompanyName))
                            {
                                company = $"| Company: {contract.CompanyName}";
                            }

                            var text = $"{contract.ContractEndDate.Value:ddd, dd-MMM-yy} - Contract Ended for: {contract.DealName} {company}";

                            listOverdue.Add(new Tuple<string, DateTime>(text, contract.ContractEndDate.Value));
                        }
                    }
                }

                if (decisionsOverdue != null && decisionsOverdue.Any())
                {
                    foreach (var decision in decisionsOverdue)
                    {
                        if (!string.IsNullOrWhiteSpace(decision.DealName))
                        {
                            var company = "";

                            if (!string.IsNullOrWhiteSpace(decision.CompanyName))
                            {
                                company = $"| Company: {decision.CompanyName}";
                            }

                            var text = $"{decision.DecisionDate.Value:ddd, dd-MMM-yy} - Decision Past Due for: {decision.DealName} {company}";

                            listOverdue.Add(new Tuple<string, DateTime>(text, decision.DecisionDate.Value));
                        }
                    }
                }

                if (tasksOverdue != null && tasksOverdue.Any())
                {
                    foreach (var task in tasksOverdue)
                    {
                        if (!string.IsNullOrWhiteSpace(task.TaskName))
                        {
                            var company = "";
                            var deal = "";

                            if (!string.IsNullOrWhiteSpace(task.CompanyName))
                            {
                                company = $"| Company: {task.CompanyName}";
                            }

                            var dealName = GetDealNameFromActivity(globalUser.DataCenter, task.ActivityId);

                            if (!string.IsNullOrWhiteSpace(dealName))
                            {
                                deal = $"| Deal: {dealName}";
                            }

                            var text = $"{task.DueDate.Value:ddd, dd-MMM-yy} - Task Past Due: {task.TaskName} {company} {deal}";

                            listOverdue.Add(new Tuple<string, DateTime>(text, task.DueDate.Value));
                        }
                    }
                }

                if (listUpcoming.Any() || listOverdue.Any())
                {
                    var stringBuilder = new StringBuilder("<html><body>");

                    if (listUpcoming.Any())
                    {
                        stringBuilder.Append("<b>UPCOMING THIS WEEK</b><hr>");

                        foreach (var text in listUpcoming.OrderBy(x => x.Item2))
                        {
                            stringBuilder.Append($"{text.Item1}</br>");
                        }
                    }

                    if (listOverdue.Any())
                    {
                        stringBuilder.Append("</br><b>PAST DUE</b><hr>");

                        foreach (var text in listOverdue.OrderBy(x => x.Item2))
                        {
                            stringBuilder.Append($"{text.Item1}</br>");
                        }
                    }

                    stringBuilder.Append("</body></html>");

                    SendEmail(stringBuilder.ToString(), globalUser.EmailAddress, globalUser.DataCenter);
                }
            }
        }

        private string GetDealNameFromActivity(string dataCenter, int activityId)
        {
            var sharedContext = new DbSharedDataContext(Utils.GetSharedConnection(dataCenter));
            var dealId = sharedContext.LinkActivityToDeals.FirstOrDefault(x => x.ActivityId == activityId)?.DealId;

            if (dealId > 0)
            {
                var deal = sharedContext.GlobalDeals.FirstOrDefault(x => x.DealId == dealId);

                if (deal != null)
                    return deal.DealName;
            }

            return "";
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
            var globalDeals = sharedContext.GlobalDeals.Where(x => globalDealIds.Contains(x.DealIdGlobal) && !x.Deleted && x.DateProposalDue >= dateFrom && x.DateProposalDue <= dateTo);

            if (globalDeals != null && globalDeals.Any())
            {
                return globalDeals.ToList();
            }

            return new List<GlobalDeal>();
        }

        private List<GlobalDeal> GetContracts(DateTime dateFrom, DateTime dateTo, List<int> globalDealIds, string dataCenter)
        {
            var sharedContext = new DbSharedDataContext(Utils.GetSharedConnectionForDataCenter(dataCenter));
            var globalDeals = sharedContext.GlobalDeals.Where(x => globalDealIds.Contains(x.DealIdGlobal) && !x.Deleted && x.ContractEndDate >= dateFrom && x.ContractEndDate <= dateTo);

            if (globalDeals != null && globalDeals.Any())
            {
                return globalDeals.ToList();
            }

            return new List<GlobalDeal>();
        }

        private List<GlobalDeal> GetDecisions(DateTime dateFrom, DateTime dateTo, List<int> globalDealIds, string dataCenter)
        {
            var sharedContext = new DbSharedDataContext(Utils.GetSharedConnectionForDataCenter(dataCenter));
            var globalDeals = sharedContext.GlobalDeals.Where(x => globalDealIds.Contains(x.DealIdGlobal) && !x.Deleted && x.DecisionDate >= dateFrom && x.DecisionDate <= dateTo);

            if (globalDeals != null && globalDeals.Any())
            {
                return globalDeals.ToList();
            }

            return new List<GlobalDeal>();
        }

        private List<Activity> GetActivities(DateTime dateFrom, DateTime dateTo, int globalUserId, string dataCenter)
        {
            var sharedContext = new DbSharedDataContext(Utils.GetSharedConnectionForDataCenter(dataCenter));
            var activities = sharedContext.Activities.Where(x => x.UserIdGlobal == globalUserId && !x.Deleted && x.ActivityDate >= dateFrom && x.ActivityDate <= dateTo && x.TaskId == 0 && !x.ActivityType.ToLower().Equals("task"));

            if (activities != null && activities.Any())
            {
                return activities.ToList();
            }

            return new List<Activity>();
        }

        private List<Activity> GetTasks(DateTime dateFrom, DateTime dateTo, int globalUserId, string dataCenter)
        {
            var sharedContext = new DbSharedDataContext(Utils.GetSharedConnectionForDataCenter(dataCenter));
            var tasks = sharedContext.Activities.Where(x => x.UserIdGlobal == globalUserId && !x.Deleted && x.DueDate >= dateFrom && x.DueDate <= dateTo && (x.TaskId > 0 || x.ActivityType.ToLower().Equals("task")));

            if (tasks != null && tasks.Any())
            {
                return tasks.ToList();
            }

            return new List<Activity>();
        }

        private List<Contact> GetBirthdays(DateTime dateFrom, DateTime dateTo, int userId, string dataCenter)
        {
            var listBirthdays = new List<Contact>();

            var context = new DbFirstFreightDataContext(Utils.GetConnectionForDataCenter(dataCenter));
            var contacts = context.Contacts.Where(x => !x.Deleted && x.ContactOwnerUserId == userId);

            if (contacts != null && contacts.Any())
            {
                foreach (var contact in contacts.ToList())
                {
                    if (contact.BirthdayMonth > 0 && contact.BirthdayMonth <= 12 && contact.BirthdayDay > 0 && contact.BirthdayDay <= 31 && Utils.IsBirthdayInRange(new DateTime(2000, contact.BirthdayMonth, contact.BirthdayDay, 0, 0, 0), dateFrom, dateTo))
                    {
                        listBirthdays.Add(contact);
                    }
                }
            }

            return listBirthdays;
        }

        private void SendEmail(string html, string toEmail, string dataCenter)
        {
            var CRM_AdminEmailSender =
                            new Recipient
                            {
                                EmailAddress = "admin@firstfreight.com",
                                Name = "First Freight CRM"
                            };

            var request = new SendEmailRequest
            {
                DataCenter = dataCenter,
                Sender = CRM_AdminEmailSender,
                Subject = "FirstFreight Weekly Digest",
                HtmlBody = html,
                OtherRecipients = new List<Recipient> {
                        new Recipient { EmailAddress = toEmail },
                        new Recipient { EmailAddress = "charles@firstfreight.com" },
                        // send copy of email to archive + dev
                        new Recipient {EmailAddress = "sendgrid@firstfreight.com" },
                    }
            };

            // send email
            new SendGridHelper().SendEmail(request);
        }
    }
}
