using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Cloudrocket.Interfaces
{
    public interface ICard
    {
        Card Card { get; set; }
    }

    public class Card
    {
        private string _cardNumber = String.Empty;

        public Card()
        {
            this.BillingAddress = new BaseAddress(); 
        }

        [DataType(DataType.Text)]
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Please enter your first name")]
        public string FirstName { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Please enter your last name")]
        public string LastName { get; set; }

        [CreditCard(AcceptedCardTypes = CreditCardAttribute.CardType.All)]
        [Display(Name = "Card Number")]
        [Required(ErrorMessage = "Please enter your card number.")]
        public string CardNumber
        {
            get { return _cardNumber; }

            set
            {
                if (_cardNumber != null)
                {
                    // Remove non-digits
                    _cardNumber = Regex.Replace(value, @"[^\d]", "");
                }

                _cardNumber = value;
            }
        }

        [DataType(DataType.Text)]
        [Display(Name = "Card Code")]
        [Required(ErrorMessage = "Please enter your card's CSC code")]
        public string CardCvvCode { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Card Type")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Please enter your card type")]
        //[Required(ErrorMessage = "Please enter your card type")]
        public string CardType { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Email Address")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Please enter your email address")]
        //[Required(ErrorMessage = "Please enter your card type")]
        public string EmailAddress { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Telephone Number")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Please enter your telephone number")]
        //[Required(ErrorMessage = "Please enter your card type")]
        public string Telephone { get; set; }

        public SelectList CardTypeSelectList
        {
            get
            {
                List<KeyValuePair<string, string>> cardTypes = new List<KeyValuePair<string, string>>();

                cardTypes.Add(new KeyValuePair<string, string>("AMEX", "American Express"));
                cardTypes.Add(new KeyValuePair<string, string>("DISCOVER", "Discover"));
                cardTypes.Add(new KeyValuePair<string, string>("MASTERCARD", "MasterCard"));
                cardTypes.Add(new KeyValuePair<string, string>("VISA", "Visa"));

                SelectList cardTypeSelectList = new SelectList(cardTypes, "key", "value", "--Credit Card Type--");

                return cardTypeSelectList;
            }
            set
            {
                value = (SelectList)CardTypeSelectList.SelectedValue;
            }
        }

        [DataType(DataType.Text)]
        [Display(Name = "Expiration Month")]
        [Required(ErrorMessage = "Please enter your card's expiration month ")]
        public string ExpirationMonth { get; set; }

        public SelectList ExpirationMonthSelectList
        {
            get
            {
                //Create the credit card expiration month SelectList
                List<KeyValuePair<string, string>> expirationDateMonths = new List<KeyValuePair<string, string>>();
                for (int i = 1; i <= 12; i++)
                {
                    DateTime month = new DateTime(2000, i, 1);
                    expirationDateMonths.Add(new KeyValuePair<string, string>(month.ToString("MM"), month.ToString("MM MMMM")));
                }

                SelectList expirationMonthSelectList = new SelectList(expirationDateMonths, "key", "value", "--Expiration Month--");

                return expirationMonthSelectList;
            }
            set
            {
                value = (SelectList)ExpirationMonthSelectList.SelectedValue;
            }
        }

        [DataType(DataType.Text)]
        [Display(Name = "Expiration Year")]
        [Required(ErrorMessage = "Please enter your card's expiration year")]
        public string ExpirationYear { get; set; }

        public SelectList ExpirationYearSelectList
        {
            get
            {
                List<KeyValuePair<string, string>> expirationDateYears = new List<KeyValuePair<string, string>>();
                for (int i = 0; i <= 8; i++)
                {
                    string year = (DateTime.Today.Year + i).ToString();
                    expirationDateYears.Add(new KeyValuePair<string, string>(year, year));
                }

                SelectList expirationYearSelectList = new SelectList(expirationDateYears, "key", "value", "--Expiration Month--");

                return expirationYearSelectList;
            }
            set
            {
                value = (SelectList)ExpirationYearSelectList.SelectedValue;
            }
        }

        public BaseAddress BillingAddress { get; set; }
    }
}