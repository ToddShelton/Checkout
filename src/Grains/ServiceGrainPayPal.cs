using System;
using System.Globalization; 
using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudrocket.Interfaces;
using Orleans;
using Orleans.Providers;

namespace Cloudrocket.Grains
{
    [StorageProvider(ProviderName = "AzureTableGrainState")]
    public class ServiceGrainPayPal : Grain<IServiceGrainPayPalState>, IServiceGrainPayPal
    {
        private static string _payPalConfigurationGrain = "payPalConfigurationGrain";
        private static IServiceGrainPayPalConfiguration payPalConfigurationGrain = GrainFactory.GetGrain<IServiceGrainPayPalConfiguration>(_payPalConfigurationGrain);
        private static Dictionary<string, string> payPalConfiguration = new Dictionary<string, string>();

        public override Task OnActivateAsync()
        {
            return base.OnActivateAsync();
        }

        public async Task<PayPal.Api.Payment> GetPayPalPaymentAsync(Order order, Card card, string requestId = null)
        {
            // TODO: Put null handling on non-required fields
            // TODO: Put error handling on required fields.

            PayPal.Api.ItemList payPalItemList = new PayPal.Api.ItemList();

            payPalItemList.items = new List<PayPal.Api.Item>();
            foreach (var orderItem in order.OrderItems)
            {
                payPalItemList.items.Add(
                    new PayPal.Api.Item()
                    {
                        currency = String.IsNullOrEmpty(orderItem.Currency) ? "USD" : orderItem.Currency, // Required
                        description = orderItem.Description,
                        price = orderItem.Price.ToString("n2", CultureInfo.InvariantCulture),
                        name = String.IsNullOrEmpty(orderItem.Name) ? "Product Name" : orderItem.Name, // Required
                        quantity = orderItem.Quantity.ToString("n0", CultureInfo.InvariantCulture),
                        sku = orderItem.ProductSKU,
                        //url = cartItem.ProductUrl, // Not defined for this resource type
                    });
            };

            var transaction = new PayPal.Api.Transaction()
            {
                amount = new PayPal.Api.Amount()
                {
                    currency = "USD",
                    total = order.Total.ToString(), // Required, and must be the sum of the item totals.
                    details = new PayPal.Api.Details()
                    {
                        shipping = order.ShippingTotal.ToString("n2",CultureInfo.InvariantCulture),
                        subtotal = order.Subtotal.ToString("n2", CultureInfo.InvariantCulture),
                        tax = order.TaxTotal.ToString("n2", CultureInfo.InvariantCulture),
                    }
                },
                invoice_number = order.OrderId.ToString(),
                item_list = payPalItemList,
            };

            var payer = new PayPal.Api.Payer()
            {
                payment_method = "credit_card", // required?
                funding_instruments = new List<PayPal.Api.FundingInstrument>() {
                    new PayPal.Api.FundingInstrument(){
                        credit_card = new PayPal.Api.CreditCard()
                        {
                            first_name = card.FirstName,
                            last_name = card.LastName,
                            billing_address = new PayPal.Api.Address(){
                                line1 = card.BillingAddress.AddressLine1,
                                line2 = card.BillingAddress.AddressLine2,
                                city = card.BillingAddress.City,
                                country_code =
                                    card.BillingAddress.Country == null ? "US" : card.BillingAddress.Country.ToString().ToUpper(),
                                state = card.BillingAddress.StateProvince,
                                postal_code = card.BillingAddress.ZipPostalCode,
                            },
                            type = card.CardType.ToLowerInvariant(),
                            number = card.CardNumber,
                            cvv2 = card.CardCvvCode,
                            expire_month = Convert.ToInt32(card.ExpirationMonth),
                            expire_year = Convert.ToInt32(card.ExpirationYear),
                        }
                    }
                }
            };

            PayPal.Api.Payment paymentRequest = new PayPal.Api.Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = new List<PayPal.Api.Transaction>() { transaction }
            };

            if (order == null) { throw new ArgumentException("GetPayPalPaymentAsync: Cart can't be null."); }
            if (order == null) { throw new ArgumentException("GetPayPalPaymentAsync: Card can't be null."); }
            if (String.IsNullOrEmpty(requestId)) { throw new ArgumentException("GetPayPalPaymentAsync: RequestID can't be null."); }

            PayPal.Api.APIContext apiContext = new PayPal.Api.APIContext();

            try
            {
                apiContext = await GetPayPalApiContextAsync(requestId);
            }
            catch (Exception ex)
            {
                throw;
            }

            //https://developer.paypal.com/docs/api/#create-a-payment
            PayPal.Api.Payment response = new PayPal.Api.Payment();

            try
            {
                // Wrap this in a Task so it's awaitable.
                response = paymentRequest.Create(apiContext);
            }
            catch (PayPal.HttpException ex) { throw; }
            catch (InvalidCastException ex) { throw; }
            catch (Exception ex) { throw; }

            State.Payment = response;
            await State.WriteStateAsync();

            return await Task.FromResult(response);
        }

        public async Task<PayPal.Api.APIContext> GetPayPalApiContextAsync(string requestId)
        {
            try
            {
                payPalConfiguration = await payPalConfigurationGrain.GetPayPalConfigurationAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
            var clientId = payPalConfiguration["clientId"];
            var clientSecret = payPalConfiguration["clientSecret"];
            var accessToken = new PayPal.Api.OAuthTokenCredential(
                clientId,
                clientSecret,
                payPalConfiguration).GetAccessToken();

            PayPal.Api.APIContext payPalApiContext = new PayPal.Api.APIContext();

            try
            {
                payPalApiContext = new PayPal.Api.APIContext(accessToken, requestId); // requestID is required.
            }
            catch (OrleansException ex)
            {
                throw;
            }

            payPalApiContext.Config = payPalConfiguration;

            return await Task.FromResult(payPalApiContext);
        }

        public async Task ClearStateAsync()
        {
            State.Etag = null; 
            await State.ClearStateAsync();
        }
    }

    public interface IServiceGrainPayPalState : IGrainState
    {
        PayPal.Api.Payment Payment { get; set; }
    }
}