using ffcrm.UserEmailService.Shared;
using System;
using System.Linq;

namespace ffcrm.UserEmailService.Helper
{
    public class Logging
    {

        // Log User Activity
        //public void LogUserAction(UserActivity userAction, string connection = null)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(connection))
        //        {
        //            connection = new Utils().GetConnectionForDataCenter();
        //        }
        //        var context = new DbFirstFreightDataContext(connection);
        //        var user = context.Users.FirstOrDefault(t => t.UserId == userAction.UserId && t.SubscriberId == userAction.SubscriberId);
        //        if (user != null)
        //        {
        //            userAction.SubscriberId = user.SubscriberId;
        //            userAction.UserId = user.UserId;
        //            userAction.UserIdGlobal = user.UserIdGlobal;
        //            userAction.UserName = user.FirstName + " " + user.LastName;
        //        }
        //        if (userAction.UserIdGlobal > 0)
        //        {
        //            var loginConnection = new Utils().GetLoginConnection();
        //            var loginContext = new Login.DbLoginDataContext(loginConnection);
        //            var gUser = loginContext.GlobalUsers.FirstOrDefault(t => t.GlobalUserId == userAction.UserIdGlobal);
        //            if (gUser != null)
        //            {
        //                userAction.SubscriberId = gUser.SubscriberId;
        //                userAction.UserId = gUser.UserId;
        //                userAction.UserIdGlobal = gUser.GlobalUserId;
        //                userAction.UserName = gUser.FullName;
        //            }
        //        }
        //        var subscriber = context.Subscribers.FirstOrDefault(t => t.SubscriberId == userAction.SubscriberId);
        //        if (user != null && subscriber != null)
        //        {
        //            var newUserAction = new UserActivity
        //            {
        //                CalendarEventId = userAction.CalendarEventId,
        //                CalendarEventSubject = userAction.CalendarEventSubject,
        //                CompanyId = userAction.CompanyId,
        //                CompanyName = userAction.CompanyName,
        //                ContactId = userAction.ContactId,
        //                ContactName = userAction.ContactName,
        //                LogData = userAction.LogData,
        //                DealId = userAction.DealId,
        //                DealName = userAction.DealName,
        //                FilterId = userAction.FilterId,
        //                FilterName = userAction.FilterName,
        //                NoteContent = userAction.NoteContent,
        //                NoteId = userAction.NoteId,
        //                SubscriberId = userAction.SubscriberId,
        //                SubscriberName = subscriber.CompanyName,
        //                TaskId = userAction.TaskId,
        //                TaskName = userAction.TaskName,
        //                UserActivityMessage = userAction.UserActivityMessage,
        //                UserId = userAction.UserId,
        //                UserName = userAction.UserName,
        //                UserIdGlobal = userAction.UserIdGlobal,
        //                UserActivityTimestamp = DateTime.UtcNow
        //            };
        //            context.UserActivities.InsertOnSubmit(newUserAction);
        //            context.SubmitChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}


        //// Log Errors
        //public int LogWebAppError(WebAppError webAppError)
        //{
        //    try
        //    {
        //        var sharedWriteableConnnection = new Utils().GetSharedConnection();
        //        var sharedWriteableContext = new DbSharedDataContext(sharedWriteableConnnection);

        //        var webAppErrorDetail = new Shared.WebAppError()
        //        {
        //            ErrorCallStack = webAppError.ErrorCallStack ?? "",
        //            ErrorCode = "",
        //            ErrorMessage = webAppError.ErrorMessage ?? "",
        //            PageCalledFrom = webAppError.PageCalledFrom ?? "",
        //            RoutineName = webAppError.RoutineName ?? "",
        //            UserId = webAppError.UserId,
        //            ErrorDateTime = DateTime.UtcNow
        //        };

        //        webAppErrorDetail.SubscriberId = webAppError.SubscriberId;
        //        webAppErrorDetail.SubscriberName = webAppError.SubscriberName;
        //        webAppErrorDetail.UserName = webAppError.UserName;

        //        sharedWriteableContext.WebAppErrors.InsertOnSubmit(webAppErrorDetail);
        //        sharedWriteableContext.SubmitChanges();

        //        // return the webAppError id
        //        return webAppErrorDetail.WebAppErrorId;
        //    }
        //    catch (Exception e)
        //    {
        //        return 0;
        //    }

        //}

    }
}
