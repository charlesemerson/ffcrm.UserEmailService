﻿using ffcrm.UserEmailService.Helper;
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
                            Cell1 = $"{proposal.DateProposalDue.Value:ddd dd-MMM-yy}",
                            Cell3 = "Proposal Due",
                            Cell4 = proposal.CompanyName,
                            Cell5 = proposal.DealName
                        }, proposal.DateProposalDue.Value));
                    }
                }

                if (contractsUpcoming != null && contractsUpcoming.Any())
                {
                    foreach (var contract in contractsUpcoming)
                    {
                        listUpcoming.Add(new Tuple<GridItem, DateTime>(new GridItem
                        {
                            Cell1 = $"{contract.ContractEndDate.Value:ddd dd-MMM-yy}",
                            Cell3 = "Contract Ending",
                            Cell4 = contract.CompanyName,
                            Cell5 = contract.DealName
                        }, contract.ContractEndDate.Value));
                    }
                }

                if (decisionsUpcoming != null && decisionsUpcoming.Any())
                {
                    foreach (var decision in decisionsUpcoming)
                    {
                        listUpcoming.Add(new Tuple<GridItem, DateTime>(new GridItem
                        {
                            Cell1 = $"{decision.DecisionDate.Value:ddd dd-MMM-yy}",
                            Cell3 = "Decision Due",
                            Cell4 = decision.CompanyName,
                            Cell5 = decision.DealName
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
                            Cell1 = $"{activity.ActivityDate:ddd dd-MMM-yy}",
                            Cell2 = $"{activity.ActivityDate:HH:mm}",
                            Cell3 = activity.CategoryName,
                            Cell4 = activity.CompanyName,
                            Cell5 = $"{activity.ContactNames} {deal}"
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
                            Cell1 = $"{task.DueDate.Value:ddd dd-MMM-yy}",
                            Cell3 = "Task Due",
                            Cell4 = task.CompanyName,
                            Cell5 = $"{dealName}{task.TaskName}",
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
                                Cell1 = $"{birthdayDate:ddd dd-MMM-yy}",
                                Cell3 = "Birthday",
                                Cell4 = birthday.CompanyName,
                                Cell5 = birthday.ContactName,
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
                                Cell1 = $"{proposal.DateProposalDue.Value:ddd dd-MMM-yy}",
                                Cell3 = "Proposal Past Due",
                                Cell4 = proposal.CompanyName,
                                Cell5 = proposal.DealName,
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
                                Cell1 = $"{contract.ContractEndDate.Value:ddd dd-MMM-yy}",
                                Cell3 = "Contract Past Due",
                                Cell4 = contract.CompanyName,
                                Cell5 = contract.DealName,
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
                                Cell1 = $"{decision.DecisionDate.Value:ddd dd-MMM-yy}",
                                Cell3 = "Decision Past Due",
                                Cell4 = decision.CompanyName,
                                Cell5 = decision.DealName,
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
                                Cell1 = $"{task.DueDate.Value:ddd dd-MMM-yy}",
                                Cell3 = "Task Past Due",
                                Cell4 = task.CompanyName,
                                Cell5 = $"{dealName}{task.TaskName}",
                            }, task.DueDate.Value));
                        }
                    }
                }

                if (listUpcoming.Any() || listOverdue.Any())
                {
                    var html = "<!doctype html><html xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\"><head><title></title><!--[if !mso]><!-- --><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"><!--<![endif]--><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"><style type=\"text/css\">#outlook a { padding:0; } body { margin:0;padding:0;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%; } table, td { border-collapse:collapse;mso-table-lspace:0pt;mso-table-rspace:0pt; } img { border:0;height:auto;line-height:100%; outline:none;text-decoration:none;-ms-interpolation-mode:bicubic; } p { display:block;margin:13px 0; }</style><!--[if mso]> <xml> <o:OfficeDocumentSettings> <o:AllowPNG/> <o:PixelsPerInch>96</o:PixelsPerInch> </o:OfficeDocumentSettings> </xml> <![endif]--><!--[if lte mso 11]> <style type=\"text/css\"> .mj-outlook-group-fix { width:100% !important; } </style> <![endif]--><!--[if !mso]><!--><link href=\"https://fonts.googleapis.com/css?family=Ubuntu:300,400,500,700\" rel=\"stylesheet\" type=\"text/css\"><style type=\"text/css\">@import url(https://fonts.googleapis.com/css?family=Ubuntu:300,400,500,700);</style><!--<![endif]--><style type=\"text/css\">@media only screen and (min-width:480px) { .mj-column-per-100 { width:100% !important; max-width: 100%; } .mj-column-per-50 { width:50% !important; max-width: 50%; } }</style><style type=\"text/css\"></style></head><body><div><!--[if mso | IE]><table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"\" style=\"width:600px;\" width=\"600\" ><tr><td style=\"line-height:0px;font-size:0px;mso-line-height-rule:exactly;\"><![endif]--><div style=\"margin:0px auto;max-width:600px;\"><table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"width:100%;\"><tbody><tr><td style=\"direction:ltr;font-size:0px;padding:20px 0;text-align:center;\"><!--[if mso | IE]><table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td class=\"\" style=\"vertical-align:top;width:600px;\" ><![endif]--><div class=\"mj-column-per-100 mj-outlook-group-fix\" style=\"font-size:0px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"vertical-align:top;\" width=\"100%\"><tr><td align=\"left\" style=\"font-size:0px;padding:10px 25px;word-break:break-word;\"><div style=\"font-family:helvetica;font-size:20px;font-weight:bold;line-height:1;text-align:left;color:#000000;\">Weekly CRM Email</div></td></tr></table></div><!--[if mso | IE]></td></tr></table><![endif]--></td></tr></tbody></table></div><!--[if mso | IE]></td></tr></table><![endif]--><table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"background:#C6E0B4;background-color:#C6E0B4;width:100%;\"><tbody><tr><td><!--[if mso | IE]><table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"\" style=\"width:600px;\" width=\"600\" ><tr><td style=\"line-height:0px;font-size:0px;mso-line-height-rule:exactly;\"><![endif]--><div style=\"margin:0px auto;max-width:600px;\"><table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"width:100%;\"><tbody><tr><td style=\"direction:ltr;font-size:0px;padding:20px 0;text-align:center;\"><!--[if mso | IE]><table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td class=\"\" style=\"vertical-align:top;width:300px;\" ><![endif]--><div class=\"mj-column-per-50 mj-outlook-group-fix\" style=\"font-size:0px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"vertical-align:top;\" width=\"100%\"><tr><td align=\"left\" style=\"font-size:0px;padding:10px 25px;word-break:break-word;\"><div style=\"font-family:helvetica;font-size:20px;font-weight:bold;line-height:1;text-align:left;color:#000000;\">UPCOMING THIS WEEK</div></td></tr></table></div><!--[if mso | IE]></td><td class=\"\" style=\"vertical-align:top;width:300px;\" ><![endif]--><div class=\"mj-column-per-50 mj-outlook-group-fix\" style=\"font-size:0px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"vertical-align:top;\" width=\"100%\"><tr><td align=\"left\" style=\"font-size:0px;padding:10px 25px;word-break:break-word;\"><div style=\"font-family:helvetica;font-size:20px;font-weight:bold;line-height:1;text-align:left;color:#000000;\">{dateFrom} thru {dateTo}</div></td></tr></table></div><!--[if mso | IE]></td></tr></table><![endif]--></td></tr></tbody></table></div><!--[if mso | IE]></td></tr></table><![endif]--></td></tr></tbody></table><table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"width:100%;\"><tbody><tr><td><!--[if mso | IE]><table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"\" style=\"width:600px;\" width=\"600\" ><tr><td style=\"line-height:0px;font-size:0px;mso-line-height-rule:exactly;\"><![endif]--><div style=\"margin:0px auto;max-width:600px;\"><table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"width:100%;\"><tbody><tr><td style=\"direction:ltr;font-size:0px;padding:20px 0;text-align:center;\"><!--[if mso | IE]><table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td class=\"\" style=\"vertical-align:top;width:600px;\" ><![endif]--><div class=\"mj-column-per-100 mj-outlook-group-fix\" style=\"font-size:0px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"vertical-align:top;\" width=\"100%\"><tr><td align=\"left\" style=\"font-size:0px;padding:10px 25px;word-break:break-word;\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\" style=\"color:#000000;font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:13px;line-height:22px;table-layout:auto;width:100%;border:none;\">{upcomingTable}</table></div><!--[if mso | IE]></td></tr></table><![endif]--></td></tr></tbody></table></div><!--[if mso | IE]></td></tr></table><![endif]--></td></tr></tbody></table><table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"background:#FFABAB;background-color:#FFABAB;width:100%;\"><tbody><tr><td><!--[if mso | IE]><table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"\" style=\"width:600px;\" width=\"600\" ><tr><td style=\"line-height:0px;font-size:0px;mso-line-height-rule:exactly;\"><![endif]--><div style=\"margin:0px auto;max-width:600px;\"><table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"width:100%;\"><tbody><tr><td style=\"direction:ltr;font-size:0px;padding:20px 0;text-align:center;\"><!--[if mso | IE]><table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td class=\"\" style=\"vertical-align:top;width:600px;\" ><![endif]--><div class=\"mj-column-per-100 mj-outlook-group-fix\" style=\"font-size:0px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"vertical-align:top;\" width=\"100%\"><tr><td align=\"left\" style=\"font-size:0px;padding:10px 25px;word-break:break-word;\"><div style=\"font-family:helvetica;font-size:20px;font-weight:bold;line-height:1;text-align:left;color:#000000;\">PAST DUE</div></td></tr></table></div><!--[if mso | IE]></td></tr></table><![endif]--></td></tr></tbody></table></div><!--[if mso | IE]></td></tr></table><![endif]--></td></tr></tbody></table><table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"width:100%;\"><tbody><tr><td><!--[if mso | IE]><table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"\" style=\"width:600px;\" width=\"600\" ><tr><td style=\"line-height:0px;font-size:0px;mso-line-height-rule:exactly;\"><![endif]--><div style=\"margin:0px auto;max-width:600px;\"><table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"width:100%;\"><tbody><tr><td style=\"direction:ltr;font-size:0px;padding:20px 0;text-align:center;\"><!--[if mso | IE]><table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td class=\"\" style=\"vertical-align:top;width:600px;\" ><![endif]--><div class=\"mj-column-per-100 mj-outlook-group-fix\" style=\"font-size:0px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"vertical-align:top;\" width=\"100%\"><tr><td align=\"left\" style=\"font-size:0px;padding:10px 25px;word-break:break-word;\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\" style=\"color:#000000;font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:13px;line-height:22px;table-layout:auto;width:100%;border:none;\">{pastDueTable}</table><![endif]--></td></tr></tbody></table></div><!--[if mso | IE]></td></tr></table><![endif]--></td></tr></tbody></table></div></body></html>";

                    var stringBuilderUpcoming = new StringBuilder();
                    var stringBuilderOverdue = new StringBuilder();

                    if (listUpcoming.Any())
                    {
                        foreach (var text in listUpcoming.OrderBy(x => x.Item2))
                        {
                            stringBuilderUpcoming.Append($"<tr><td width=\"80\">{text.Item1.Cell1}</td><td width=\"40\">{text.Item1.Cell2}</td><td>{text.Item1.Cell3}</td><td>{text.Item1.Cell4}</td><td>{text.Item1.Cell5}</td></tr>");
                        }
                    }

                    if (listOverdue.Any())
                    {
                        foreach (var text in listOverdue.OrderBy(x => x.Item2))
                        {
                            stringBuilderOverdue.Append($"<tr><td width=\"80\">{text.Item1.Cell1}</td><td width=\"40\">{text.Item1.Cell2}</td><td>{text.Item1.Cell3}</td><td>{text.Item1.Cell4}</td><td>{text.Item1.Cell5}</td></tr>");
                        }
                    }

                    html = html.Replace("{dateFrom}", $"{dateFrom:ddd, dd-MMM-yy}");
                    html = html.Replace("{dateTo}", $"{dateTo:ddd, dd-MMM-yy}");
                    html = html.Replace("{upcomingTable}", stringBuilderUpcoming.ToString());
                    html = html.Replace("{pastDueTable}", stringBuilderOverdue.ToString());

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
