using ffcrm.UserEmailService.Shared;
using System.Linq;

namespace ffcrm.UserEmailService.Helper
{
    public class Currencies
    {
        public string GetCurrencySymbolFromCode(string currencyCode)
        {
            var sharedContext = new DbSharedDataContext(new Utils().GetSharedConnection());

            var currency = sharedContext.Currencies.Where(x => x.CurrencyCode.ToLower() == currencyCode).FirstOrDefault();

            if (currency != null)
            {
                return currency.CurrencySymbol;
            }

            return "";
        }

        public Currency GetCurrencyDetailsFromCode(string currencyCode)
        {
            if (!string.IsNullOrEmpty(currencyCode))
            {
                var sharedContext = new DbSharedDataContext(new Utils().GetSharedConnection());

                var currency = sharedContext.Currencies.Where(x => x.CurrencyCode.ToLower() == currencyCode).FirstOrDefault();

                if (currency != null)
                {
                    return currency;
                }
            }

            return new Currency();
        }

        public string RenderCurrencyAmount(double amount, string currencyCode = "USD", int decimalPlaces = 0)
        {
            var currencyAmount = "";
            var currencySymbol = "";

            if (!string.IsNullOrEmpty(currencyCode))
            {
                currencySymbol = GetCurrencySymbolFromCode(currencyCode);
            }
            if (decimalPlaces == 0)
            {
                currencyAmount = currencySymbol + string.Format("{0:n0}", amount);
            }
            if (decimalPlaces == 1)
            {
                currencyAmount = currencySymbol + string.Format("{0:n1}", amount);
            }
            if (decimalPlaces == 2)
            {
                currencyAmount = currencySymbol + string.Format("{0:n2}", amount);
            }

            return currencyAmount;
        }
    }
}
