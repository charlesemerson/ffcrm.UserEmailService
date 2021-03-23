using ffcrm.UserEmailService.Helper;
using ffcrm.UserEmailService.Login;
using ffcrm.UserEmailService.Models;
using ffcrm.UserEmailService.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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

            var sharedDb = new DbSharedDataContext(Utils.GetSharedConnection());
            var activeSubscriberIds = sharedDb.Subscribers.Where(t => !t.Deleted && t.Active && t.SubscriberId != 0 && t.SubscriberId != 100).Select(t => t.SubscriberId).ToList();

            var loginDb = new DbLoginDataContext(Utils.GetLoginConnection());

            //var globalUsers = loginDb.GlobalUsers.Where(x => t.LoginEnabled && !t.Deleted && x.EmailDigest.ToLower() == "weekly").OrderBy(x => x.FullName);
            var globalUsers = loginDb.GlobalUsers.Where(t => activeSubscriberIds.Contains(t.SubscriberId) && t.LoginEnabled && !t.Deleted).OrderBy(x => x.FullName).ToList();

            var emailSentCount = 0;

            foreach (var globalUser in globalUsers)
            {
                var listUpcoming = new List<Tuple<GridItem, DateTime>>();
                var listOverdue = new List<Tuple<GridItem, DateTime>>();

                var globalDealIds = GetGlobalDealIdsForGlobalUser(globalUser.GlobalUserId, globalUser.DataCenter);

                var proposalsUpcoming = GetProposals(dateFrom, dateTo, globalDealIds, globalUser.DataCenter);
                var contractsUpcoming = GetContracts(dateFrom, dateTo, globalDealIds, globalUser.DataCenter);
                var decisionsUpcoming = GetDecisions(dateFrom, dateTo, globalDealIds, globalUser.DataCenter);
                var eventsUpcoming = GetEvents(dateFrom, dateTo, globalUser.GlobalUserId, globalUser.DataCenter);
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
                        listUpcoming.Add(new Tuple<GridItem, DateTime>(new GridItem
                        {
                            CellMobileDate = $"{proposal.DateProposalDue.Value:dd-MMM-yy}",
                            CellDay = $"{proposal.DateProposalDue.Value:ddd}",
                            CellDate = $"{proposal.DateProposalDue.Value:dd-MMM-yy}",
                            CellType = "Proposal Due",
                            CellCompany = proposal.CompanyName,
                            CellDetails = proposal.DealName
                        }, proposal.DateProposalDue.Value));
                    }
                }

                if (contractsUpcoming != null && contractsUpcoming.Any())
                {
                    foreach (var contract in contractsUpcoming)
                    {
                        listUpcoming.Add(new Tuple<GridItem, DateTime>(new GridItem
                        {
                            CellMobileDate = $"{contract.ContractEndDate.Value:dd-MMM-yy}",
                            CellDay = $"{contract.ContractEndDate.Value:ddd}",
                            CellDate = $"{contract.ContractEndDate.Value:dd-MMM-yy}",
                            CellType = "Contract Ending",
                            CellCompany = contract.CompanyName,
                            CellDetails = contract.DealName
                        }, contract.ContractEndDate.Value));
                    }
                }

                if (decisionsUpcoming != null && decisionsUpcoming.Any())
                {
                    foreach (var decision in decisionsUpcoming)
                    {
                        listUpcoming.Add(new Tuple<GridItem, DateTime>(new GridItem
                        {
                            CellMobileDate = $"{decision.DecisionDate.Value:dd-MMM-yy}",
                            CellDay = $"{decision.DecisionDate.Value:ddd}",
                            CellDate = $"{decision.DecisionDate.Value:dd-MMM-yy}",
                            CellType = "Decision Due",
                            CellCompany = decision.CompanyName,
                            CellDetails = decision.DealName
                        }, decision.DecisionDate.Value));
                    }
                }

                if (eventsUpcoming != null && eventsUpcoming.Any())
                {
                    foreach (var activity in eventsUpcoming)
                    {
                        var deal = "";
                        var dealName = GetDealNameFromActivity(globalUser.DataCenter, activity.ActivityId);
                        string time = activity.IsAllDay ? "" : activity.StartDateTime.Value.ToString("hh:mm tt");
                        listUpcoming.Add(new Tuple<GridItem, DateTime>(new GridItem
                        {
                            CellMobileDate = $"{activity.ActivityDate:dd-MMM-yy}<br/>{activity.ActivityDate:hh:mm tt}",
                            CellDay = $"{activity.ActivityDate:ddd}",
                            CellDate = $"{activity.ActivityDate:dd-MMM-yy}",
                            CellTime = $"{time}",
                            CellType = activity.CategoryName,
                            CellCompany = activity.CompanyName,
                            CellDetails = $"{activity.ContactNames}{(string.IsNullOrEmpty(activity.ContactNames) || string.IsNullOrEmpty(dealName) ? "" : " | ") + deal}"
                        }, activity.ActivityDate));
                    }
                }

                if (tasksUpcoming != null && tasksUpcoming.Any())
                {
                    foreach (var task in tasksUpcoming)
                    {
                        var dealName = GetDealNameFromActivity(globalUser.DataCenter, task.ActivityId);
                        listUpcoming.Add(new Tuple<GridItem, DateTime>(new GridItem
                        {
                            CellMobileDate = $"{task.DueDate.Value:dd-MMM-yy}",
                            CellDay = $"{task.DueDate.Value:ddd}",
                            CellDate = $"{task.DueDate.Value:dd-MMM-yy}",
                            CellType = "Task Due",
                            CellCompany = task.CompanyName,
                            CellDetails = $"{dealName}{(string.IsNullOrEmpty(dealName) || string.IsNullOrEmpty(task.TaskName) ? "" : " | ") + task.TaskName}",
                        }, task.DueDate.Value));
                    }
                }

                if (birthdaysUpcoming != null && birthdaysUpcoming.Any())
                {
                    foreach (var birthday in birthdaysUpcoming)
                    {
                        if (!string.IsNullOrWhiteSpace(birthday.ContactName))
                        {
                            var birthdayYear = dateFrom.Year;
                            if (birthday.BirthdayMonth == dateTo.Month) birthdayYear = dateTo.Year;
                            var birthdayDate = new DateTime(birthdayYear, birthday.BirthdayMonth, birthday.BirthdayDay);

                            listUpcoming.Add(new Tuple<GridItem, DateTime>(new GridItem
                            {
                                CellMobileDate = $"{birthdayDate:dd-MMM-yy}",
                                CellDay = $"{birthdayDate:ddd}",
                                CellDate = $"{birthdayDate:dd-MMM-yy}",
                                CellType = "Birthday",
                                CellCompany = birthday.CompanyName,
                                CellDetails = birthday.ContactName,
                            }, birthdayDate));
                        }
                    }
                }

                if (proposalsOverdue != null && proposalsOverdue.Any())
                {
                    foreach (var proposal in proposalsOverdue)
                    {
                        if (!string.IsNullOrWhiteSpace(proposal.DealName))
                        {
                            listOverdue.Add(new Tuple<GridItem, DateTime>(new GridItem
                            {
                                CellMobileDate = $"{proposal.DateProposalDue.Value:dd-MMM-yy}",
                                CellDay = $"{proposal.DateProposalDue.Value:ddd}",
                                CellDate = $"{proposal.DateProposalDue.Value:dd-MMM-yy}",
                                CellType = "Proposal Past Due",
                                CellCompany = proposal.CompanyName,
                                CellDetails = proposal.DealName,
                            }, proposal.DateProposalDue.Value));
                        }
                    }
                }

                if (contractsOverdue != null && contractsOverdue.Any())
                {
                    foreach (var contract in contractsOverdue)
                    {
                        if (!string.IsNullOrWhiteSpace(contract.DealName))
                        {
                            listOverdue.Add(new Tuple<GridItem, DateTime>(new GridItem
                            {
                                CellMobileDate = $"{contract.ContractEndDate.Value:dd-MMM-yy}",
                                CellDay = $"{contract.ContractEndDate.Value:ddd}",
                                CellDate = $"{contract.ContractEndDate.Value:dd-MMM-yy}",
                                CellType = "Contract Past Due",
                                CellCompany = contract.CompanyName,
                                CellDetails = contract.DealName,
                            }, contract.ContractEndDate.Value));
                        }
                    }
                }

                if (decisionsOverdue != null && decisionsOverdue.Any())
                {
                    foreach (var decision in decisionsOverdue)
                    {
                        if (!string.IsNullOrWhiteSpace(decision.DealName))
                        {
                            listOverdue.Add(new Tuple<GridItem, DateTime>(new GridItem
                            {
                                CellMobileDate = $"{decision.DecisionDate.Value:dd-MMM-yy}",
                                CellDay = $"{decision.DecisionDate.Value:ddd}",
                                CellDate = $"{decision.DecisionDate.Value:dd-MMM-yy}",
                                CellType = "Decision Past Due",
                                CellCompany = decision.CompanyName,
                                CellDetails = decision.DealName,
                            }, decision.DecisionDate.Value));
                        }
                    }
                }

                if (tasksOverdue != null && tasksOverdue.Any())
                {
                    foreach (var task in tasksOverdue)
                    {
                        if (!string.IsNullOrWhiteSpace(task.TaskName))
                        {
                            var dealName = GetDealNameFromActivity(globalUser.DataCenter, task.ActivityId);
                            listOverdue.Add(new Tuple<GridItem, DateTime>(new GridItem
                            {
                                CellMobileDate = $"{task.DueDate.Value:dd-MMM-yy}",
                                CellDay = $"{task.DueDate.Value:ddd}",
                                CellDate = $"{task.DueDate.Value:dd-MMM-yy}",
                                CellType = "Task Past Due",
                                CellCompany = task.CompanyName,
                                CellDetails = $"{dealName}{(string.IsNullOrEmpty(dealName) || string.IsNullOrEmpty(task.TaskName) ? "" : " | ") + task.TaskName}",
                            }, task.DueDate.Value));
                        }
                    }
                }

                if (listUpcoming.Any() || listOverdue.Any())
                {
                    var path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Templates\\WeeklyCRM\\email.html");
                    var html = File.ReadAllText(path);
                    var stringBuilderUpcoming = new StringBuilder();
                    var stringBuilderOverdue = new StringBuilder();

                    var count = 0;
                    var styles1 = "font-size: 13px; padding: 5px 10px 5px 0px; text-overflow: ellipsis; overflow: hidden; vertical-align: top; line-height: normal;";
                    if (listUpcoming.Any())
                    {
                        foreach (var text in listUpcoming.OrderBy(x => x.Item2))
                        {
                            count++;
                            stringBuilderUpcoming.Append("<tr style=\"background-color:" + ((count % 2 == 0) ? "transparent" : "#f7f9fa") + ";\">" +
                                "<td style=\"width: 34px;" + styles1 + $"padding-left: 10px;\">{text.Item1.CellDay}</td>" +
                                "<td style=\"width: 70px;" + styles1 + $"\">{text.Item1.CellMobileDate}</td>" +
                                "<td style=\"" + styles1 + $"\">{text.Item1.CellType}</td>" +
                                "<td style=\"" + styles1 + $"\">{text.Item1.CellCompany}</td>" +
                                "<td style=\"" + styles1 + $"\">{text.Item1.CellDetails}</td>" +
                             "</tr>");
                        }
                    }

                    count = 0;
                    if (listOverdue.Any())
                    {
                        foreach (var text in listOverdue.OrderBy(x => x.Item2))
                        {
                            count++;
                            stringBuilderOverdue.Append("<tr style=\"background-color:" + ((count % 2 == 0) ? "transparent" : "#f7f9fa") + ";\">" +
                                "<td style=\"width: 34px;" + styles1 + $"padding-left: 10px;\">{text.Item1.CellDay}</td>" +
                                "<td style=\"width: 70px;" + styles1 + $"\">{text.Item1.CellMobileDate}</td>" +
                                "<td style=\"" + styles1 + $"\">{text.Item1.CellType}</td>" +
                                "<td style=\"" + styles1 + $"\">{text.Item1.CellCompany}</td>" +
                                "<td style=\"" + styles1 + $"\">{text.Item1.CellDetails}</td>" +
                            "</tr>");
                        }
                    }

                    html = html.Replace("{dateFrom}", $"{dateFrom:ddd, dd-MMM-yy}");
                    html = html.Replace("{dateTo}", $"{dateTo:ddd, dd-MMM-yy}");
                    html = html.Replace("{upcomingTable}", stringBuilderUpcoming.ToString());
                    html = html.Replace("{pastDueTable}", stringBuilderOverdue.ToString());
                    html = html.Replace("{tableStyles1-upcoming}", listUpcoming.Any() ? "" : "display:none;");
                    html = html.Replace("{tableStyles2-upcoming}", listUpcoming.Any() ? "" : "display:none;");
                    html = html.Replace("{tablesVerticalSpacer}", listUpcoming.Any() && listOverdue.Any() ? "<br/><br/>" : "");
                    html = html.Replace("{tableStyles1-pastDue}", listOverdue.Any() ? "" : "display:none;");
                    html = html.Replace("{tableStyles2-pastDue}", listOverdue.Any() ? "" : "display:none;");

                    //SendEmail(html, "dean.martin@firstfreight0.onmicrosoft.com", globalUser.DataCenter);
                    //System.Diagnostics.Debug.WriteLine(html);
                    SendEmail(html, globalUser.EmailAddress, globalUser.DataCenter);
                    emailSentCount += 1;
                    if (emailSentCount == 20)
                    {
                        break;
                    }
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
            var sharedContext = new DbSharedDataContext(Utils.GetSharedConnectionForDataCenter(dataCenter));
            var globalDealIds = sharedContext.LinkGlobalDealGlobalUsers.Where(x => x.GlobalUserId == globalUserId).Select(x => x.GlobalDealId);

            if (globalDealIds != null && globalDealIds.Any())
            {
                return globalDealIds.ToList();
            }

            return new List<int>();
        }

        private List<GlobalDeal> GetProposals(DateTime dateFrom, DateTime dateTo, List<int> globalDealIds, string dataCenter)
        {
            var sharedContext = new DbSharedDataContext(Utils.GetSharedConnectionForDataCenter(dataCenter));
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

        private List<Activity> GetEvents(DateTime dateFrom, DateTime dateTo, int globalUserId, string dataCenter)
        {
            var sharedContext = new DbSharedDataContext(Utils.GetSharedConnectionForDataCenter(dataCenter));
            var activities = sharedContext.Activities.Where(x => x.UserIdGlobal == globalUserId && !x.Deleted && x.ActivityDate >= dateFrom && x.ActivityDate <= dateTo && (x.CalendarEventId > 0 || x.ActivityType.ToLower().Equals("event")));

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
                      //  new Recipient{  EmailAddress = toEmail  },
                      new Recipient{  EmailAddress = "charles@firstfreight.com"  },
                      new Recipient{  EmailAddress = "devseff01@gmail.com"  },
                        // send copy of email to archive + dev
                        // new Recipient{EmailAddress = "sendgrid@firstfreight.com" },
                    }
            };

            // send email
            new SendGridHelper().SendEmail(request);
        }
    }
}
