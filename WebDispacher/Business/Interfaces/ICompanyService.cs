using System.Collections.Generic;
using System.Threading.Tasks;
using DaoModels.DAO.DTO;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Http;
using Stripe;
using WebDispacher.Constants;
using WebDispacher.Models;
using WebDispacher.Models.Subscription;
using WebDispacher.ViewModels.Company;
using WebDispacher.ViewModels.Contact;
using WebDispacher.ViewModels.Dispatcher;
using WebDispacher.ViewModels.Payment;
using WebDispacher.ViewModels.RA.Carrier.Registration;
using WebDispacher.ViewModels.Resources;

namespace WebDispacher.Business.Interfaces
{
    public interface ICompanyService
    {
        Task<int> GetCountCompaniesByStatus(CompanyStatus status);
        Task<UserMailQuestion> GetUserQuestionWithAnswers(int id);
        Task<List<UserMailQuestion>> GetUserQuestions();
        Task<List<UserMailQuestion>> GetUserQuestionsWithoutAnswers(int page);
        Task<List<UserMailQuestion>> GetUserQuestionsWithAnswers(int page);
        Task<List<UserMailQuestion>> GetAllUserQuestions(int page);
        Task<int> GetCountNewUserQuestionsPages();
        Task<int> GetCountAnsweredUserQuestionsPages();
        Task<int> GetCountAllUserQuestionsPages();
        Task SendUserAnswer(AdminAnswerViewModel model, string companyId, string localDate);
        Task<bool> CreateUserQuestions(UserMailQuestion userMailQuestion, string localDate);
        Task<Company> GetActiveUserCompanyByCompanyTypeUserId(string userId, CompanyType companyType);
        Task<Company> CreateCompany(FewMoreDetailsViewModel model, CompanyType companyType = CompanyType.Carrier);
        Task AddUserToCompany(User user, Company company);
        void InitStripeForCompany(string nameCompany, string emailCompany, int companyId);
        Task<List<Contact>> GetContactsByCompanyId(int page, string companyId);
        Task UpdateCompanyStatus(int companyId, CompanyStatus companyStatus);
        Task<int> GetNewUserMailQuestions();
        Task<bool> ActivateCompany(string companyId);
        Task<bool> UploadCompanyRequiredDoc(IFormFile certificateOfInsurance, IFormFile mcLetter, string companyId);
        Task<bool> CheckCompanyRequiredDoc(string idCompany);
        Task<List<ContactViewModel>> GetContactsViewModels(int page, string idCompany);
        Task <User> GetUserByCompanyId(string companyId);
        Task<bool> SendEmailToUserByCompanyId(string companyId, string subject, string message);
        //Task<List<CompanyViewModel>> GetCompaniesViewModels(int page);
        Task<List<Company>> GetCompaniesWithUsers(int page);
        Dispatcher CheckKeyDispatcher(string key);
        List<Dispatcher> GetDispatchers(int idCompany);
        List<Company> GetCompanies();
        Task<CompanyViewModel> GetCompanyById(int id);
        Task<int> GetCountCompaniesPages();
        ContactViewModel GetContact(int id);
        Task EditCompany(CompanyViewModel model, string localDate);
        string RefreshTokenDispatch(string idDispatch);
        void CreateDispatch(DispatcherViewModel dispatcher, int idCompany);
        Task EditContact(ContactViewModel model, string localDate);
        void EditDispatch(DispatcherViewModel dispatcher);
        Task DeleteContactById(int id);
        DispatcherViewModel GetDispatcherById(int idDispatch);
        void RemoveDispatchById(int idDispatch);
        ResponseStripe AddPaymentCard(string idCompany, CardViewModel card);
        void DeletePaymentMethod(string idPayment, string idCompany);
        Task<SubscribeST> GetSubscriptionIdCompany(string idCompany, ActiveType activeType = ActiveType.Active);
        ResponseStripe SelectDefaultPaymentMethod(string idPayment, string idCompany = null,
            CustomerST customer_ST = null);

        List<Stripe.PaymentMethod> GetPaymentMethod(string idCompany);
        List<DaoModels.DAO.Models.PaymentMethod> GetPaymentMethodsST(string idCompany);
        Task RemoveDocCompany(int docId);
        Task<List<DocumentCompany>> GetCompanyDoc(string id);
        Task CreateContact(ContactViewModel model, string companyId, string localDate);

        //Task AddShortCompany(FewMoreDetailsViewModel model);

        Task<Company> CreateCompanyByAdminWithDocs(CreateCompanyViewModel model,
            IFormFile MCNumberConfirmation, IFormFile IFTA, IFormFile KYU, IFormFile logbookPapers,
            IFormFile COI, IFormFile permits, string dateTimeLocal);
        bool CheckCompanyName(string companyName);
        Task<int> GetCountContactsPages(string idCompany);
        string GetTypeNavBar(string companyId, string typeNav = NavConstants.Work);
        List<Models.Subscription.Subscription> GetSubscriptions();
        Task<string> SelectSub(string priceId, string companyId, string priodDays);
        Task CancelSubscriptionsNext(string companyId);
        Task<SubscriptionCompanyDTO> GetSubscription(string companyId);
        bool GetCancelSubscribe(string idCompany);
        List<UserDTO> GetUsers(int idCompanySelect);
        CustomerST GetCustomer_STByIdCompany(string companyId);
        Task SaveDocCompany(IFormFile uploadedFile, string nameDoc, int companyId, string localDate);
        Task SetViewInUserMailQuestion(ViewUserMailQuestion model);
        Task<ResourceViewModel> GetResourceById(int id);
        Task<List<ResourceViewModel>> GetAllResources();
        Task<ResourceViewModel> EditResource(ResourceViewModel model, string localDate);
        Task UpdatePositionResources(List<ResourceViewModel> updates);
    }
}