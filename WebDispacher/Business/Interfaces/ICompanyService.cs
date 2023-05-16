using System.Collections.Generic;
using System.Threading.Tasks;
using DaoModels.DAO.DTO;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Http;
using Stripe;
using WebDispacher.Models;
using WebDispacher.Models.Subscription;
using WebDispacher.ViewModels.Company;
using WebDispacher.ViewModels.Contact;
using WebDispacher.ViewModels.Dispatcher;
using WebDispacher.ViewModels.Payment;
using WebDispacher.ViewModels.RA.Carrier.Registration;

namespace WebDispacher.Business.Interfaces
{
    public interface ICompanyService
    {
        Dispatcher CheckKeyDispatcher(string key);
        List<Dispatcher> GetDispatchers(int idCompany);
        List<Commpany> GetCompanies();
        Task<CompanyViewModel> GetCompanyById(int id);
        Task<List<CompanyDTO>> GetCompaniesDTO(int page);
        Task<int> GetCountCompaniesPages();
        ContactViewModel GetContact(int id);
        Task EditCompany(CompanyViewModel company);
        string RefreshTokenDispatch(string idDispatch);
        void CreateDispatch(DispatcherViewModel dispatcher, int idCompany);
        void EditContact(ContactViewModel contact);
        void EditDispatch(DispatcherViewModel dispatcher);
        Task DeleteContactById(int id);
        DispatcherViewModel GetDispatcherById(int idDispatch);
        void RemoveDispatchById(int idDispatch);
        ResponseStripe AddPaymentCard(string idCompany, CardViewModel card);
        void DeletePaymentMethod(string idPayment, string idCompany);
        Subscribe_ST GetSubscriptionIdCompany(string idCompany, ActiveType activeType = ActiveType.Active);
        ResponseStripe SelectDefaultPaymentMethod(string idPayment, string idCompany = null,
            Customer_ST customer_ST = null);

        List<PaymentMethod> GetPaymentMethod(string idCompany);
        List<PaymentMethod_ST> GetPaymentMethodsST(string idCompany);
        void RemoveDocCompany(string idDock);
        Task<List<DucumentCompany>> GetCompanyDoc(string id);
        void CreateContact(ContactViewModel contact, string idCompany);

        Task AddShortCompany(FewMoreDetailsViewModel model);

        Task AddCompany(CreateCompanyViewModel model, IFormFile MCNumberConfirmation, IFormFile IFTA,
            IFormFile KYU, IFormFile logbookPapers, IFormFile COI, IFormFile permits, string dateTimeLocal);
        bool CheckCompanyName(string companyName);
        Task<List<Contact>> GetContacts(int page, string idCompany);
        Task<int> GetCountContactsPages(string idCompany);
        string GetTypeNavBar(string key, string idCompany, string typeNav = "Work");
        List<Models.Subscription.Subscription> GetSubscriptions();
        string SelectSub(string idPrice, string idCompany, string priodDays);
        void CancelSubscriptionsNext(string idCompany);
        SubscriptionCompanyDTO GetSubscription(string idCompany);
        bool GetCancelSubscribe(string idCompany);
        List<UserDTO> GetUsers(int idCompanySelect);
        Customer_ST GetCustomer_STByIdCompany(string idCompany);
        Task SaveDocCompany(IFormFile uploadedFile, string nameDoc, string id);
    }
}