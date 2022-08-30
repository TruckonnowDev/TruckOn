using DaoModels.DAO.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebDispacher.Models;

namespace WebDispacher.Service
{
    public class StripeApi
    {
        internal Customer CreateCustomer(string nameCommpany, int idCompany, string emailCommpany)
        {
            Customer customer = null;
            try
            {
                CustomerService customerService = new CustomerService();
                CustomerCreateOptions customerOptions = new CustomerCreateOptions
                {
                    Name = $"{nameCommpany}_{idCompany}",
                    Metadata = new Dictionary<string, string>()
                    {
                        {"nameCommpany",  nameCommpany },
                        {"idCompany", idCompany.ToString() }
                    },
                    Email = emailCommpany,
                };
                customer = customerService.Create(customerOptions);
            }
            catch(Exception e)
            {

            }
            return customer;
        }

        internal Subscription CreateSupsctibe(string customer, int periodDays)
        {
            Subscription subscription = null;
            try
            {
                SubscriptionCreateOptions subscriptionOptions = new SubscriptionCreateOptions
                {
                    Customer = customer,
                    Items = new List<SubscriptionItemOptions>()
                    {
                        new SubscriptionItemOptions()
                        {
                            Plan = "price_1IiO19KfezfzRoxlm1wjJ31N",
                            Quantity = 0,
                        }
                    },
                    TrialPeriodDays = periodDays,
                    
                };
                var subscriptionService = new Stripe.SubscriptionService();
                subscription = subscriptionService.Create(subscriptionOptions);
            }
            catch (Exception eeee)
            {

            }
            return subscription;
        }

        internal Subscription UpdateSupsctibe(int countDriver, string idItem)
        {
            Subscription subscription = null;
            try
            {
                var options = new SubscriptionItemUpdateOptions
                {
                    Quantity = countDriver

                };
                var service = new SubscriptionItemService();
                service.Update(idItem, options);
            }
            catch (Exception eeee)
            {

            }
            return subscription;
        }

        internal ResponseStripe GetSubscriptionSTById(string idSubscribeST)
        {
            ResponseStripe responseStripe = new ResponseStripe()
            {
                Content = null,
                IsError = false,
                Message = ""
            };
            try
            {
                var service = new SubscriptionService();
                responseStripe.Content = service.Get(idSubscribeST);
            } 
            catch (Exception e)
            {
                responseStripe.Message = e.Message;
                responseStripe.IsError = true;
            }
            return responseStripe;
        }

        internal List<PaymentMethod> GetPaymentMethodsByCustomerST(string idCustomerST)
        {
            List<PaymentMethod> paymentMethods = null;
            try
            {
                var options = new PaymentMethodListOptions
                {
                    Customer = idCustomerST,
                    Type = "card",
                };
                var service = new PaymentMethodService();
                paymentMethods = service.List(
                  options
                ).ToList();
            }
            catch
            {

            }
            return paymentMethods;
        }

        internal Customer RemoveCustomer(Customer_ST customer_ST)
        {
            Customer customer = null;
            try
            {
                var service = new CustomerService();
                customer = service.Delete(customer_ST.IdCustomerST);
            }
            catch (Exception e)
            {
            }
            return customer;
        }

        internal ResponseStripe CreatePaymentMethod(string number, string name, string expiry, string cvc)
        {
            ResponseStripe responseStripe = new ResponseStripe()
            {
                Content = null,
                IsError = false,
                Message = ""
            };
            try
            {
                int expMM = Convert.ToInt32(expiry.Remove(expiry.IndexOf("/")).Trim());
                int expYYYY = Convert.ToInt32(expiry.Remove(0, expiry.IndexOf("/") + 1).Trim());
                var options = new PaymentMethodCreateOptions
                {
                    Type = "card",
                    Card = new PaymentMethodCardCreateOptions
                    {
                        Number = number,
                        ExpMonth = expMM,
                        ExpYear = expYYYY,
                        Cvc = cvc,
                    },
                    Metadata = new Dictionary<string, string>
                    {
                        { "number", number },
                        { "name", name },
                        { "expMM", expMM.ToString() },
                        { "expYYYY", expYYYY.ToString() },
                        { "cvc", cvc },
                        {"default_payment_method", "checked" }
                    }

                };
                var service = new PaymentMethodService();
                PaymentMethod paymentMethod = service.Create(options);
                responseStripe.Content = paymentMethod;
            }
            catch (Exception e)
            {

                responseStripe.Message = e.Message;
                responseStripe.IsError = true;
            }
            return responseStripe;
        }

