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

            var loginDb = new DbLoginDataContext(Utils.GetLoginConnection());

            var globalUsers = loginDb.GlobalUsers.Where(x => x.EmailDigest.ToLower() == "weekly").OrderBy(x => x.FullName);

            foreach (var globalUser in globalUsers)
            {
                var listUpcoming = new List<Tuple<GridItem, DateTime>>();
                var listOverdue = new List<Tuple<GridItem, DateTime>>();

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
                        listUpcoming.Add(new Tuple<GridItem, DateTime>(new GridItem
                        {
                            CellMobileDate = $"{proposal.DateProposalDue.Value:dd-MMM-yy}<br/>{proposal.DateProposalDue.Value:hh:mm tt}",
                            CellDay = $"{proposal.DateProposalDue.Value:ddd}",
                            CellDate = $"{proposal.DateProposalDue.Value:dd-MMM-yy}",
                            CellTime = $"{proposal.DateProposalDue.Value:hh:mm tt}",
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
                            CellMobileDate = $"{contract.ContractEndDate.Value:dd-MMM-yy}<br/>{contract.ContractEndDate.Value:hh:mm tt}",
                            CellDay = $"{contract.ContractEndDate.Value:ddd}",
                            CellDate = $"{contract.ContractEndDate.Value:dd-MMM-yy}",
                            CellTime = $"{contract.ContractEndDate.Value:hh:mm tt}",
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
                            CellMobileDate = $"{decision.DecisionDate.Value:dd-MMM-yy}<br/>{decision.DecisionDate.Value:hh:mm tt}",
                            CellDay = $"{decision.DecisionDate.Value:ddd}",
                            CellDate = $"{decision.DecisionDate.Value:dd-MMM-yy}",
                            CellTime = $"{decision.DecisionDate.Value:hh:mm tt}",
                            CellType = "Decision Due",
                            CellCompany = decision.CompanyName,
                            CellDetails = decision.DealName
                        }, decision.DecisionDate.Value));
                    }
                }

                if (activitiesUpcoming != null && activitiesUpcoming.Any())
                {
                    foreach (var activity in activitiesUpcoming)
                    {
                        var deal = "";

                        var dealName = GetDealNameFromActivity(globalUser.DataCenter, activity.ActivityId);

                        if (!string.IsNullOrWhiteSpace(dealName))
                        {
                            deal = $"| {dealName}";
                        }

                        listUpcoming.Add(new Tuple<GridItem, DateTime>(new GridItem
                        {
                            CellMobileDate = $"{activity.ActivityDate:dd-MMM-yy}<br/>{activity.ActivityDate:hh:mm tt}",
                            CellDay = $"{activity.ActivityDate:ddd}",
                            CellDate = $"{activity.ActivityDate:dd-MMM-yy}",
                            CellTime = $"{activity.ActivityDate:hh:mm tt}",
                            CellType = activity.CategoryName,
                            CellCompany = activity.CompanyName,
                            CellDetails = $"{activity.ContactNames} {deal}"
                        }, activity.ActivityDate));
                    }
                }

                if (tasksUpcoming != null && tasksUpcoming.Any())
                {
                    foreach (var task in tasksUpcoming)
                    {
                        var dealName = GetDealNameFromActivity(globalUser.DataCenter, task.ActivityId);

                        if (!string.IsNullOrWhiteSpace(dealName))
                            dealName = $"{dealName} | ";

                        listUpcoming.Add(new Tuple<GridItem, DateTime>(new GridItem
                        {
                            CellMobileDate = $"{task.DueDate.Value:dd-MMM-yy}<br/>{task.DueDate.Value:hh:mm tt}",
                            CellDay = $"{task.DueDate.Value:ddd}",
                            CellDate = $"{task.DueDate.Value:dd-MMM-yy}",
                            CellTime = $"{task.DueDate.Value:hh:mm tt}",
                            CellType = "Task Due",
                            CellCompany = task.CompanyName,
                            CellDetails = $"{dealName}{task.TaskName}",
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

                            if (birthday.BirthdayMonth == dateTo.Month)
                            {
                                birthdayYear = dateTo.Year;
                            }

                            var birthdayDate = new DateTime(birthdayYear, birthday.BirthdayMonth, birthday.BirthdayDay);

                            listUpcoming.Add(new Tuple<GridItem, DateTime>(new GridItem
                            {
                                CellMobileDate = $"{birthdayDate:dd-MMM-yy}<br/>{birthdayDate:hh:mm tt}",
                                CellDay = $"{birthdayDate:ddd}",
                                CellDate = $"{birthdayDate:dd-MMM-yy}",
                                CellTime = $"{birthdayDate:hh:mm tt}",
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
                                CellMobileDate = $"{proposal.DateProposalDue.Value:dd-MMM-yy}<br/>{proposal.DateProposalDue.Value:hh:mm tt}",
                                CellDay = $"{proposal.DateProposalDue.Value:ddd}",
                                CellDate = $"{proposal.DateProposalDue.Value:dd-MMM-yy}",
                                CellTime = $"{proposal.DateProposalDue.Value:hh:mm tt}",
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
                                CellMobileDate = $"{contract.ContractEndDate.Value:dd-MMM-yy}<br/>{contract.ContractEndDate.Value:hh:mm tt}",
                                CellDay = $"{contract.ContractEndDate.Value:ddd}",
                                CellDate = $"{contract.ContractEndDate.Value:dd-MMM-yy}",
                                CellTime = $"{contract.ContractEndDate.Value:hh:mm tt}",
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
                                CellMobileDate = $"{decision.DecisionDate.Value:dd-MMM-yy}<br/>{decision.DecisionDate.Value:hh:mm tt}",
                                CellDay = $"{decision.DecisionDate.Value:ddd}",
                                CellDate = $"{decision.DecisionDate.Value:dd-MMM-yy}",
                                CellTime = $"{decision.DecisionDate.Value:hh:mm tt}",
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

                            if (!string.IsNullOrWhiteSpace(dealName))
                                dealName = $"{dealName} | ";

                            listOverdue.Add(new Tuple<GridItem, DateTime>(new GridItem
                            {
                                CellMobileDate = $"{task.DueDate.Value:dd-MMM-yy}<br/>{task.DueDate.Value:hh:mm tt}",
                                CellDay = $"{task.DueDate.Value:ddd}",
                                CellDate = $"{task.DueDate.Value:dd-MMM-yy}",
                                CellTime = $"{task.DueDate.Value:hh:mm tt}",
                                CellType = "Task Past Due",
                                CellCompany = task.CompanyName,
                                CellDetails = $"{dealName}{task.TaskName}",
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

                    if (listUpcoming.Any())
                    {
                        foreach (var text in listUpcoming.OrderBy(x => x.Item2))
                        {
                            stringBuilderUpcoming.Append("<tr>" +
                                $"<td class=\"data-cell show-mobile\">{text.Item1.CellMobileDate}</td>" +
                                $"<td class=\"data-cell hide-mobile\">{text.Item1.CellDay}</td>" +
                                $"<td class=\"data-cell hide-mobile\">{text.Item1.CellDate}</td>" +
                                $"<td class=\"data-cell hide-mobile\">{text.Item1.CellTime}</td>" +
                                $"<td class=\"data-cell\">{text.Item1.CellType}</td>" +
                                $"<td class=\"data-cell\">{text.Item1.CellCompany}</td>" +
                                $"<td class=\"data-cell\">{text.Item1.CellDetails}</td>" +
                             "</tr>");
                        }
                    }

                    if (listOverdue.Any())
                    {
                        foreach (var text in listOverdue.OrderBy(x => x.Item2))
                        {
                            stringBuilderOverdue.Append($"<tr>" +
                                $"<td class=\"data-cell show-mobile\">{text.Item1.CellMobileDate}</td>" +
                                $"<td class=\"data-cell hide-mobile\">{text.Item1.CellDay}</td>" +
                                $"<td class=\"data-cell hide-mobile\">{text.Item1.CellDate}</td>" +
                                $"<td class=\"data-cell hide-mobile\">{text.Item1.CellTime}</td>" +
                                $"<td class=\"data-cell\">{text.Item1.CellType}</td>" +
                                $"<td class=\"data-cell\">{text.Item1.CellCompany}</td>" +
                                $"<td class=\"data-cell\">{text.Item1.CellDetails}</td>" +
                            "</tr>");
                        }
                    }

                    html = html.Replace("{dateFrom}", $"{dateFrom:ddd, dd-MMM-yy}");
                    html = html.Replace("{dateTo}", $"{dateTo:ddd, dd-MMM-yy}");
                    html = html.Replace("{upcomingTable}", stringBuilderUpcoming.ToString());
                    html = html.Replace("{pastDueTable}", stringBuilderOverdue.ToString());

                    if (!listUpcoming.Any() || !listOverdue.Any()) {
                        var addStyles = "<style>";
                        if (!listUpcoming.Any()) addStyles += ".section-content-upcoming, .section-head-upcoming{display:none !important;}";
                        if (!listOverdue.Any()) addStyles += ".section-content-past-due, .section-head-past-due{display:none !important;}";
                        addStyles += "</style>";
                        html = html.Replace("<!-- {addStyles} -->", addStyles);
                    }

                    SendEmail(html, globalUser.EmailAddress, globalUser.DataCenter);
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
                    new Recipient {EmailAddress = "sendgrid@firstfreight.com" }
                }
            };

            // send email
            new SendGridHelper().SendEmail(request);
        }
    }
}

//Template
/*
 <mjml>
  <mj-body>
    <mj-section>
      <mj-column>
        <mj-text font-size="20px" font-weight="bold" font-family="helvetica">Weekly CRM Email</mj-text>
      </mj-column>
    </mj-section>
    <mj-section background-color="#C6E0B4" full-width="full-width">
      <mj-column width="50%">
        <mj-text font-size="20px" font-weight="bold" font-family="helvetica">UPCOMING THIS WEEK</mj-text>
      </mj-column>
      <mj-column width="50%">
        <mj-text font-size="20px" font-weight="bold" font-family="helvetica">{dateFrom} thru {dateTo}</mj-text>
      </mj-column>
    </mj-section>
    <mj-section full-width="full-width">
      <mj-column>
        <mj-table>
          <tr>
            <td width="80">Fri 23-Oct-20</td>
            <td width="40">09:00</td>
            <td>Proposal Past Due</td>
            <td>New Company Name bbwp4884</td>
            <td>New Deal njwmg | Task name for edit</td>
            <td>Maersk</td>
          </tr>
        </mj-table>
      </mj-column>
    </mj-section>
      <mj-section background-color="#FFABAB" full-width="full-width">
      <mj-column>
        <mj-text font-size="20px" font-weight="bold" font-family="helvetica">PAST DUE</mj-text>
      </mj-column>
    </mj-section>
    <mj-section full-width="full-width">
      <mj-column>
        <mj-table>
          <tr>
            <td>Mon</td>
            <td>04-Jan-21</td>
            <td>09:00</td>
            <td>Video Call</td>
            <td>Maersk</td>
            <td>Testing 123</td>
          </tr>
          <tr>
            <td>Mon</td>
            <td>04-Jan-21</td>
            <td>09:00</td>
            <td>Video Call</td>
            <td>Maersk</td>
            <td>Testing 123</td>
          </tr>
        </mj-table>
      </mj-column>
    </mj-section>
  </mj-body>
</mjml>*/
