using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AutoMapper;
using DaoModels.DAO;
using DaoModels.DAO.DTO;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Stripe;
using WebDispacher.Business.Interfaces;
using WebDispacher.Models;
using WebDispacher.Models.Subscription;
using WebDispacher.Service;
using WebDispacher.Service.TransportationManager;

namespace WebDispacher.Business.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly Context db;
        private readonly IMapper mapper; 
        private readonly StripeApi stripeApi;
        private readonly IUserService userService;

        public CompanyService(
            IMapper mapper,
            Context db,
            IUserService userService)
        {
            this.userService = userService;
            this.mapper = mapper;
            this.db = db;
            stripeApi = new StripeApi();
        }
        
        public List<UserDTO> GetUsers(int idCompanySelect)
        {
            List<Commpany> commpanies = GetCompanies();
            
            return GetUsers()
                .Where(u => idCompanySelect == 0 || u.CompanyId == idCompanySelect)
                .Select(z => new UserDTO()
                {
                    CompanyName = commpanies.FirstOrDefault(c => c.Id == z.CompanyId) != null ? commpanies.FirstOrDefault(c => c.Id == z.CompanyId).Name : "",
                    Id = z.Id,
                    Login = z.Login,
                    Password = z.Password,
                    CompanyId = z.CompanyId,
                    Date = z.Date
                })
                .ToList();
        }

        public Dispatcher CheckKeyDispatcher(string key)
        {
            return GetDispatcherByKeyUsers(key);
        }
        
        public List<Dispatcher> GetDispatchers(int idCompany)
        {
            return GetDispatchersDb(idCompany);
        }
        
        public List<Commpany> GetCompanies()
        {
            return db.Commpanies.ToList();
        }
        
        public List<CompanyDTO> GetCompaniesDTO()
        {
            return GetCompanies()
                .Select(z => new CompanyDTO()
                {
                    Id = z.Id,
                    Active = z.Active,
                    Name = z.Name,
                    Type = z.Type == TypeCompany.BaseCommpany ? "Home Company" : z.Type == TypeCompany.NormalCompany ? "Regular company" : "Unknown",
                    DateRegistration = z.DateRegistration
                }).ToList();
        }
        
        public Contact GetContact(int id)
        {
            return GetContactById(id);
        }
        
        public string RefreshTokenDispatch(string idDispatch)
        {
            string token = GetHash();
            RefreshTokenDispatchDb(idDispatch, token);
            return token;
        }
        
        public void CreateDispatch(string typeDispatcher, string login, string password, int idCompany)
        {
            Dispatcher dispatcher = new Dispatcher()
            {
                Login = login,
                Password = password,
                Type = typeDispatcher,
                IdCompany = idCompany,
                key = GetHash(),
            };
            
            CreateDispatchDb(dispatcher);
        }
        
        public void DeleteContactById(int id)
        {
            Contact contact = db.Contacts.FirstOrDefault(c => c.ID == id);
            if(contact != null)
            {
                db.Contacts.Remove(contact);
                db.SaveChanges();
            }
        }
        
        public Dispatcher GetDispatcherById(int idDispatch)
        {
            return db.Dispatchers.FirstOrDefault(d => d.Id == idDispatch);
        }
        
        public void RemoveDispatchById(int idDispatch)
        {
            db.Dispatchers.Remove(db.Dispatchers.First(d => d.Id == idDispatch));
            db.SaveChanges();
        }
        
        public void EditContact(int id, string fullName, string emailAddress, string phoneNumber)
        {
            Contact contact = db.Contacts.FirstOrDefault(c => c.ID == id);
            if(contact != null)
            {
                contact.Email = emailAddress;
                contact.Name = fullName;
                contact.Phone = phoneNumber;
                db.SaveChanges();
            }
        }
        
        public void EditDispatch(int idDispatch, string typeDispatcher, string login, string password)
        {
            Dispatcher dispatcher = db.Dispatchers.FirstOrDefault(d => d.Id == idDispatch);
            if(dispatcher != null)
            {
                dispatcher.Login = login;
                dispatcher.Password = password;
                dispatcher.Type = typeDispatcher;
                db.SaveChanges();
            }
        }
        
        public ResponseStripe AddPaymentCard(string idCompany, string number, string name, string expiry, string cvc)
        {
            ResponseStripe responseStripe = stripeApi.CreatePaymentMethod(number.Replace(" ", ""), name, expiry, cvc);
            
            if (responseStripe != null && !responseStripe.IsError)
            {
                PaymentMethod paymentMethod = (PaymentMethod)responseStripe.Content;
                Customer_ST customer_ST = GetCustomer_STByIdCompany(idCompany);
                responseStripe = stripeApi.AttachPayMethod(paymentMethod.Id, customer_ST.IdCustomerST);
                
                if (responseStripe != null && !responseStripe.IsError)
                {
                    responseStripe = stripeApi.SelectDefaultPaymentMethod(paymentMethod.Id, customer_ST.IdCustomerST);
                }
                
                if (responseStripe != null && !responseStripe.IsError)
                {
                    bool isFirstPaymentMethodInCompany = CheckFirstPaymentMethodInCompany(idCompany);
                    PaymentMethod_ST paymentMethod_ST = new PaymentMethod_ST()
                    {
                        IdCompany = Convert.ToInt32(idCompany),
                        IdCustomerAttachPaymentMethod = customer_ST.IdCustomerST,
                        IdPaymentMethod_ST = paymentMethod.Id,
                        IsDefault = !isFirstPaymentMethodInCompany
                    };
                    AddPaymentMethod_ST(paymentMethod_ST);
                }
            }
            
            return responseStripe;
        }
        
        public void DeletePaymentMethod(string idPayment, string idCompany)
        {
            stripeApi.DeletePaymentMethod(idPayment);
            string idPaymentNewSelect = RemovePaymentMethod(idCompany, idPayment);
            if(idPaymentNewSelect != null)
            {
                SelectDefaultPaymentMethod(idPaymentNewSelect, idCompany);
            }
        }
        
        public Subscribe_ST GetSubscriptionIdCompany(string idCompany, ActiveType activeType = ActiveType.Active)
        {
            return db.Subscribe_STs.FirstOrDefault(s => s.IdCompany.ToString() == idCompany && s.ActiveType == activeType);
        }
        
        public ResponseStripe SelectDefaultPaymentMethod(string idPayment, string idCompany = null, Customer_ST customer_ST = null)
        {
            if(customer_ST == null)
            {
                customer_ST = GetCustomer_STByIdCompany(idCompany);
            }
            ResponseStripe responseStripe = stripeApi.SelectDefaultPaymentMethod(idPayment, customer_ST.IdCustomerST);
            if (responseStripe != null && !responseStripe.IsError)
            {
                UnSelectPaymentMethod_ST(idCompany);
                SelectPaymentMethod_ST(idCompany, idPayment);
            }
            
            return responseStripe;
        }

        public List<PaymentMethod> GetpaymentMethod(string idCompany)
        {
            Customer_ST customer_ST = GetCustomer_STByIdCompany(idCompany);
            return stripeApi.GetPaymentMethodsByCustomerST(customer_ST.IdCustomerST);
        }
        
        public List<PaymentMethod_ST> GetpaymentMethodsST(string idCompany)
        {
            return GetPaymentMethod_STsByIdCompany(idCompany);
        }
        
        public void RemoveDocCompany(string idDock)
        {
            db.DucumentCompanies.Remove(db.DucumentCompanies.First(d => d.Id.ToString() == idDock));
            db.SaveChanges();
        }
        
        public async Task<List<DucumentCompany>> GetCompanyDoc(string id)
        {
            return await db.DucumentCompanies.Where(d => d.IdCommpany.ToString() == id).ToListAsync();
        }
        
        public void CreateContact(string fullName, string emailAddress, string phoneNumber, string idCompany)
        {
            Contact contact = new Contact();
            contact.Email = emailAddress;
            contact.Name = fullName;
            contact.Phone = phoneNumber;
            contact.Phone = phoneNumber;
            contact.CompanyId = Convert.ToInt32(idCompany);
            
            SaveNewContact(contact);
        }

        public async void AddCompany(string nameCommpany, string emailCommpany, IFormFile MCNumberConfirmation, IFormFile IFTA,
            IFormFile KYU, IFormFile logbookPapers, IFormFile COI, IFormFile permits)
        {
            Commpany commpany = new Commpany()
            {
                Active = true,
                DateRegistration = DateTime.Now.ToString(),
                Name = nameCommpany,
                Type = TypeCompany.NormalCompany
            };
            
            int id = AddCommpanyDb(commpany);
            InitStripeForCompany(nameCommpany, emailCommpany, id);
            userService.CreateUserForCompanyId(id, nameCommpany, CreateToken(nameCommpany, new Random().Next(10, 1000).ToString()));
            await SaveDocCompany(MCNumberConfirmation, "MC number confirmation", id.ToString());
            
            if (IFTA != null)
            {
                await SaveDocCompany(IFTA, "IFTA (optional for 26000+)", id.ToString());
            }
            
            await SaveDocCompany(KYU, "KYU", id.ToString());
            await SaveDocCompany(logbookPapers, "Logbook Papers (manual, certificate, malfunction letter)", id.ToString());
            await SaveDocCompany(COI, "COI (certificate of insurance)", id.ToString());
            await SaveDocCompany(permits, "Permits (optional OR, FL, NM)", id.ToString());
        }
        
        public List<Contact> GetContacts(string idCompany)
        {
            return db.Contacts.Where(c => c.CompanyId.ToString() == idCompany).ToList();
        }

        public string GetTypeNavBar(string key, string idCompany, string typeNav = "Work")
        {
            string typeNavBar = "";
            Users users = userService.GetUserByKey(key);
            Commpany commpany = userService.GetCompanyById(idCompany);
            if (users != null && commpany != null)
            {
                if (typeNav == "Cancel")
                {
                    if (commpany.Type == TypeCompany.NormalCompany)
                    {
                        typeNavBar = "CancelSubscribe";
                    }
                }
                else if (typeNav == "Work")
                {
                    if (commpany.Type == TypeCompany.BaseCommpany)
                    {
                        typeNavBar = "BaseCommpany";
                    }
                    else if (commpany.Type == TypeCompany.NormalCompany)
                    {
                        typeNavBar = "NormaCommpany";
                    }
                }
                else if(typeNav == "Settings")
                {
                    if (commpany.Type == TypeCompany.BaseCommpany)
                    {
                        typeNavBar = "BaseCommpanySettings";
                    }
                    else if (commpany.Type == TypeCompany.NormalCompany)
                    {
                        typeNavBar = "NormaCommpanySettings";
                    }
                }
            }
            return typeNavBar;
        }

        public List<Models.Subscription.Subscription> GetSubscriptions()
        {
            List<Models.Subscription.Subscription> subscriptions = new List<Models.Subscription.Subscription>();
            
            subscriptions.Add(new Models.Subscription.Subscription()
            {
                Id = 1,
                IdSubscriptionST = "price_1IPTehKfezfzRoxln6JFEbgG",
                Name = "One dollar a day",
                PeriodDays = 1,
                Price = "$1",
            });
            
            subscriptions.Add(new Models.Subscription.Subscription()
            {
                Id = 2,
                IdSubscriptionST = "price_1IPTjVKfezfzRoxlgRaotwe3",
                Name = "One dollar a week",
                PeriodDays = 7,
                Price = "$1",
            });
            
            subscriptions.Add(new Models.Subscription.Subscription()
            {
                Id = 3,
                IdSubscriptionST = "price_1H3j18KfezfzRoxlJ9Of1Urs",
                Name = "One dollar a month",
                PeriodDays = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month),
                Price = "$1",
            });
            
            return subscriptions;
        }
        
        public string SelectSub(string idPrice, string idCompany, string priodDays)
        {
            string errorStr = "";
            Customer_ST customer_ST = GetCustomer_STByIdCompany(idCompany);
            Subscribe_ST subscribe_ST = GetSubscriptionIdCompany(idCompany);
            ResponseStripe responseStripe = stripeApi.GetSubscriptionSTById(subscribe_ST.IdSubscribeST);
            
            if(!responseStripe.IsError)
            {
                Stripe.Subscription subscription = responseStripe.Content as Stripe.Subscription;
                
                if(subscription.Status == "canceled")
                {
                    responseStripe = stripeApi.CreateSupsctibe(idPrice, customer_ST);
                    
                    if (!responseStripe.IsError)
                    {
                        UpdateTypeInactiveByIdCompany(idCompany);
                        subscribe_ST = new Subscribe_ST();
                        subscription = responseStripe.Content as Stripe.Subscription;
                        subscribe_ST.IdCustomerST = subscription.CustomerId;
                        subscribe_ST.IdSubscribeST = subscription.Id;
                        subscribe_ST.IdCompany = Convert.ToInt32(idCompany);
                        subscribe_ST.Status = subscription.Status;
                        subscribe_ST.ActiveType = ActiveType.Active;
                        SaveSubscribeST(subscribe_ST);
                    }
                    else
                    {
                        errorStr = responseStripe.Message;
                    }
                    
                    return errorStr;
                }
                
                stripeApi.UpdateSubscribeCancelAtPeriodEnd(subscription.Id, true);
                Subscribe_ST subscribe_STNext = GetSubscriptionIdCompany(idCompany, ActiveType.NextActive);
                
                if (subscribe_STNext != null)
                {
                    stripeApi.CanceleSubscribe(subscribe_STNext.IdSubscribeST);
                    UpdateTypeActiveSubById(subscribe_STNext.Id, ActiveType.Inactive);
                }
                
                subscribe_STNext = new Subscribe_ST();
                Stripe.Subscription subscriptionNext = stripeApi.CreateSupsctibeNext(customer_ST.IdCustomerST, idPrice, priodDays, subscription.CurrentPeriodEnd);
                subscribe_STNext.IdCustomerST = subscriptionNext.CustomerId;
                subscribe_STNext.IdSubscribeST = subscriptionNext.Id;
                subscribe_STNext.IdCompany = Convert.ToInt32(idCompany);
                subscribe_STNext.Status = subscriptionNext.Status;
                subscribe_STNext.ActiveType = ActiveType.NextActive;
                
                SaveSubscribeST(subscribe_STNext);
            }
            else
            {
                errorStr = responseStripe.Message;
            }
            
            return errorStr;
        }

        public void CancelSubscriptionsNext(string idCompany)
        {
            Subscribe_ST subscribe_ST = GetSubscriptionIdCompany(idCompany);
            stripeApi.UpdateSubscribeCancelAtPeriodEnd(subscribe_ST.IdSubscribeST, false);
            Subscribe_ST subscribe_STNext = GetSubscriptionIdCompany(idCompany, ActiveType.NextActive);
            
            if(subscribe_STNext != null)
            {
                stripeApi.CanceleSubscribe(subscribe_STNext.IdSubscribeST);
                UpdateTypeActiveSubById(subscribe_STNext.Id, ActiveType.Inactive);
            }
        }
        
        public SubscriptionCompanyDTO GetSubscription(string idCompany)
        {
            SubscriptionCompanyDTO subscriptionCompanyDTO = new SubscriptionCompanyDTO();
            Subscribe_ST subscribe_ST = GetSubscriptionIdCompany(idCompany);
            ResponseStripe response = stripeApi.GetSubscriptionSTById(subscribe_ST.IdSubscribeST);
            Stripe.Subscription subscriptions = null;
            
            if(!response.IsError)
            {
                subscriptions = response.Content as Stripe.Subscription;
                Stripe.Price price = subscriptions.Items.Data[0].Price;
                subscriptionCompanyDTO.EndPeriod = subscriptions.CurrentPeriodEnd.ToShortDateString();
                subscriptionCompanyDTO.StartPeriod = subscriptions.CurrentPeriodStart.ToShortDateString();
                subscriptionCompanyDTO.Name = "25$ from driver";
                subscriptionCompanyDTO.NextInvoce = subscriptions.CurrentPeriodEnd.ToShortDateString();
                subscriptionCompanyDTO.PeriodCount = (subscriptions.CurrentPeriodEnd - subscriptions.CurrentPeriodStart).Days.ToString();
                subscriptionCompanyDTO.Price = (price.UnitAmount * subscriptions.Items.Data[0].Quantity / 100).ToString();
                subscriptionCompanyDTO.Status = subscriptions.Status;
            }
            
            return subscriptionCompanyDTO;
        }
        
        public bool GetCancelSubscribe(string idCompany)
        {
            Commpany commpany = userService.GetCompanyById(idCompany);
            
            if(commpany.Type == TypeCompany.BaseCommpany)
            {
                return false;
            }
            
            SubscriptionCompanyDTO subscriptionCompanyDTO = GetSubscription(idCompany);
            
            return subscriptionCompanyDTO.Status == "canceled";
        }
        
        private void UpdateTypeActiveSubById(int idSub, ActiveType activeType)
        {
            Subscribe_ST subscribe_ST = db.Subscribe_STs.FirstOrDefault(s => s.Id == idSub);
            
            if(subscribe_ST != null)
            {
                subscribe_ST.ActiveType = activeType;
                
                db.SaveChanges();
            }
        }
        
        private void UpdateTypeInactiveByIdCompany(string idCompany)
        {
            List<Subscribe_ST> subscribe_STs = db.Subscribe_STs
                .Where(s => s.IdCompany.ToString() == idCompany && s.ActiveType != ActiveType.Inactive)
                .ToList();
            
            foreach(Subscribe_ST subscribe_ST in subscribe_STs)
            {
                subscribe_ST.ActiveType = ActiveType.Inactive;
            }
            
            db.SaveChanges();
        }
        
        private string CreateToken(string login, string password)
        {
            string token = "";
            for (int i = 0; i < login.Length; i++)
            {
                token += i * new Random().Next(1, 1000) + login[i];
            }
            for (int i = 0; i < password.Length; i++)
            {
                token += i * new Random().Next(1, 1000) + password[i];
            }
            return token;
        }
        
        private void InitStripeForCompany(string nameCommpany, string emailCommpany, int idCompany)
        {
            Customer_ST customer_ST = new Customer_ST();
            Subscribe_ST subscribe_ST = new Subscribe_ST();
            Customer customer = stripeApi.CreateCustomer(nameCommpany, idCompany, emailCommpany);
            customer_ST.DateCreated = customer.Created;
            customer_ST.IdCompany = idCompany;
            customer_ST.IdCustomerST = customer.Id;
            customer_ST.NameCompany = nameCommpany;
            customer_ST.NameCompanyST = customer.Name;
            Stripe.Subscription subscription = stripeApi.CreateSupsctibe(customer.Id, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            subscribe_ST.IdCustomerST = subscription.CustomerId;
            subscribe_ST.IdItemSubscribeST = subscription.Items.Data[0].Id;
            subscribe_ST.IdSubscribeST = subscription.Id;
            subscribe_ST.IdCompany = idCompany;
            subscribe_ST.Status = subscription.Status;
            SaveCustomerST(customer_ST);
            SaveSubscribeST(subscribe_ST);
        }
        
        private void SaveCustomerST(Customer_ST customer_ST)
        {
            db.Customer_STs.Add(customer_ST);
            db.SaveChanges();
        }
        
        private void SaveSubscribeST(Subscribe_ST subscribe_ST)
        {
            db.Subscribe_STs.Add(subscribe_ST);
            db.SaveChanges();
        }
        
        public async Task SaveDocCompany(IFormFile uploadedFile, string nameDoc, string id)
        {
            string path = $"../Document/Copmpany/{id}/" + uploadedFile.FileName;
            if (!Directory.Exists("../Document/Copmpany"))
            {
                Directory.CreateDirectory($"../Document/Copmpany");
            }
            if (!Directory.Exists($"../Document/Copmpany/{id}"))
            {
                Directory.CreateDirectory($"../Document/Copmpany/{id}");
            }
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                uploadedFile.CopyTo(fileStream);
            }
            SaveDocCompanyDb(path, id, nameDoc);
        }
        
        private void SaveDocCompanyDb(string path, string id, string nameDoc)
        {
            string pref = path.Remove(0, path.LastIndexOf(".") + 1);
            
            DucumentCompany ducumentCompany = new DucumentCompany()
            {
                DocPath = path,
                NameDoc = nameDoc,
                IdCommpany = Convert.ToInt32(id)
            };
            
            db.DucumentCompanies.Add(ducumentCompany);
            
            db.SaveChanges();
        }
        
        private int AddCommpanyDb(Commpany commpany)
        {
            db.Commpanies.Add(commpany);
            db.SaveChanges();
            return commpany.Id;
        }
        
        private async void SaveNewContact(Contact contact)
        {
            await db.Contacts.AddAsync(contact);
            await db.SaveChangesAsync();
        }
        
        
        
        
        
        private void SelectPaymentMethod_ST(string idCompany, string idPayment)
        {
            PaymentMethod_ST paymentMethod_ST = db.PaymentMethods.FirstOrDefault(pm => pm.IdCompany.ToString() == idCompany && pm.IdPaymentMethod_ST == idPayment);
            if (paymentMethod_ST != null)
            {
                paymentMethod_ST.IsDefault = true;
                db.SaveChanges();
            }
        }
        
        private void UnSelectPaymentMethod_ST(string idCompany)
        {
            PaymentMethod_ST paymentMethod_ST = db.PaymentMethods.FirstOrDefault(pm => pm.IdCompany.ToString() == idCompany && pm.IsDefault);
            if(paymentMethod_ST != null)
            {
                paymentMethod_ST.IsDefault = false;
                db.SaveChanges();
            }
        }
        
        private string RemovePaymentMethod(string idCompany, string idPayment)
        {
            string idPaymentNewSelect = null;
            PaymentMethod_ST paymentMethod_ST = db.PaymentMethods.FirstOrDefault(pm => pm.IdCompany.ToString() == idCompany && pm.IdPaymentMethod_ST == idPayment);
            db.PaymentMethods.Remove(paymentMethod_ST);
            db.SaveChanges();
            if(paymentMethod_ST.IsDefault)
            {
                paymentMethod_ST = db.PaymentMethods.FirstOrDefault();
                if(paymentMethod_ST != null)
                {
                    idPaymentNewSelect = paymentMethod_ST.IdPaymentMethod_ST;
                }
            }
            return idPaymentNewSelect;
        }
        
        private void AddPaymentMethod_ST(PaymentMethod_ST paymentMethod_ST)
        {
            db.PaymentMethods.Add(paymentMethod_ST);
            db.SaveChanges();
        }
        
        private bool CheckFirstPaymentMethodInCompany(string idCompany)
        {
            bool isFirstPaymentMethodInCompany = false;
            List<PaymentMethod_ST> paymentMethod_STs = GetPaymentMethod_STsByIdCompany(idCompany);
            if(paymentMethod_STs != null && paymentMethod_STs.Count != 0)
            {
                isFirstPaymentMethodInCompany = true;
            }
            return isFirstPaymentMethodInCompany;
        }
        
        private List<PaymentMethod_ST> GetPaymentMethod_STsByIdCompany(string idCompany)
        {
            return db.PaymentMethods.Where(pm => pm.IdCompany.ToString() == idCompany).ToList();
        }
        
        public Customer_ST GetCustomer_STByIdCompany(string idCompany)
        {
            Customer_ST customer_ST = null;
            customer_ST = db.Customer_STs.FirstOrDefault(c => c.IdCompany.ToString() == idCompany);
            return customer_ST;
        }
        
        private void CreateDispatchDb(Dispatcher dispatcher)
        {
            db.Dispatchers.Add(dispatcher);
            db.SaveChanges();
        }
        
        private string GetHash()
        {
            var bytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            string hash = BitConverter.ToString(bytes).Replace("-", "").ToLower();
            return hash;
        }
        
        private void RefreshTokenDispatchDb(string idDispatch, string token)
        {
            Dispatcher dispatcher = db.Dispatchers.FirstOrDefault(d => d.Id.ToString() == idDispatch);
            if(dispatcher != null)
            {
                dispatcher.key = token;
                db.SaveChanges();
            }
        }
        
        private Contact GetContactById(int id)
        {
            return db.Contacts.FirstOrDefault(c => c.ID == id) ;
        }

        private List<Dispatcher> GetDispatchersDb(int idCompany)
        {
            return db.Dispatchers
                .Where(d => d.IdCompany == idCompany)
                .ToList();
        }
        
        private Dispatcher GetDispatcherByKeyUsers(string key)
        {
            return db.Dispatchers.FirstOrDefault(d => d.key != null && d.key == key);
        }
        
        private List<Users> GetUsers()
        {
            return db.User.ToList();
        }
    }
}