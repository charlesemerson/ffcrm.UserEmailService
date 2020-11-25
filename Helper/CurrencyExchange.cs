using ffcrm.UserEmailService.Model;
using ffcrm.UserEmailService.Shared;
using System;
using System.Linq;

namespace ffcrm.UserEmailService.Helper
{
    public class CurrencyExchange
    {

        public double GetCurrencyExchangeRate(string currencyCode, DateTime? exchangeDateTime, Session session)
        {
            double exchangeRate = 0;
            // Must Have SubscriberId
            if (session.SubscriberId <= 0) return exchangeRate;
            // Must Have currencyCode
            if (currencyCode != null)
            {
                try
                {
                    if (exchangeDateTime != null)
                    {
                        var sharedContext = new DbSharedDataContext(new Utils().GetSharedConnection());

                        var exchangeRates = sharedContext.CurrencyExchangeRates
                            .Where(x => x.CurrencyCode.ToLower() == currencyCode &&
                            x.ExchangeDate.Value.ToString() == exchangeDateTime.Value.ToString("yyyy-MM-dd") + " 00:00:01");

                        exchangeRate = exchangeRates.Any() ? Util.NullToZero(exchangeRates.First().ExchangeRate) : GetMostRecentCurrencyExchangeRate(currencyCode, session);
                    }
                    else
                    {
                        // exchangeDateTime Is Null
                        exchangeRate = GetMostRecentCurrencyExchangeRate(currencyCode, session);
                    }
                }
                catch (Exception ex)
                {
                    // Log Error
                    new Logging().LogWebAppError(new WebAppError
                    {
                        ErrorCode = "201",
                        ErrorCallStack = ex.StackTrace,
                        ErrorDateTime = DateTime.UtcNow,
                        RoutineName = "GetCurrencyExchangeRate",
                        SubscriberId = session.SubscriberId,
                        ErrorMessage = ex.Message.ToString(),
                        UserId = session.UserId
                    });

                    Console.WriteLine(ex);
                }
            }

            return exchangeRate;
        }

        public double GetCalculatedCurrencyExchangeValue(string sourceCurrencyCode, string targetCurrencyCode, double amount, DateTime? exchangeDate, Session session)
        {
            double dblCalculatedAmount = 0;
            if (!(!string.IsNullOrEmpty(sourceCurrencyCode) & !string.IsNullOrEmpty(targetCurrencyCode)))
                return dblCalculatedAmount;
            if (session.SubscriberId <= 0) return dblCalculatedAmount;
            if (exchangeDate == null)
            {
                exchangeDate = DateTime.Now;
            }
            double dblSourceExchangeRate = 0;
            double dblTargetExchangeRate = 0;
            double sourceAmountUsd = 0;
            try
            {
                // Get Source and Target Exchange Rates on the Exchange Date
                dblSourceExchangeRate = GetCurrencyExchangeRate(sourceCurrencyCode, exchangeDate, session);
                dblTargetExchangeRate = GetCurrencyExchangeRate(targetCurrencyCode, exchangeDate, session);
                // Translate Source Amount to USD
                sourceAmountUsd = amount / dblSourceExchangeRate;
                // Round to Four decimals
                sourceAmountUsd = Math.Round(sourceAmountUsd, 4);
                // Translate USD Amount to Target Currency
                dblCalculatedAmount = sourceAmountUsd * dblTargetExchangeRate;
                // Round to Four decimals
                dblCalculatedAmount = Math.Round(dblCalculatedAmount, 4);
            }
            catch (Exception ex)
            {
                // Log Error
                new Logging().LogWebAppError(new WebAppError
                {
                    ErrorCode = "201",
                    ErrorCallStack = ex.StackTrace,
                    ErrorDateTime = DateTime.UtcNow,
                    RoutineName = "GetCalculatedCurrencyExchangeValue",
                    SubscriberId = session.SubscriberId,
                    ErrorMessage = ex.Message + "Source Xchg rate: " + dblSourceExchangeRate + " Target Xchg rate: " + dblTargetExchangeRate + " Amount: " + sourceAmountUsd,
                    UserId = session.UserId
                });
                
                Console.WriteLine(ex);
            }
            return dblCalculatedAmount;
        }

        public double GetMostRecentCurrencyExchangeRate(string currencyCode, Session session)
        {
            double exchangeRate = 0;
            if (string.IsNullOrEmpty(currencyCode)) return exchangeRate;
            if (session.SubscriberId <= 0) return exchangeRate;
            try
            {
                var sharedContext = new DbSharedDataContext(new Utils().GetSharedConnection());

                var exchangeRates = sharedContext.CurrencyExchangeRates
                    .Where(x => x.CurrencyCode.ToLower() == currencyCode).OrderByDescending(x => x.ExchangeDate);

                if (exchangeRates.Any())
                {
                    // Most Recent Exchange Rate for CurrencyCode
                    exchangeRate = exchangeRates.First().ExchangeRate ?? 0;
                }
            }
            catch (Exception ex)
            {
                // Log Error
                new Logging().LogWebAppError(new WebAppError
                {
                    ErrorCode = "201",
                    ErrorCallStack = ex.StackTrace,
                    ErrorDateTime = DateTime.UtcNow,
                    RoutineName = "GetMostRecentCurrencyExchangeRate",
                    SubscriberId = session.SubscriberId,
                    ErrorMessage = ex.Message,
                    UserId = session.UserId
                });

                Console.WriteLine(ex);
            }
            return exchangeRate;
        }

    }
}
