using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DaoModels.DAO;
using DaoModels.DAO.DTO;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;
using MDispatch.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Stripe;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Models;
using WebDispacher.Models.Subscription;
using WebDispacher.Service;
using WebDispacher.Service.EmailSmtp;
using WebDispacher.Service.TransportationManager;
using WebDispacher.ViewModels.Company;
using WebDispacher.ViewModels.Contact;
using WebDispacher.ViewModels.Dispatcher;
using WebDispacher.ViewModels.Payment;
using WebDispacher.ViewModels.RA.Carrier.Registration;

namespace WebDispacher.Business.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly Context db;
        private readonly IMapper mapper; 
        private readonly StripeApi stripeApi;
        private readonly IUserService userService;
        private readonly int maxFileLength = 6 * 1024 * 1024;

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
            var companies = GetCompanies();
            
            return GetUsers()
                .Where(u => idCompanySelect == 0 || u.CompanyId == idCompanySelect)
                .Select(z => new UserDTO()
                {
                    CompanyName = companies.FirstOrDefault(c => c.Id == z.CompanyId) 
                                  != null ? companies.FirstOrDefault(c => c.Id == z.CompanyId).Name : string.Empty,
                    Id = z.Id,
                    Login = z.Login,
                    Password = z.Password,
                    CompanyId = z.CompanyId,
                    Date = z.Date
                })
                .ToList();
        }

        public bool CheckCompanyName(string companyName)
        {
            return db.Commpanies.FirstOrDefault(u => u.Name == companyName) != null;
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
        
        public async Task<CompanyViewModel> GetCompanyById(int id)
        {
            var company = await GetCompanyByIdDb(id);
            
            return company ?? new CompanyViewModel();
        }
        
        public async Task<List<CompanyDTO>> GetCompaniesDTO(int page)
        {
            var companies = await GetCompaniesDb(page);

            var companiesDTO = companies.Select(c => new CompanyDTO()
            {
                Id = c.Id,
                Active = c.Active,
                Name = c.Name,
                Type = c.Type == TypeCompany.BaseCommpany
                        ? CompanyConstants.HomeCompany : c.Type == TypeCompany.NormalCompany
                            ? CompanyConstants.RegularCompany : CompanyConstants.Unknown,
                DateRegistration = c.DateRegistration

            }).ToList();

            return companiesDTO;
        }

        public ContactViewModel GetContact(int id)
        {
            return GetContactById(id);
        }
        
        public string RefreshTokenDispatch(string idDispatch)
        {
            var token = GetHash();
            
            RefreshTokenDispatchDb(idDispatch, token);
            
            return token;
        }
        
        public void CreateDispatch(DispatcherViewModel dispatcher, int idCompany)
        {
            dispatcher.Key = GetHash();
            dispatcher.IdCompany = idCompany;
            
            db.Dispatchers.Add(mapper.Map<Dispatcher>(dispatcher));
            
            db.SaveChanges();
        }
        
        public async Task DeleteContactById(int id)
        {
            var contact = await db.Contacts.FirstOrDefaultAsync(c => c.ID == id);

            if (contact == null) return;
            
            db.Contacts.Remove(contact);
            
            await db.SaveChangesAsync();
        }
        
        public DispatcherViewModel GetDispatcherById(int idDispatch)
        {
            var dispatcher = db.Dispatchers.FirstOrDefault(d => d.Id == idDispatch);

            return mapper.Map<DispatcherViewModel>(dispatcher);
        }
        
        public void RemoveDispatchById(int idDispatch)
        {
            var dispatcher = db.Dispatchers.FirstOrDefault(d => d.Id == idDispatch);
            if (dispatcher == null) return;
            
            db.Dispatchers.Remove(dispatcher);
            
            db.SaveChanges();
        }
        
        public void EditContact(ContactViewModel contact)
        {
            var contactEdit = db.Contacts.FirstOrDefault(c => c.ID == contact.Id);
            if (contactEdit == null) return;
            
            contactEdit.Email = contact.Email.ToLower();
            contactEdit.Name = contact.Name;
            contactEdit.Phone = contact.Phone;
            
            db.SaveChanges();
        }
        
        public async Task EditCompany(CompanyViewModel company)
        {
            var companyEdit = await db.Commpanies.FirstOrDefaultAsync(c => c.Id == company.Id);
            var userEdit = await db.User.FirstOrDefaultAsync(u => u.CompanyId == company.Id);
            if (companyEdit == null || userEdit == null) return;

            companyEdit.Name = company.Name;
            userEdit.Password = company.Password;
            userEdit.Login = company.Email.ToLower();
            
            await db.SaveChangesAsync();
        }
        
        public void EditDispatch(DispatcherViewModel dispatcher)
        {
            var dispatcherEdit = db.Dispatchers.FirstOrDefault(d => d.Id == dispatcher.Id);
            if (dispatcherEdit == null) return;
            
            dispatcherEdit.Login = dispatcher.Login;
            dispatcherEdit.Password = dispatcher.Password;
            dispatcherEdit.Type = dispatcher.Type;
            
            db.SaveChanges();
        }
        
        public ResponseStripe AddPaymentCard(string idCompany, CardViewModel card)
        {
            var responseStripe = stripeApi.CreatePaymentMethod(card.Number.Replace(" ", ""), card.Name, card.Expiry, card.Cvc);

            if (responseStripe == null || responseStripe.IsError) return responseStripe;
            
            var paymentMethod = (PaymentMethod)responseStripe.Content;
            var customerSt = GetCustomer_STByIdCompany(idCompany);
            responseStripe = stripeApi.AttachPayMethod(paymentMethod.Id, customerSt.IdCustomerST);
                
            if (responseStripe != null && !responseStripe.IsError)
            {
                responseStripe = stripeApi.SelectDefaultPaymentMethod(paymentMethod.Id, customerSt.IdCustomerST);
            }

            if (responseStripe == null || responseStripe.IsError) return responseStripe;
            
            var isFirstPaymentMethodInCompany = CheckFirstPaymentMethodInCompany(idCompany);
            var paymentMethodSt = new PaymentMethod_ST()
            {
                IdCompany = Convert.ToInt32(idCompany),
                IdCustomerAttachPaymentMethod = customerSt.IdCustomerST,
                IdPaymentMethod_ST = paymentMethod.Id,
                IsDefault = !isFirstPaymentMethodInCompany
            };
            
            AddPaymentMethodSt(paymentMethodSt);

            return responseStripe;
        }
        
        public void DeletePaymentMethod(string idPayment, string idCompany)
        {
            stripeApi.DeletePaymentMethod(idPayment);
            var idPaymentNewSelect = RemovePaymentMethod(idCompany, idPayment);
            
            if (idPaymentNewSelect != null)
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
            if (customer_ST == null)
            {
                customer_ST = GetCustomer_STByIdCompany(idCompany);
            }
            
            var responseStripe = stripeApi.SelectDefaultPaymentMethod(idPayment, customer_ST.IdCustomerST);

            if (responseStripe == null || responseStripe.IsError) return responseStripe;
            
            UnSelectPaymentMethod_ST(idCompany);
            SelectPaymentMethod_ST(idCompany, idPayment);

            return responseStripe;
        }

        public List<PaymentMethod> GetPaymentMethod(string idCompany)
        {
            var customerSt = GetCustomer_STByIdCompany(idCompany);
            
            return stripeApi.GetPaymentMethodsByCustomerST(customerSt.IdCustomerST);
        }
        
        public List<PaymentMethod_ST> GetPaymentMethodsST(string idCompany)
        {
            return GetPaymentMethod_STsByIdCompany(idCompany);
        }

        public void RemoveDocCompany(string idDock)
        {
            var documentCompany = db.DucumentCompanies.FirstOrDefault(d => d.Id.ToString() == idDock);
            
            if (documentCompany == null) return;
            
            db.DucumentCompanies.Remove(documentCompany);
            db.SaveChanges();
        }
        
        public async Task<List<DucumentCompany>> GetCompanyDoc(string id)
        {
            return await db.DucumentCompanies.Where(d => d.IdCommpany.ToString() == id).ToListAsync();
        }
        
        public void CreateContact(ContactViewModel contact, string idCompany)
        {
            contact.CompanyId = Convert.ToInt32(idCompany); 
            contact.Email = contact.Email != null ? contact.Email.ToLower() : contact.Email;

            var addContact = mapper.Map<Contact>(contact);
            

            db.Contacts.AddAsync(addContact);
            
            db.SaveChangesAsync();
        }

        public async Task AddShortCompany(FewMoreDetailsViewModel model)
        {
            var companyModel = mapper.Map<CreateCompanyViewModel>(model);

            var company = new Commpany()
            {
                Active = true,
                DateRegistration = DateTime.Now.ToString(),
                Name = companyModel.Name,
                Type = TypeCompany.NormalCompany
            };

            var id = AddCompanyDb(company);

            InitStripeForCompany(companyModel.Name, companyModel.Email, id);
            userService.CreateUserForCompanyId(id, companyModel.Email, model.PersonalData.Password);

            var pattern = new PaternSourse().GetPatternRegistrationMail();

            await new AuthMessageSender().Execute(companyModel.Email, UserConstants.ThankRegistrationSubject, pattern);
        }

        public async Task AddCompany(CreateCompanyViewModel model,
            IFormFile MCNumberConfirmation, IFormFile IFTA, IFormFile KYU, IFormFile logbookPapers,
            IFormFile COI, IFormFile permits, string dateTimeLocal)
        {
            var company = new Commpany()
            {
                Active = true,
                DateRegistration = dateTimeLocal,
                Name = model.Name,
                Type = TypeCompany.NormalCompany
            };

            var id = AddCompanyDb(company);
            InitStripeForCompany(model.Name, model.Email, id);
            userService.CreateUserForCompanyId(id, model.Email, model.Password);

            if (MCNumberConfirmation != null)
            {
                await SaveDocCompany(MCNumberConfirmation, DocAndFileConstants.McNumber, id.ToString());
            }

            if (IFTA != null)
            {
                await SaveDocCompany(IFTA, DocAndFileConstants.Ifta, id.ToString());
            }

            if (KYU != null)
            {
                await SaveDocCompany(KYU, DocAndFileConstants.Kyu, id.ToString());
            }

            if (logbookPapers != null)
            {
                await SaveDocCompany(logbookPapers, DocAndFileConstants.LogbookPapers, id.ToString());
            }

            if (COI != null)
            {
                await SaveDocCompany(COI, DocAndFileConstants.Coi, id.ToString());
            }

            if (permits != null)
            {
                await SaveDocCompany(permits, DocAndFileConstants.Permits, id.ToString());
            }
        }
        
        public async Task<List<Contact>> GetContacts(int page, string idCompany)
        {
            return await GetContactsInDb(page, idCompany);
        }

        public async Task<int> GetCountContactsPages(string idCompany)
        {
            return await GetCountContactsPagesInDb(idCompany);
        }

        public async Task<int> GetCountCompaniesPages()
        {
            return await GetCountCompaniesPagesInDb();
        }

        public string GetTypeNavBar(string key, string idCompany, string typeNav = NavConstants.Work)
        {
            var typeNavBar = string.Empty;
            var users = userService.GetUserByKey(key);
            var company = userService.GetCompanyById(idCompany);

            if (users == null || company == null) return typeNavBar;
            
            switch (typeNav)
            {
                case NavConstants.TypeNavCancel:
                {
                    if (company.Type == TypeCompany.NormalCompany)
                    {
                        typeNavBar = NavConstants.CancelSubscribe;
                    }

                    break;
                }
                case NavConstants.Work when company.Type == TypeCompany.BaseCommpany:
                    typeNavBar = NavConstants.BaseCompany;
                    break;
                case NavConstants.Work:
                {
                    if (company.Type == TypeCompany.NormalCompany)
                    {
                        typeNavBar = NavConstants.NormalCompany;
                    }

                    break;
                }
                case NavConstants.TypeNavSettings when company.Type == TypeCompany.BaseCommpany:
                    typeNavBar = NavConstants.TypeNavBaseCompanySettings;
                    break;
                case NavConstants.TypeNavSettings:
                {
                    if (company.Type == TypeCompany.NormalCompany)
                    {
                        typeNavBar = NavConstants.TypeNavNormalCompanySettings;
                    }

                    break;
                }
            }

            return typeNavBar;
        }

        public List<Models.Subscription.Subscription> GetSubscriptions()
        {
            var subscriptions = new List<Models.Subscription.Subscription>
            {
                new Models.Subscription.Subscription()
                {
                    Id = 1,
                    IdSubscriptionST = DriverConstants.SubscriptionStIdDay,
                    Name = DriverConstants.SubscriptionNameDay,
                    PeriodDays = 1,
                    Price = DriverConstants.SubscriptionPrice,
                },
                new Models.Subscription.Subscription()
                {
                    Id = 2,
                    IdSubscriptionST = DriverConstants.SubscriptionStIdWeek,
                    Name = DriverConstants.SubscriptionNameWeek,
                    PeriodDays = 7,
                    Price = DriverConstants.SubscriptionPrice,
                },
                new Models.Subscription.Subscription()
                {
                    Id = 3,
                    IdSubscriptionST = DriverConstants.SubscriptionStIdMonth,
                    Name = DriverConstants.SubscriptionNameMonth,
                    PeriodDays = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month),
                    Price = DriverConstants.SubscriptionPrice,
                }
            };

            return subscriptions;
        }
        
        public string SelectSub(string idPrice, string idCompany, string priodDays)
        {
            var errorStr = string.Empty;
            var customerSt = GetCustomer_STByIdCompany(idCompany);
            var subscribeSt = GetSubscriptionIdCompany(idCompany);
            var responseStripe = stripeApi.GetSubscriptionSTById(subscribeSt.IdSubscribeST);
            
            if(!responseStripe.IsError)
            {
                var subscription = responseStripe.Content as Stripe.Subscription;
                
                if(subscription.Status == DriverConstants.Canceled)
                {
                    responseStripe = stripeApi.CreateSupsctibe(idPrice, customerSt);
                    
                    if (!responseStripe.IsError)
                    {
                        UpdateTypeInactiveByIdCompany(idCompany);
                        subscribeSt = new Subscribe_ST();
                        subscription = responseStripe.Content as Stripe.Subscription;
                        subscribeSt.IdCustomerST = subscription.CustomerId;
                        subscribeSt.IdSubscribeST = subscription.Id;
                        subscribeSt.IdCompany = Convert.ToInt32(idCompany);
                        subscribeSt.Status = subscription.Status;
                        subscribeSt.ActiveType = ActiveType.Active;
                        SaveSubscribeSt(subscribeSt);
                    }
                    else
                    {
                        errorStr = responseStripe.Message;
                    }
                    
                    return errorStr;
                }
                
                stripeApi.UpdateSubscribeCancelAtPeriodEnd(subscription.Id, true);
                var subscribeStNext = GetSubscriptionIdCompany(idCompany, ActiveType.NextActive);
                
                if (subscribeStNext != null)
                {
                    stripeApi.CanceleSubscribe(subscribeStNext.IdSubscribeST);
                    UpdateTypeActiveSubById(subscribeStNext.Id, ActiveType.Inactive);
                }
                
                subscribeStNext = new Subscribe_ST();
                var subscriptionNext = stripeApi.CreateSupsctibeNext(customerSt.IdCustomerST, idPrice, priodDays, subscription.CurrentPeriodEnd);
                subscribeStNext.IdCustomerST = subscriptionNext.CustomerId;
                subscribeStNext.IdSubscribeST = subscriptionNext.Id;
                subscribeStNext.IdCompany = Convert.ToInt32(idCompany);
                subscribeStNext.Status = subscriptionNext.Status;
                subscribeStNext.ActiveType = ActiveType.NextActive;
                
                SaveSubscribeSt(subscribeStNext);
            }
            else
            {
                errorStr = responseStripe.Message;
            }
            
            return errorStr;
        }

        public void CancelSubscriptionsNext(string idCompany)
        {
            var subscribeSt = GetSubscriptionIdCompany(idCompany);
            stripeApi.UpdateSubscribeCancelAtPeriodEnd(subscribeSt.IdSubscribeST, false);
            var subscribeStNext = GetSubscriptionIdCompany(idCompany, ActiveType.NextActive);

            if (subscribeStNext == null) return;
            
            stripeApi.CanceleSubscribe(subscribeStNext.IdSubscribeST);
            UpdateTypeActiveSubById(subscribeStNext.Id, ActiveType.Inactive);
        }
        
        public SubscriptionCompanyDTO GetSubscription(string idCompany)
        {
            var subscriptionCompanyDto = new SubscriptionCompanyDTO();
            var subscribeSt = GetSubscriptionIdCompany(idCompany);

            var response = stripeApi.GetSubscriptionSTById(subscribeSt.IdSubscribeST);

            if (response.IsError) return subscriptionCompanyDto;
            
            var subscriptions = response.Content as Stripe.Subscription;
            var price = subscriptions.Items.Data[0].Price;
            
            subscriptionCompanyDto.EndPeriod = subscriptions.CurrentPeriodEnd.ToShortDateString();
            subscriptionCompanyDto.StartPeriod = subscriptions.CurrentPeriodStart.ToShortDateString();
            subscriptionCompanyDto.Name = DriverConstants.DriverSubscribe;
            subscriptionCompanyDto.NextInvoce = subscriptions.CurrentPeriodEnd.ToShortDateString();
            subscriptionCompanyDto.PeriodCount = (subscriptions.CurrentPeriodEnd - subscriptions.CurrentPeriodStart).Days.ToString();
            subscriptionCompanyDto.Price = (price.UnitAmount * subscriptions.Items.Data[0].Quantity / 100).ToString();
            subscriptionCompanyDto.Status = subscriptions.Status;

            return subscriptionCompanyDto;
        }
        
        public bool GetCancelSubscribe(string idCompany)
        {
            var company = userService.GetCompanyById(idCompany);
            
            if (company.Type == TypeCompany.BaseCommpany)
            {
                return false;
            }
            
            var subscriptionCompanyDto = GetSubscription(idCompany);
            
            return subscriptionCompanyDto.Status == DriverConstants.Canceled;
        }

        private async Task<int> GetCountContactsPagesInDb(string idCompany)
        {
            var countDrivers = await db.Contacts.Where(c => c.CompanyId.ToString() == idCompany).CountAsync();

            var countPages = GetCountPage(countDrivers, UserConstants.NormalPageCount);

            return countPages;
        }

        private int GetCountPage(int countElements, int countElementsInOnePage)
        {
            var countPages = (countElements / countElementsInOnePage) % countElementsInOnePage;

            return countPages > 0 ? countPages + 1 : countPages;
        }

        private async Task<int> GetCountCompaniesPagesInDb()
        {
            var countCompanies = await db.Commpanies.CountAsync();

            var countPages = GetCountPage(countCompanies, UserConstants.NormalPageCount);

            return countPages;
        }

        private async Task<List<Contact>> GetContactsInDb(int page, string idCompany)
        {
            var contacts = db.Contacts.Where(c => c.CompanyId.ToString() == idCompany).AsQueryable();

            if (page == UserConstants.AllPagesNumber) return await contacts.ToListAsync();

            try
            {
                contacts = contacts.Skip(UserConstants.NormalPageCount * page - UserConstants.NormalPageCount);

                contacts = contacts.Take(UserConstants.NormalPageCount);
            }
            catch (Exception)
            {
                contacts = contacts.Skip((UserConstants.NormalPageCount * page) - UserConstants.NormalPageCount);
            }

            var listContacts = await contacts.ToListAsync();

            return listContacts;
        }

        private void UpdateTypeActiveSubById(int idSub, ActiveType activeType)
        {
            var subscribeSt = db.Subscribe_STs.FirstOrDefault(s => s.Id == idSub);
            
            if (subscribeSt == null) return;
            
            subscribeSt.ActiveType = activeType;
                
            db.SaveChanges();
        }
        
        private void UpdateTypeInactiveByIdCompany(string idCompany)
        {
            var subscribeSTs = db.Subscribe_STs
                .Where(s => s.IdCompany.ToString() == idCompany && s.ActiveType != ActiveType.Inactive)
                .ToList();
            
            foreach(var subscribeSt in subscribeSTs)
            {
                subscribeSt.ActiveType = ActiveType.Inactive;
            }
            
            db.SaveChanges();
        }

        private string CreateToken(string login, string password)
        {
            var token = new StringBuilder(string.Empty);

            for (var i = 0; i < login.Length; i++)
            {
                token.Append(i * new Random().Next(1, 1000) + login[i]);
            }
            for (var i = 0; i < password.Length; i++)
            {
                token.Append(i * new Random().Next(1, 1000) + password[i]);
            }

            return token.ToString();
        }

        private async Task<CompanyViewModel> GetCompanyByIdDb(int id)
        {
            var companyInfo = await db.Commpanies.FirstOrDefaultAsync(x => x.Id == id);

            if (companyInfo == null) return null;

            var companyViewModel = mapper.Map<CompanyViewModel>(companyInfo);

            var userInfo = await db.User.FirstOrDefaultAsync(x => x.CompanyId == id);
            if (userInfo == null) return null;

            companyViewModel.Password = userInfo.Password;
            companyViewModel.Email = userInfo.Login;

            return companyViewModel;
        }

        private async Task<List<Commpany>> GetCompaniesDb(int page)
        {
            var companies = db.Commpanies.AsQueryable();

            if (page == UserConstants.AllPagesNumber) return await companies.ToListAsync();

            try
            {
                companies = companies.Skip(UserConstants.NormalPageCount * page - UserConstants.NormalPageCount);

                companies = companies.Take(UserConstants.NormalPageCount);
            }
            catch (Exception)
            {
                companies = companies.Skip((UserConstants.NormalPageCount * page) - UserConstants.NormalPageCount);
            }

            var listCompanies = await companies.ToListAsync();

            return listCompanies;
        }

        private void InitStripeForCompany(string nameCompany, string emailCompany, int idCompany)
        {
            var customer = stripeApi.CreateCustomer(nameCompany, idCompany, emailCompany.ToLower());

            var customerSt = new Customer_ST()
            {
                DateCreated = customer.Created,
                IdCompany = idCompany,
                IdCustomerST = customer.Id,
                NameCompany = nameCompany,
                NameCompanyST = customer.Name
            };

            var subscription = stripeApi.CreateSupsctibe(customer.Id, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

            var subscribeSt = new Subscribe_ST()
            {
                IdCustomerST = subscription.CustomerId,
                IdItemSubscribeST = subscription.Items.Data[0].Id,
                IdSubscribeST = subscription.Id,
                IdCompany = idCompany,
                Status = subscription.Status
            };
            
            SaveCustomerSt(customerSt);
            SaveSubscribeSt(subscribeSt);
        }
        
        private void SaveCustomerSt(Customer_ST customerSt)
        {
            db.Customer_STs.Add(customerSt);
            db.SaveChanges();
        }
        
        private void SaveSubscribeSt(Subscribe_ST subscribeSt)
        {
            db.Subscribe_STs.Add(subscribeSt);
            db.SaveChanges();
        }
        
        public async Task SaveDocCompany(IFormFile uploadedFile, string nameDoc, string id)
        {
            if (uploadedFile.Length > maxFileLength) return;

            var path = $"../Document/Copmpany/{id}/" + uploadedFile.FileName;
            
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
            var pref = path.Remove(0, path.LastIndexOf(".") + 1);
            
            var documentCompany = new DucumentCompany()
            {
                DocPath = path,
                NameDoc = nameDoc,
                IdCommpany = Convert.ToInt32(id)
            };
            
            db.DucumentCompanies.Add(documentCompany);
            
            db.SaveChanges();
        }
        
        private int AddCompanyDb(Commpany company)
        {
            db.Commpanies.Add(company);
            db.SaveChanges();
            
            return company.Id;
        }

        private void SelectPaymentMethod_ST(string idCompany, string idPayment)
        {
            var paymentMethodSt = db.PaymentMethods
                .FirstOrDefault(pm => pm.IdCompany.ToString() == idCompany && pm.IdPaymentMethod_ST == idPayment);

            if (paymentMethodSt == null) return;
            
            paymentMethodSt.IsDefault = true;
            db.SaveChanges();
        }
        
        private void UnSelectPaymentMethod_ST(string idCompany)
        {
            var paymentMethodSt = db.PaymentMethods
                .FirstOrDefault(pm => pm.IdCompany.ToString() == idCompany && pm.IsDefault);

            if (paymentMethodSt == null) return;
            
            paymentMethodSt.IsDefault = false;
            db.SaveChanges();
        }
        
        private string RemovePaymentMethod(string idCompany, string idPayment)
        {
            string idPaymentNewSelect = null;
            var paymentMethodSt = db.PaymentMethods.FirstOrDefault(pm => pm.IdCompany.ToString() == idCompany && pm.IdPaymentMethod_ST == idPayment);

            if (paymentMethodSt == null) return null;
            
            db.PaymentMethods.Remove(paymentMethodSt);
            db.SaveChanges();
            
            if (!paymentMethodSt.IsDefault) return null;
            
            paymentMethodSt = db.PaymentMethods.FirstOrDefault();
            
            if(paymentMethodSt != null)
            {
                idPaymentNewSelect = paymentMethodSt.IdPaymentMethod_ST;
            }

            return idPaymentNewSelect;
        }
        
        private void AddPaymentMethodSt(PaymentMethod_ST paymentMethodSt)
        {
            db.PaymentMethods.Add(paymentMethodSt);
            
            db.SaveChanges();
        }
        
        private bool CheckFirstPaymentMethodInCompany(string idCompany)
        {
            var isFirstPaymentMethodInCompany = false;
            var paymentMethodSTs = GetPaymentMethod_STsByIdCompany(idCompany);
            
            if(paymentMethodSTs != null && paymentMethodSTs.Count != 0)
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
            return db.Customer_STs.FirstOrDefault(c => c.IdCompany.ToString() == idCompany);
        }

        private string GetHash()
        {
            var bytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            
            var hash = BitConverter.ToString(bytes).Replace("-", "").ToLower();
            
            return hash;
        }
        
        private void RefreshTokenDispatchDb(string idDispatch, string token)
        {
            var dispatcher = db.Dispatchers.FirstOrDefault(d => d.Id.ToString() == idDispatch);
            
            if (dispatcher == null) return;
            
            dispatcher.key = token;
            db.SaveChanges();
        }
        
        private ContactViewModel GetContactById(int id)
        {
            var contact = db.Contacts.FirstOrDefault(c => c.ID == id);

            return mapper.Map<ContactViewModel>(contact);
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
        
        private IEnumerable<Users> GetUsers()
        {
            return db.User.ToList();
        }
    }
}