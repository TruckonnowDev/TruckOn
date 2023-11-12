using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
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
using WebDispacher.ViewModels.Driver;
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

        private readonly IHttpContextAccessor httpContextAccessor;

        private readonly int maxFileLength = 6 * 1024 * 1024;

        public CompanyService(
            IMapper mapper,
            Context db, 
            IHttpContextAccessor httpContextAccessor,
            IUserService userService)
        {
            this.userService = userService;
            this.mapper = mapper;
            this.db = db;
            this.httpContextAccessor = httpContextAccessor;
            stripeApi = new StripeApi();
        }

        public async Task<Company> GetActiveUserCompanyByCompanyTypeUserId(string userId, CompanyType companyType)
        {
            var userCompany = await db.Companies.Include(c => c.CompanyUsers)
                .FirstOrDefaultAsync(c => 
                    c.CompanyUsers
                        .FirstOrDefault(cu => cu.UserId == userId).CompanyId == c.Id && c.CompanyType == companyType);

            return userCompany;
        }

        public List<UserDTO> GetUsers(int idCompanySelect)
        {
            var companies = GetCompanies();

            return null;
            /*return GetUsers()
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
                .ToList();*/
        }

        public async Task<bool> UploadCompanyRequiredDoc(
            IFormFile certificateOfInsurance,
            IFormFile mcLetter,
            string companyId)
        {
            try
            {
                if (!int.TryParse(companyId, out var result)) return false;

                await SaveDocCompany(certificateOfInsurance, DocAndFileConstants.CertificateOfInsurance, result, string.Empty);
                await SaveDocCompany(mcLetter, DocAndFileConstants.McLetter, result, string.Empty);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public async Task<int> GetNewUserMailQuestions()
        {
            var newUserMailQuestions = await db.UsersMailsQuestions
                .Where(question => !question.Views.Any())
                .CountAsync();

            return newUserMailQuestions;
        }
        
        public async Task SetViewInUserMailQuestion(ViewUserMailQuestion model)
        {
            using (var context = new Context())
            {
                if (model == null) return;

                model.UserAgent = GetUserAgent();
                model.IPAddress = GetIPAddress();

                await context.ViewsUsersMailsQuestions.AddAsync(model);

                await context.SaveChangesAsync();

                return;
            }
        }

        public async Task<bool> CheckCompanyRequiredDoc(string idCompany)
        {
            var companyDocs = await GetCompanyDoc(idCompany);

            return IsHaveRequredDocs(companyDocs);
        }

        public bool CheckCompanyName(string companyName)
        {
            return db.Companies.FirstOrDefault(u => u.Name == companyName) != null;
        }

        public Dispatcher CheckKeyDispatcher(string key)
        {
            return GetDispatcherByKeyUsers(key);
        }
        
        public List<Dispatcher> GetDispatchers(int idCompany)
        {
            return GetDispatchersDb(idCompany);
        }
        
        public List<Company> GetCompanies()
        {
            return db.Companies.ToList();
        }
        
        public async Task<CompanyViewModel> GetCompanyById(int id)
        {
            var companyViewModel = await GetCompanyByIdDb(id);
            
            return companyViewModel ?? new CompanyViewModel();
        }

        public async Task<bool> SendEmailToUserByCompanyId(string companyId, string subject, string message)
        {
            var user = await GetUserByCompanyId(companyId);

            var pattern = new PaternSourse().GetPatternSendMessageToUser(message);

            try
            {
                await new AuthMessageSender().Execute(user.Email, subject, pattern);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task SendUserAnswer(AdminAnswerViewModel model, string companyId, string localDate)
        {
            if (model == null) return;

            var question = await db.UsersMailsQuestions.FirstOrDefaultAsync(umq => umq.Id == model.QuestionId);

            if (question == null) return;

            try
            {
                var pattern = new PaternSourse().GetPatternSendMessageToUser(model.Message);

                await new AuthMessageSender().Execute(question.SenderEmail, UserConstants.AdminMessageSubject, pattern);
            }
            catch (Exception ex)
            {
                return;
            }


            var company = await GetCompanyById(Convert.ToInt32(companyId));
            var dateTimeAnswer = string.IsNullOrEmpty(localDate) ? DateTime.Now : DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            var answerToUser = new AdminAnswer()
            {
                AdminName = company.Name,
                Message = model.Message,
                DateTimeAnswer = dateTimeAnswer,
                UserMailQuestion = question,
            };

            question.MailStatus = MailStatus.Answered;

            await db.AdminAnswers.AddAsync(answerToUser);

            await db.SaveChangesAsync();
        }

        public async Task<UserMailQuestion> GetUserQuestionWithAnswers(int id)
        {
            var userQuestionWithAnswers = await db.UsersMailsQuestions
                .Include(umq => umq.AdminAnswers)
                .Include(umq => umq.PhoneNumber)
                .FirstOrDefaultAsync(umq => umq.Id == id);

            return userQuestionWithAnswers;
        }
        
        public async Task<List<UserMailQuestion>> GetUserQuestions()
        {
            var userQuestions = await db.UsersMailsQuestions.ToListAsync();

            return userQuestions;
        }

        public async Task<List<UserMailQuestion>> GetUserQuestionsWithoutAnswers(int page)
        {
            var userQuestions = db.UsersMailsQuestions
                .Include(umq => umq.PhoneNumber)
                .Where(umq => umq.AdminAnswers.Count == 0 && umq.MailStatus == MailStatus.Received)
                .OrderByDescending(umq => umq.DateTimeReceived)
                .AsQueryable();

            if (page == UserConstants.AllPagesNumber) return await userQuestions.ToListAsync();

            userQuestions = GetEntriesByPage(userQuestions, page);

            var listUserQuestions = await userQuestions.ToListAsync();

            return listUserQuestions;
        }
        
        public async Task<List<UserMailQuestion>> GetUserQuestionsWithAnswers(int page)
        {
            var userQuestions = db.UsersMailsQuestions
                .Include(umq => umq.PhoneNumber)
                .Where(umq => umq.AdminAnswers.Count > 0 && umq.MailStatus == MailStatus.Answered)
                .OrderByDescending(umq => umq.DateTimeReceived)
                .AsQueryable();

            if (page == UserConstants.AllPagesNumber) return await userQuestions.ToListAsync();

            userQuestions = GetEntriesByPage(userQuestions, page);

            var listUserQuestions = await userQuestions.ToListAsync();

            return listUserQuestions;
        }
        
        public async Task<List<UserMailQuestion>> GetAllUserQuestions(int page)
        {
            var userQuestions = db.UsersMailsQuestions
                .Include(umq => umq.PhoneNumber)
                .OrderByDescending(umq => umq.DateTimeReceived)
                .AsQueryable();

            if (page == UserConstants.AllPagesNumber) return await userQuestions.ToListAsync();

            userQuestions = GetEntriesByPage(userQuestions, page);

            var listUserQuestions = await userQuestions.ToListAsync();

            return listUserQuestions;
        }

        public async Task<int> GetCountNewUserQuestionsPages()
        {
            return await GetCountNewUserQuestionsPagesInDb();
        }

        private async Task<int> GetCountNewUserQuestionsPagesInDb()
        {
            var countNewUserQuestions = await db.UsersMailsQuestions.Where(umq => umq.AdminAnswers.Count == 0 && umq.MailStatus == MailStatus.Received).CountAsync();

            var countPages = GetCountPage(countNewUserQuestions, UserConstants.NormalPageCount);

            return countPages;
        }
        
        public async Task<int> GetCountAnsweredUserQuestionsPages()
        {
            return await GetCountAnsweredUserQuestionsPagesInDb();
        }

        private async Task<int> GetCountAnsweredUserQuestionsPagesInDb()
        {
            var countNewUserQuestions = await db.UsersMailsQuestions
                .Where(umq => umq.AdminAnswers.Count > 0 && umq.MailStatus == MailStatus.Answered)
                .CountAsync();

            var countPages = GetCountPage(countNewUserQuestions, UserConstants.NormalPageCount);

            return countPages;
        }
        
        public async Task<int> GetCountAllUserQuestionsPages()
        {
            return await GetCountAllUserQuestionsPagesInDb();
        }

        private async Task<int> GetCountAllUserQuestionsPagesInDb()
        {
            var countNewUserQuestions = await db.UsersMailsQuestions
                .CountAsync();

            var countPages = GetCountPage(countNewUserQuestions, UserConstants.NormalPageCount);

            return countPages;
        }

        private IQueryable<T> GetEntriesByPage<T>(IQueryable<T> entry, int page)
        {
            try
            {
                entry = entry.Skip(UserConstants.NormalPageCount * page - UserConstants.NormalPageCount);

                entry = entry.Take(UserConstants.NormalPageCount);
            }
            catch (Exception)
            {
                entry = entry.Skip((UserConstants.NormalPageCount * page) - UserConstants.NormalPageCount);
            }

            return entry;
        }
        
        public async Task<List<UserMailQuestion>> GetUserQuestionsWithAnswers()
        {
            var userQuestions = await db.UsersMailsQuestions
                .Include(umq => umq.PhoneNumber)
                .Where(umq => umq.AdminAnswers.Count == 0)
                .ToListAsync();

            return userQuestions;
        }

        public async Task<bool> CreateUserQuestions(UserMailQuestion userMailQuestion, string localDate)
        {
            try
            {
                var dateTimeUpdate = string.IsNullOrEmpty(localDate) ? DateTime.Now : DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

                var phoneNumber = new PhoneNumber
                {
                    DialCode = userMailQuestion.PhoneNumber.DialCode,
                    Iso2 = userMailQuestion.PhoneNumber.Iso2,
                    Number = userMailQuestion.PhoneNumber.Number,
                    Name = userMailQuestion.PhoneNumber.Name,
                };

                await db.PhonesNumbers.AddAsync(phoneNumber);

                await db.SaveChangesAsync();

                var userQuestion = new UserMailQuestion
                {
                    SenderName = userMailQuestion.SenderName,
                    SenderEmail = userMailQuestion.SenderEmail,
                    PhoneNumberId = phoneNumber.Id,
                    Message = userMailQuestion.Message,
                    DateTimeReceived = dateTimeUpdate,
                };

                await db.UsersMailsQuestions.AddAsync(userQuestion);

                await db.SaveChangesAsync();

                return true;
            }
            catch(Exception ex) 
            { 
                return false; 
            }
        }

        public async Task<List<Company>> GetCompaniesWithUsers(int page)
        {
            return await GetCompaniesWithUsersDb(page);
        }

        public async Task<User> GetUserByCompanyId(string companyId)
        {
            using (var dbt = new Context())
            {
                int companyIndex = Convert.ToInt32(companyId);
                var userInfo = await dbt.Users
                    .Include(u => u.CompanyUsers)
                    .FirstOrDefaultAsync(x => x.CompanyUsers.First(cu => cu.CompanyId == companyIndex).UserId == x.Id);

                if (userInfo == null) return null;

                return userInfo;
            }
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

        public DispatcherType GetDispatcherTypeByName(string name)
        {
            var dispatcherType = db.DispatchersTypes.FirstOrDefault(dt => dt.Name == name);

            return dispatcherType;
        }
        
        public void CreateDispatch(DispatcherViewModel model, int idCompany)
        {
            var dispatcher = new Dispatcher
            {
                CompanyId = idCompany,
                Key = GetHash(),
                Login = model.Login,
                Password = model.Password,
                DispatcherTypeId = GetDispatcherTypeByName(DispatcherTypesConstants.CentralDispatch).Id,
            };

            db.Dispatchers.Add(dispatcher);
            
            db.SaveChanges();
        }
        
        public async Task DeleteContactById(int id)
        {
            var contact = await db.Contacts.FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null) return;
            
            db.Contacts.Remove(contact);
            
            await db.SaveChangesAsync();
        }
        
        public DispatcherViewModel GetDispatcherById(int idDispatch)
        {
            var dispatcher = db.Dispatchers.Include(d => d.DispatcherType).FirstOrDefault(d => d.Id == idDispatch);


            return new DispatcherViewModel
            {
                Id = dispatcher.Id,
                IdCompany = dispatcher.CompanyId,
                Login = dispatcher.Login,
                Password = dispatcher.Password,
                Key = dispatcher.Key,
                Type = dispatcher.DispatcherType.Name
            };
        }
        
        public void RemoveDispatchById(int idDispatch)
        {
            var dispatcher = db.Dispatchers.FirstOrDefault(d => d.Id == idDispatch);
            if (dispatcher == null) return;
            
            db.Dispatchers.Remove(dispatcher);
            
            db.SaveChanges();
        }
        
        public async Task EditContact(ContactViewModel model, string localDate)
        {
            var dateTimeUpdate = DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            var contactEdit = await db.Contacts
                .Include(c => c.PhoneNumber)
                .FirstOrDefaultAsync(c => c.Id == model.Id);

            if (contactEdit == null) return;
            
            contactEdit.Name = model.Name;
            contactEdit.Position = model.Position;
            contactEdit.Email = model.Email.ToLower();
            contactEdit.PhoneNumber.Name = model.PhoneNumber.Name;
            contactEdit.PhoneNumber.Number = model.PhoneNumber.Number;
            if (contactEdit.PhoneNumber.DialCode != 0) contactEdit.PhoneNumber.DialCode = model.PhoneNumber.DialCode;
            contactEdit.PhoneNumber.Iso2 = model.PhoneNumber.Iso2;
            contactEdit.Ext = model.Ext;
            contactEdit.DateTimeLastUpdate = dateTimeUpdate;
            
            await db.SaveChangesAsync();
        }
        
        public async Task EditCompany(CompanyViewModel model, string localDate)
        {
            if (model == null) return;

            var dateTimeUpdate = DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            var companyEdit = await db.Companies.Include(d => d.PhoneNumber).FirstOrDefaultAsync(c => c.Id == model.Id);

            if (companyEdit == null) return;

            companyEdit.Name = model.Name;
            if (companyEdit.PhoneNumber.DialCode != 0) companyEdit.PhoneNumber.DialCode = model.PhoneNumber.DialCode;
            companyEdit.PhoneNumber.Name = model.PhoneNumber.Name;
            companyEdit.PhoneNumber.Number = model.PhoneNumber.Number;
            companyEdit.PhoneNumber.Iso2 = model.PhoneNumber.Iso2;
            companyEdit.CompanyType = model.CompanyType;
            companyEdit.DateTimeLastUpdate = dateTimeUpdate;
            
            await db.SaveChangesAsync();
        }
        
        public void EditDispatch(DispatcherViewModel dispatcher)
        {
            var dispatcherEdit = db.Dispatchers.FirstOrDefault(d => d.Id == dispatcher.Id);

            if (dispatcherEdit == null) return;
            
            dispatcherEdit.Login = dispatcher.Login;
            dispatcherEdit.Password = dispatcher.Password;
            //dispatcherEdit.DispatcherTypeId = dispatcher.Type;
            
            db.SaveChanges();
        }
        
        public ResponseStripe AddPaymentCard(string idCompany, CardViewModel card)
        {
            var responseStripe = stripeApi.CreatePaymentMethod(card.Number.Replace(" ", ""), card.Name, card.Expiry, card.Cvc);

            if (responseStripe == null || responseStripe.IsError) return responseStripe;
            
            var paymentMethod = (Stripe.PaymentMethod)responseStripe.Content;
            var customerSt = GetCustomer_STByIdCompany(idCompany);
            responseStripe = stripeApi.AttachPayMethod(paymentMethod.Id, customerSt.IdCustomerST);
                
            if (responseStripe != null && !responseStripe.IsError)
            {
                responseStripe = stripeApi.SelectDefaultPaymentMethod(paymentMethod.Id, customerSt.IdCustomerST);
            }

            if (responseStripe == null || responseStripe.IsError) return responseStripe;
            
            var isFirstPaymentMethodInCompany = CheckFirstPaymentMethodInCompany(idCompany);
            var paymentMethodSt = new DaoModels.DAO.Models.PaymentMethod()
            {
                CompanyId = Convert.ToInt32(idCompany),
                CustomerAttachPaymentMethodId = customerSt.IdCustomerST,
                PaymentMethodSTId = paymentMethod.Id,
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
        
        public async Task<SubscribeST> GetSubscriptionIdCompany(string idCompany, ActiveType activeType = ActiveType.Active)
        {
            return await db.SubscribeST.FirstOrDefaultAsync(s => s.CustomerST.CompanyId.ToString() == idCompany && s.ActiveType == activeType);
        }
        
        public ResponseStripe SelectDefaultPaymentMethod(string idPayment, string idCompany = null, CustomerST customer_ST = null)
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

        public List<Stripe.PaymentMethod> GetPaymentMethod(string idCompany)
        {
            var customerSt = GetCustomer_STByIdCompany(idCompany);

            return stripeApi.GetPaymentMethodsByCustomerST(customerSt.IdCustomerST);
        }
        
        public List<DaoModels.DAO.Models.PaymentMethod> GetPaymentMethodsST(string idCompany)
        {
            return GetPaymentMethod_STsByIdCompany(idCompany);
        }

        public async Task RemoveDocCompany(int docId)
        {
            var documentCompany = await db.DocumentsCompanies.FirstOrDefaultAsync(d => d.Id == docId);
            
            if (documentCompany == null) return;
            
            db.DocumentsCompanies.Remove(documentCompany);

            await db.SaveChangesAsync();
        }
        
        public async Task<List<DocumentCompany>> GetCompanyDoc(string id)
        {
            if (!int.TryParse(id, out var companyId)) return new List<DocumentCompany>();

            return await db.DocumentsCompanies.Where(d => d.CompanyId == companyId).ToListAsync();
        }
        
        public async Task CreateContact(ContactViewModel model, string companyId, string localDate)
        {
            if (!int.TryParse(companyId, out var result)) return;

            var dateTimeCreate = DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            var phoneContact = new PhoneNumber
            {
                Number = model.PhoneNumber.Number,
                DialCode = model.PhoneNumber.DialCode,
                Iso2 = model.PhoneNumber.Iso2,
                Name = model.PhoneNumber.Name,
            };

            await db.PhonesNumbers.AddAsync(phoneContact);

            await db.SaveChangesAsync();

            var contact = new Contact
            {
                Name = model.Name,
                Position = model.Position,
                Email = model.Email != null ? model.Email.ToLower() : model.Email,
                Ext = model.Ext,
                PhoneNumberId = phoneContact.Id,
                DateTimeCreate = dateTimeCreate,
                DateTimeLastUpdate = dateTimeCreate,
                CompanyId = result
            };

            await db.Contacts.AddAsync(contact);
            
            await db.SaveChangesAsync();
        }

        public async Task AddUserToCompany(User user, Company company)
        {
            var companyUser = new CompanyUser
            {
                UserId = user.Id,
                CompanyId = company.Id
            };
            
            await db.CompanyUsers.AddAsync(companyUser);
            await db.SaveChangesAsync();
        }

        public async Task<Company> CreateCompany(FewMoreDetailsViewModel model, CompanyType companyType = CompanyType.Carrier)
        {
            var phoneCompany = new PhoneNumber
            {
                Number = model.PersonalData.Number,
                DialCode = model.PersonalData.DialCode,
                Iso2 = model.PersonalData.Iso2,
                Name = model.PersonalData.Name,
            };

            await db.PhonesNumbers.AddAsync(phoneCompany);

            await db.SaveChangesAsync();

            var company = new Company()
            {
                Name = model.PersonalData.CompanyName,
                PhoneNumberId = phoneCompany.Id,
                CompanyType = companyType,
                USDOT = model.PersonalData.USDOTNumber.Value,
                CompanyStatus = CompanyStatus.Active,
                DateTimeRegistration = DateTime.Now,
                DateTimeLastUpdate = DateTime.Now,
            };

            await db.Companies.AddAsync(company);
            await db.SaveChangesAsync();

            InitStripeForCompany(model.PersonalData.CompanyName, model.PersonalData.Email, company.Id);

            return company;
        }

        public async Task<bool> ActivateCompany(string companyId)
        {
            if (!int.TryParse(companyId, out var result)) return false;

            var company = await db.Companies.FirstOrDefaultAsync(x => x.Id == result);

            var companyUser = await GetUserByCompanyId(companyId);

            if (company == null || companyUser == null) return false;

            try
            {
                company.CompanyStatus = CompanyStatus.Active;

                await db.SaveChangesAsync();

                InitStripeForCompany(company.Name, companyUser.Email, company.Id);

                var pattern = new PaternSourse().GetPatternSendMessageActivateCompany();

                await new AuthMessageSender().Execute(companyUser.Email, UserConstants.ActivateEmailSubject, pattern);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public async Task<Company> CreateCompanyByAdminWithDocs(CreateCompanyViewModel model,
            IFormFile MCNumberConfirmation, IFormFile IFTA, IFormFile KYU, IFormFile logbookPapers,
            IFormFile COI, IFormFile permits, string dateTimeLocal)
        {
            var phoneCompany = new PhoneNumber
            {
                Number = model.PhoneNumber.Number,
                DialCode = model.PhoneNumber.DialCode,
                Iso2 = model.PhoneNumber.Iso2,
                Name = model.PhoneNumber.Name,
            };

            await db.PhonesNumbers.AddAsync(phoneCompany);

            await db.SaveChangesAsync();

            var company = new Company()
             {
                 Name = model.Name,
                PhoneNumberId = phoneCompany.Id,
                CompanyType = model.CompanyType,
                 CompanyStatus = CompanyStatus.Active,
                 DateTimeRegistration = DateTime.Now,
                 DateTimeLastUpdate = DateTime.Now,
             };

             await db.Companies.AddAsync(company);

             await db.SaveChangesAsync();

             InitStripeForCompany(model.Name, model.Email, company.Id);

             if (MCNumberConfirmation != null)
             {
                 await SaveDocCompany(MCNumberConfirmation, DocAndFileConstants.McNumber, company.Id, dateTimeLocal);
             }

             if (IFTA != null)
             {
                 await SaveDocCompany(IFTA, DocAndFileConstants.Ifta, company.Id, dateTimeLocal);
             }

             if (KYU != null)
             {
                 await SaveDocCompany(KYU, DocAndFileConstants.Kyu, company.Id, dateTimeLocal);
             }

             if (logbookPapers != null)
             {
                 await SaveDocCompany(logbookPapers, DocAndFileConstants.LogbookPapers, company.Id, dateTimeLocal);
             }

             if (COI != null)
             {
                 await SaveDocCompany(COI, DocAndFileConstants.Coi, company.Id, dateTimeLocal);
             }

             if (permits != null)
             {
                 await SaveDocCompany(permits, DocAndFileConstants.Permits, company.Id, dateTimeLocal);
             }

            return company;
        }

        public async Task<List<Contact>> GetContactsByCompanyId(int page, string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await GetContactsInDb(page, result);
            }

            return new List<Contact>();
        }
        
        public async Task<List<ContactViewModel>> GetContactsViewModels(int page, string idCompany)
        {
            return await GetContactsViewModelInDb(page, idCompany);
        }

        public async Task<int> GetCountContactsPages(string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await GetCountContactsPagesInDb(result);
            }

            return 0;
        }

        public async Task<int> GetCountCompaniesPages()
        {
            return await GetCountCompaniesPagesInDb();
        }

        public string GetTypeNavBar(string companyId, string typeNav = NavConstants.Work)
        {
            var typeNavBar = string.Empty;
            var company = userService.GetCompanyById(companyId);

            if (company == null) return typeNavBar;
            if (company.CompanyStatus == CompanyStatus.Admin)
            {
                return NavConstants.BaseCompany;
            }
            switch (typeNav)
            {
                case NavConstants.TypeNavCancel:
                    {
                        if (company.CompanyStatus == CompanyStatus.Active)
                        {
                            typeNavBar = NavConstants.CancelSubscribe;
                        }

                        break;
                    }
                case NavConstants.Work when company.CompanyStatus == CompanyStatus.Admin:
                    typeNavBar = NavConstants.BaseCompany;
                    break;
                case NavConstants.Work:
                    {
                        if (company.CompanyStatus == CompanyStatus.Active)
                        {
                            if (company.CompanyType == CompanyType.Carrier)
                            {
                                typeNavBar = NavConstants.NormalCompany;
                            }

                            if (company.CompanyType == CompanyType.Broker)
                            {
                                typeNavBar = NavConstants.BrokerCompany;
                            }

                        }

                        break;
                    }
                case NavConstants.TypeNavSettings when company.CompanyStatus == CompanyStatus.Admin:
                    typeNavBar = NavConstants.TypeNavBaseCompanySettings;
                    break;
                case NavConstants.TypeNavSettings:
                    {
                        if (company.CompanyStatus == CompanyStatus.Active)
                        {
                            typeNavBar = NavConstants.TypeNavNormalCompanySettings;
                        }

                        break;
                    }
                case NavConstants.NormalCompany when company.CompanyStatus == CompanyStatus.Active:
                    {
                        typeNavBar = NavConstants.NormalCompany; 
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
        
        public async Task<string> SelectSub(string priceId, string companyId, string priodDays)
        {
            var errorStr = string.Empty;
            var customerSt = GetCustomer_STByIdCompany(companyId);
            var subscribeSt = await GetSubscriptionIdCompany(companyId);
            var responseStripe = stripeApi.GetSubscriptionSTById(subscribeSt.SubscribeSTId);
            
            if(!responseStripe.IsError)
            {
                var subscription = responseStripe.Content as Stripe.Subscription;
                
                if(subscription.Status == DriverConstants.Canceled)
                {
                    responseStripe = stripeApi.CreateSupsctibe(priceId, customerSt);
                    
                    if (!responseStripe.IsError)
                    {
                        UpdateTypeInactiveByIdCompany(companyId);
                        subscribeSt = new SubscribeST();
                        subscription = responseStripe.Content as Stripe.Subscription;
                        subscribeSt.CustomerST.IdCustomerST = subscription.CustomerId;
                        subscribeSt.SubscribeSTId = subscription.Id;
                        subscribeSt.CustomerST.CompanyId = Convert.ToInt32(companyId);
                        //subscribeSt.Status = subscription.Status;
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
                var subscribeStNext = await GetSubscriptionIdCompany(companyId, ActiveType.NextActive);
                
                if (subscribeStNext != null)
                {
                    stripeApi.CanceleSubscribe(subscribeStNext.SubscribeSTId);
                    UpdateTypeActiveSubById(subscribeStNext.Id, ActiveType.Inactive);
                }
                
                subscribeStNext = new SubscribeST();
                var subscriptionNext = stripeApi.CreateSupsctibeNext(customerSt.IdCustomerST, priceId, priodDays, subscription.CurrentPeriodEnd);
                subscribeStNext.CustomerST.IdCustomerST = subscriptionNext.CustomerId;
                subscribeStNext.SubscribeSTId = subscriptionNext.Id;
                subscribeStNext.CustomerST.CompanyId = Convert.ToInt32(companyId);
                //subscribeStNext.Status = subscriptionNext.Status;
                subscribeStNext.ActiveType = ActiveType.NextActive;
                
                SaveSubscribeSt(subscribeStNext);
            }
            else
            {
                errorStr = responseStripe.Message;
            }
            
            return errorStr;
        }

        public async Task CancelSubscriptionsNext(string companyId)
        {
            var subscribeSt = await GetSubscriptionIdCompany(companyId);
            stripeApi.UpdateSubscribeCancelAtPeriodEnd(subscribeSt.SubscribeSTId, false);
            var subscribeStNext = await GetSubscriptionIdCompany(companyId, ActiveType.NextActive);

            if (subscribeStNext == null) return;
            
            stripeApi.CanceleSubscribe(subscribeStNext.SubscribeSTId);
            UpdateTypeActiveSubById(subscribeStNext.Id, ActiveType.Inactive);
        }
        
        public async Task<SubscriptionCompanyDTO> GetSubscription(string companyId)
        {
            var subscriptionCompanyDto = new SubscriptionCompanyDTO();
            var subscribeSt = await GetSubscriptionIdCompany(companyId);

            var response = stripeApi.GetSubscriptionSTById(subscribeSt.SubscribeSTId);

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
        public async Task UpdateCompanyStatus(int companyId, CompanyStatus companyStatus)
        {
            var companyEdit = await db.Companies.FirstOrDefaultAsync(c => c.Id == companyId);
            if (companyEdit == null) return;

            companyEdit.CompanyStatus = companyStatus;

            await db.SaveChangesAsync();
        }

        public bool GetCancelSubscribe(string companyId)
        {
            var company = userService.GetCompanyById(companyId);
            
            if (company.CompanyStatus == CompanyStatus.Admin)
            {
                return false;
            }
            
            var subscriptionCompanyDto = GetSubscription(companyId);
            
            return subscriptionCompanyDto.Status.ToString() == DriverConstants.Canceled;
        }

        private async Task<int> GetCountContactsPagesInDb(int companyId)
        {
            var countDrivers = await db.Contacts
                .Where(c => c.CompanyId == companyId)
                .CountAsync();

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
            var countCompanies = await db.Companies.CountAsync();

            var countPages = GetCountPage(countCompanies, UserConstants.NormalPageCount);

            return countPages;
        }

        private async Task<List<Contact>> GetContactsInDb(int page, int companyId)
        {
            var contacts = db.Contacts
                .Include(c => c.PhoneNumber)
                .Where(c => c.CompanyId == companyId)
                .OrderByDescending(c => c.Id)
                .AsQueryable();

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

        private async Task<List<Company>> GetCompaniesWithUsersDb(int page)
        {
            var companies = db.Companies
                .Include(c => c.CompanyUsers)
                .ThenInclude(c => c.User)
                .OrderByDescending(c => c.Id)
                .AsQueryable();

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

        private bool IsHaveRequredDocs(List<DocumentCompany> docs)
        {
            var requredDocsNames = new List<string> {
                DocAndFileConstants.CertificateOfInsurance,
                DocAndFileConstants.McLetter };

            foreach (var item in requredDocsNames)
            {
                if (!docs.Any(x => x.Name == item)) return false;
            }

            return true;
        }
        private async Task<List<ContactViewModel>> GetContactsViewModelInDb(int page, string idCompany)
        {
            var contacts = db.Contacts.Where(c => c.CompanyId.ToString() == idCompany).AsQueryable();

            if (page == UserConstants.AllPagesNumber) return mapper.Map<List<ContactViewModel>>(await contacts.ToListAsync());

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

            return mapper.Map<List<ContactViewModel>>(listContacts);
        }

        private void UpdateTypeActiveSubById(int idSub, ActiveType activeType)
        {
            var subscribeSt = db.SubscribeST.FirstOrDefault(s => s.Id == idSub);
            
            if (subscribeSt == null) return;
            
            subscribeSt.ActiveType = activeType;
                
            db.SaveChanges();
        }
        
        private void UpdateTypeInactiveByIdCompany(string idCompany)
        {
            var subscribeSTs = db.SubscribeST
                .Where(s => s.CustomerST.CompanyId.ToString() == idCompany && s.ActiveType != ActiveType.Inactive)
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
            using (var dbt = new Context())
            {
                var companyUserInfo = await dbt.CompanyUsers.FirstOrDefaultAsync(cu => cu.CompanyId == id);
                if (companyUserInfo == null) return null;

                var companyInfo = await dbt.Companies.Include(c => c.PhoneNumber).FirstOrDefaultAsync(x => x.Id == companyUserInfo.CompanyId);
                if (companyInfo == null) return null;
                var userInfo = await dbt.Users.FirstOrDefaultAsync(x => x.Id == companyUserInfo.UserId);
                if (userInfo == null) return null;

                var companyViewModel = new CompanyViewModel
                {
                    Id = companyInfo.Id,
                    Name = companyInfo.Name,
                    PhoneNumber = companyInfo.PhoneNumber,
                    CompanyType = companyInfo.CompanyType,
                    CompanyStatus = companyInfo.CompanyStatus,
                    Email = userInfo.Email,
                };

                return companyViewModel;
            }
        }

        private async Task<List<Company>> GetCompaniesDb(int page)
        {
            var companies = db.Companies.AsQueryable();

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

        public void InitStripeForCompany(string nameCompany, string emailCompany, int companyId)
        {
            var customer = stripeApi.CreateCustomer(nameCompany, companyId, emailCompany.ToLower());

            var customerSt = new CustomerST()
            {
                DateCreated = customer.Created,
                CompanyId = companyId,
                IdCustomerST = customer.Id,
                NameCompanyST = customer.Name
            };

            SaveCustomerSt(customerSt);

            var subscription = stripeApi.CreateSupsctibe(customer.Id, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

            var subscribeSt = new SubscribeST()
            {
                CustomerSTId = customerSt.Id,
                ItemSubscribeSTId = subscription.Items.Data[0].Id,
                SubscribeSTId = subscription.Id,
                //Status = subscription.Status
            };
            
            SaveSubscribeSt(subscribeSt);
        }
        
        private void SaveCustomerSt(CustomerST customerSt)
        {
            db.CustomerST.Add(customerSt);
            db.SaveChanges();
        }
        
        private void SaveSubscribeSt(SubscribeST subscribeSt)
        {
            db.SubscribeST.Add(subscribeSt);
            db.SaveChanges();
        }
        
        public async Task SaveDocCompany(IFormFile uploadedFile, string nameDoc, int companyId, string localDate)
        {
            if (uploadedFile.Length > maxFileLength) return;

            var path = $"../Document/Copmpany/{companyId}/" + uploadedFile.FileName;
            
            if (!Directory.Exists("../Document/Copmpany"))
            {
                Directory.CreateDirectory($"../Document/Copmpany");
            }
            
            if (!Directory.Exists($"../Document/Copmpany/{companyId}"))
            {
                Directory.CreateDirectory($"../Document/Copmpany/{companyId}");
            }
            
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                uploadedFile.CopyTo(fileStream);
            }
            
            await SaveDocCompanyDb(path, companyId, nameDoc, localDate);
        }
        
        private async Task SaveDocCompanyDb(string path, int companyId, string nameDoc, string localDate)
        {
            var dateTimeUpload = string.IsNullOrEmpty(localDate) ? DateTime.Now : DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            var pref = path.Remove(0, path.LastIndexOf(".") + 1);
            
            var documentCompany = new DocumentCompany()
            {
                DocPath = path,
                Name = nameDoc,
                CompanyId = companyId,
                DateTimeUpload = dateTimeUpload
            };

            await db.DocumentsCompanies.AddAsync(documentCompany);
            
            await db.SaveChangesAsync();
        }

        private void SelectPaymentMethod_ST(string idCompany, string idPayment)
        {
            var paymentMethodSt = db.PaymentsMethods
                .FirstOrDefault(pm => pm.CompanyId.ToString() == idCompany && pm.PaymentMethodSTId == idPayment);

            if (paymentMethodSt == null) return;
            
            paymentMethodSt.IsDefault = true;
            db.SaveChanges();
        }
        
        private void UnSelectPaymentMethod_ST(string idCompany)
        {
            var paymentMethodSt = db.PaymentsMethods
                .FirstOrDefault(pm => pm.CompanyId.ToString() == idCompany && pm.IsDefault);

            if (paymentMethodSt == null) return;
            
            paymentMethodSt.IsDefault = false;
            db.SaveChanges();
        }
        
        private string RemovePaymentMethod(string idCompany, string idPayment)
        {
            string idPaymentNewSelect = null;
            var paymentMethodSt = db.PaymentsMethods.FirstOrDefault(pm => pm.CompanyId.ToString() == idCompany && pm.PaymentMethodSTId == idPayment);

            if (paymentMethodSt == null) return null;
            
            db.PaymentsMethods.Remove(paymentMethodSt);
            db.SaveChanges();
            
            if (!paymentMethodSt.IsDefault) return null;
            
            paymentMethodSt = db.PaymentsMethods.FirstOrDefault();
            
            if(paymentMethodSt != null)
            {
                idPaymentNewSelect = paymentMethodSt.PaymentMethodSTId;
            }

            return idPaymentNewSelect;
        }
        
        private void AddPaymentMethodSt(DaoModels.DAO.Models.PaymentMethod paymentMethodSt)
        {
            db.PaymentsMethods.Add(paymentMethodSt);
            
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
        
        private List<DaoModels.DAO.Models.PaymentMethod> GetPaymentMethod_STsByIdCompany(string idCompany)
        {
            return db.PaymentsMethods.Where(pm => pm.CompanyId.ToString() == idCompany).ToList();
        }
        
        public CustomerST GetCustomer_STByIdCompany(string companyId)
        {
            if (!int.TryParse(companyId, out var result)) return null;

            return db.CustomerST.FirstOrDefault(c => c.CompanyId == result);
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
            
            dispatcher.Key = token;
            db.SaveChanges();
        }
        
        private ContactViewModel GetContactById(int id)
        {
            var contact = db.Contacts
                .Include(c => c.PhoneNumber)
                .FirstOrDefault(c => c.Id == id);

            return mapper.Map<ContactViewModel>(contact);
        }

        private List<Dispatcher> GetDispatchersDb(int companyId)
        {
            return db.Dispatchers
                .Include(d => d.DispatcherType)
                .Where(d => d.CompanyId == companyId)
                .ToList();
        }
        
        private Dispatcher GetDispatcherByKeyUsers(string key)
        {
            return db.Dispatchers.FirstOrDefault(d => d.Key != null && d.Key == key);
        }
        
        private IEnumerable<User> GetUsers()
        {
            return db.Users.ToList();
        }

        private string GetUserAgent()
        {
            return httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();
        }

        private string GetIPAddress()
        {
            return httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
        }
    }
}