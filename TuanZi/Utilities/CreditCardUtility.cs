using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

namespace TuanZi.Utilities
{
    public static partial class CreditCardUtility
    {
        //http://www.regular-expressions.info/creditcard.html
        //# Visa: ^4[0-9]{12}(?:[0-9]{3})?$ All Visa card numbers start with a 4. New cards have 16 digits. Old cards have 13.
        //# MasterCard:  All MasterCard numbers start with the numbers 51 through 55. All have 16 digits.
        //# American Express: ^3[47][0-9]{13}$ American Express card numbers start with 34 or 37 and have 15 digits.
        //# Diners Club: ^3(?:0[0-5]|[68][0-9])[0-9]{11}$ Diners Club card numbers begin with 300 through 305, 36 or 38. All have 14 digits. There are Diners Club cards that begin with 5 and have 16 digits. These are a joint venture between Diners Club and MasterCard, and should be processed like a MasterCard.
        //# Discover: ^6(?:011|5[0-9]{2})[0-9]{12}$ Discover card numbers begin with 6011 or 65. All have 16 digits.
        //# JCB: ^(?:2131|1800|35\d{3})\d{11}$ JCB 

        public static string GetCardType(string cardNumber)
        {
            var cardType = string.Empty;
            try
            {
                var cardNum = cardNumber.Replace(" ", "").Replace("-", "");

                var AMEXPattern = @"^3[47][0-9]{13}$";
                var MasterCardPattern = @"^5[1-5][0-9]{14}$";
                var VisaCardPattern = @"^4[0-9]{12}(?:[0-9]{3})?$";
                var DinersClubCardPattern = @"^3(?:0[0-5]|[68][0-9])[0-9]{11}$";
                var enRouteCardPattern = @"^(2014|2149)";
                var DiscoverCardPattern = @"^6(?:011|5[0-9]{2})[0-9]{12}$";
                var JCBCardPattern = @"^(?:2131|1800|35\d{3})\d{11}$";

                var patterns = new Dictionary<string, string>();
                patterns.Add("AMEX", AMEXPattern);
                patterns.Add("MasterCard", MasterCardPattern);
                patterns.Add("Visa", VisaCardPattern);
                patterns.Add("DinersClub", DinersClubCardPattern);
                patterns.Add("enRoute", enRouteCardPattern);
                patterns.Add("Discover", DiscoverCardPattern);
                patterns.Add("JCB", JCBCardPattern);

                foreach (var cardTypeName in patterns.Keys)
                {
                    var regex = new Regex(patterns[cardTypeName]);
                    if (regex.IsMatch(cardNum))
                    {
                        cardType = cardTypeName;
                        break;
                    }
                }
            }
            catch { }
            return cardType;

        }

       
    }

    
}