        internal void CanceleSubscribe(string idSubscribeST)
        {
            var service = new SubscriptionService();
            service.Cancel(idSubscribeST, null);
        }

        internal Subscription CreateSupsctibeNext(string idCustomerST, string idPrice, string periodDays, DateTime currentPeriodEnd)
        {
            Subscription subscription = null;
            try
            {
                SubscriptionCreateOptions subscriptionOptions = new SubscriptionCreateOptions
                {
                    Customer = idCustomerST,
                    Items = new List<SubscriptionItemOptions>()
                    {
                        new SubscriptionItemOptions()
                        {
                            Plan = idPrice,
                        }
                    },
                    TrialPeriodDays = (currentPeriodEnd.Date - DateTime.Now).Days + 1,

                };
                var subscriptionService = new Stripe.SubscriptionService();
                subscription = subscriptionService.Create(subscriptionOptions);
            }
            catch (Exception eeee)
            {

            }
            return subscription;
        }

        internal void UpdateSubscribeCancelAtPeriodEnd(string idSubscribeST, bool cancelAtPeriodEnd)
        {
            try
            {
                var options = new SubscriptionUpdateOptions
                {
                    CancelAtPeriodEnd = cancelAtPeriodEnd,
                };
                var service = new SubscriptionService();
                service.Update(idSubscribeST, options); 
            }
            catch
            {

            }
        }

        internal ResponseStripe CreateSupsctibe(string idPrice, Customer_ST customer_ST)
        {
            ResponseStripe responseStripe = new ResponseStripe()
            {
                Message = "",
                IsError = false,
                Content = null,
            };
            try
            {
                SubscriptionCreateOptions subscriptionOptions = new SubscriptionCreateOptions
                {
                    Customer = customer_ST.IdCustomerST,
                    Items = new List<SubscriptionItemOptions>()
                    {
                        new SubscriptionItemOptions()
                        {
                            Plan = idPrice,
                        }
                    },

                };
                var subscriptionService = new Stripe.SubscriptionService();
                responseStripe.Content = subscriptionService.Create(subscriptionOptions);
            }
            catch (Exception e)
            {
                responseStripe.IsError = true;
                responseStripe.Message = e.Message;
            }
            return responseStripe;
        }

        internal ResponseStripe AttachPayMethod(string idPaymentMethod, string idCustomerST)
        {
            ResponseStripe responseStripe = new ResponseStripe()
            {
                Content = null,
                IsError = false,
                Message = ""
            };
            try
            {
                var options = new PaymentMethodAttachOptions
                {
                    Customer = idCustomerST,
                };
                var service = new PaymentMethodService();
                service.Attach(
                  idPaymentMethod,
                  options
                );
                responseStripe.Content = service.Attach(
                  idPaymentMethod,
                  options
                ); 
            }
            catch (Exception e)
            {
                responseStripe.Message = e.Message;
                responseStripe.IsError = true;
            }
            return responseStripe;
        }

        internal void DeletePaymentMethod(string idPayment)
        {
            try
            {
                var service = new PaymentMethodService();
                service.Detach(idPayment);
            }
            catch
            { }
        }

        internal ResponseStripe SelectDefaultPaymentMethod(string idPayment, string idCustomerST)
        {
            ResponseStripe responseStripe = new ResponseStripe()
            {
                Content = null,
                IsError = false,
                Message = ""
            };
            try
            {
                var options = new CustomerUpdateOptions
                {
                    InvoiceSettings = new CustomerInvoiceSettingsOptions()
                    {
                        DefaultPaymentMethod = idPayment
                    }
                };
                var service = new CustomerService();
                service.Update(idCustomerST, options);
            }
            catch (Exception e)
            {
                responseStripe.Message = e.Message;
                responseStripe.IsError = true;
            }
            return responseStripe;
        }
    }
}