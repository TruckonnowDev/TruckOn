using System.Collections.Generic;
using System.Threading.Tasks;
using DaoModels.DAO.DTO;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Http;
using Stripe;
using WebDispacher.Models;
using WebDispacher.Models.Subscription;

namespace WebDispacher.Business.Interfaces
{
    public interface ICompanyService
    {
        Dispatcher CheckKeyDispatcher(string key);
        List<Dispatcher> GetDispatchers(int idCompany);
        List<Commpany> GetCompanies();
        List<CompanyDTO> GetCompaniesDTO();
        Contact GetContact(int id);
        string RefreshTokenDispatch(string idDispatch);
        void CreateDispatch(string typeDispatcher, string login, string password, int idCompany);
        void EditContact(int id, string fullName, string emailAddress, string phoneNumber);
        void EditDispatch(int idDispatch, string typeDispatcher, string login, string password);
        void DeleteContactById(int id);
        Dispatcher GetDispatcherById(int idDispatch);
        void RemoveDispatchById(int idDispatch);
        ResponseStripe AddPaymentCard(string idCompany, string number, string name, string expiry, string cvc);
        void DeletePaymentMethod(string idPayment, string idCompany);
        Subscribe_ST GetSubscriptionIdCompany(string idCompany, ActiveType activeType = ActiveType.Active);
        ResponseStripe SelectDefaultPaymentMethod(string idPayment, string idCompany = null,
            Customer_ST customer_ST = null);

        List<PaymentMethod> GetpaymentMethod(string idCompany);
        List<PaymentMethod_ST> GetpaymentMethodsST(string idCompany);
        void RemoveDocCompany(string idDock);
        Task<List<DucumentCompany>> GetCompanyDoc(string id);
        void CreateContact(string fullName, string emailAddress, string phoneNumber, string idCompany);

        void AddCompany(string nameCommpany, string emailCommpany, IFormFile MCNumberConfirmation, IFormFile IFTA,
            IFormFile KYU, IFormFile logbookPapers, IFormFile COI, IFormFile permits);

        List<Contact> GetContacts(string idCompany);
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