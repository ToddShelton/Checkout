using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Cloudrocket.Interfaces
{
    /// ASP.NET MVC 3 Credit Card Validator Attribute
    /// by Ben Cull - 4 November 2010
    /// http://benjii.me/2010/11/credit-card-validator-attribute-for-asp-net-mvc-3/
    ///
    /// With special thanks to:
    /// Thomas @ Orb of Knowledge - http://orb-of-knowledge.blogspot.com/2009/08/extremely-fast-luhn-function-for-c.html
    /// For the Extremely fast Luhn algorithm implementation
    ///
    /// And Paul Ingles - http://www.codeproject.com/Articles/2782/Credit-Card-Validator-control-for-ASP-NET
    /// For a timeless blog post on credit card validation

    public class CreditCardAttribute : ValidationAttribute
    {
        private CardType _cardTypes;

        public CardType AcceptedCardTypes
        {
            get { return _cardTypes; }
            set { _cardTypes = value; }
        }

        public CreditCardAttribute()
        {
            _cardTypes = CardType.All;
        }

        public CreditCardAttribute(CardType AcceptedCardTypes)
        {
            _cardTypes = AcceptedCardTypes;
        }

        public override bool IsValid(object value)
        {
            var number = Convert.ToString(value);

            // Remove non-digits first
            number = Regex.Replace(number, @"[^\d]", "");

            if (String.IsNullOrEmpty(number))
                return true;

            return IsValidType(number, _cardTypes) && IsValidNumber(number);
        }

        public override string FormatErrorMessage(string name)
        {
            return "The " + name + " field contains an invalid credit card number.";
        }

        public enum CardType
        {
            Unknown = 1,
            Visa = 2,
            MasterCard = 4,
            Amex = 8,
            Diners = 16,

            All = CardType.Visa | CardType.MasterCard | CardType.Amex | CardType.Diners,
            AllOrUnknown = CardType.Unknown | CardType.Visa | CardType.MasterCard | CardType.Amex | CardType.Diners,
        }

        private bool IsValidType(string cardNumber, CardType cardType)
        {
            // Visa
            if (Regex.IsMatch(cardNumber, "^(4)")
                && ((cardType & CardType.Visa) != 0))
                return cardNumber.Length == 13 || cardNumber.Length == 16;

            // MasterCard
            if (Regex.IsMatch(cardNumber, "^(51|52|53|54|55)")
                && ((cardType & CardType.MasterCard) != 0))
                return cardNumber.Length == 16;

            // Amex
            if (Regex.IsMatch(cardNumber, "^(34|37)")
                && ((cardType & CardType.Amex) != 0))
                return cardNumber.Length == 15;

            // Diners
            if (Regex.IsMatch(cardNumber, "^(300|301|302|303|304|305|36|38)")
                && ((cardType & CardType.Diners) != 0))
                return cardNumber.Length == 14;

            //Unknown
            if ((cardType & CardType.Unknown) != 0)
                return true;

            return false;
        }

        private bool IsValidNumber(string number)
        {
            int[] DELTAS = new int[] { 0, 1, 2, 3, 4, -4, -3, -2, -1, 0 };
            int checksum = 0;
            char[] chars = number.ToCharArray();
            for (int i = chars.Length - 1; i > -1; i--)
            {
                int j = ((int)chars[i]) - 48;
                checksum += j;
                if (((i - chars.Length) % 2) == 0)
                    checksum += DELTAS[j];
            }

            return ((checksum % 10) == 0);
        }
    }
}