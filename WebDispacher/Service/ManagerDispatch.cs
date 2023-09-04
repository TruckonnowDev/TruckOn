/*using BaceModel.ModelInspertionDriver;
using DaoModels.DAO.DTO;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Interface;
using DaoModels.DAO.Models;
using DaoModels.DAO.Models.Settings;
using Microsoft.AspNetCore.Http;
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebDispacher.Business.Interfaces;
using WebDispacher.Dao;
using WebDispacher.Models;
using WebDispacher.Models.Subscription;
using WebDispacher.Notify;
using WebDispacher.Service.EmailSmtp;
using WebDispacher.Service.TransportationManager;

namespace WebDispacher.Service
{
    public class ManagerDispatch
    {
        public SqlCommadWebDispatch _sqlEntityFramworke = null;
        public StripeApi stripeApi = null;

        public ManagerDispatch()
        {
            _sqlEntityFramworke = new SqlCommadWebDispatch();
            stripeApi = new StripeApi();
            //stripeApi.UpdateSupsctibe(0, "si_JL4mcwD3l5akXr");
        }

        internal Dispatcher CheckKeyDispatcher(string key)
        {
            return _sqlEntityFramworke.GetDispatcherByKeyUsers(key);
        }

        internal bool GetCancelSubscribe(string idCompany)
        {
            Commpany commpany = _sqlEntityFramworke.GetCompanyById(idCompany);
            if(commpany.Type == TypeCompany.BaseCommpany)
            {
                return false;
            }
            SubscriptionCompanyDTO subscriptionCompanyDTO = GetSubscription(idCompany);
            return subscriptionCompanyDTO.Status == "canceled";
        }

        public async Task<List<Driver>> GetDrivers(string idCompany)
        {
            return await _sqlEntityFramworke.GetDriversInDb(idCompany);
        }

        internal SubscriptionCompanyDTO GetSubscription(string idCompany)
        {
            SubscriptionCompanyDTO subscriptionCompanyDTO = new SubscriptionCompanyDTO();
            Subscribe_ST subscribe_ST = _sqlEntityFramworke.GetSubscriptionIdCompany(idCompany);
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

        public void DeletedOrder(string id)
        {
            _sqlEntityFramworke.RecurentOnDeleted(id);
        }

        internal List<Dispatcher> GetDispatchers(int idCompany)
        {
            return _sqlEntityFramworke.GetDispatchersDB(idCompany);
        }

        public void ArchvedOrder(string id)
        {
            _sqlEntityFramworke.RecurentOnArchived(id);
        }

        internal void CancelSubscriptionsNext(string idCompany)
        {
            Subscribe_ST subscribe_ST = _sqlEntityFramworke.GetSubscriptionIdCompany(idCompany);
            stripeApi.UpdateSubscribeCancelAtPeriodEnd(subscribe_ST.IdSubscribeST, false);
            Subscribe_ST subscribe_STNext = _sqlEntityFramworke.GetSubscriptionIdCompany(idCompany, ActiveType.NextActive);
            if(subscribe_STNext != null)
            {
                stripeApi.CanceleSubscribe(subscribe_STNext.IdSubscribeST);
                _sqlEntityFramworke.UpdateTypeActiveSubById(subscribe_STNext.Id, ActiveType.Inactive);
            }
        }

        internal string SelectSub(string idPrice, string idCompany, string priodDays)
        {
            string errorStr = "";
            Customer_ST customer_ST = _sqlEntityFramworke.GetCustomer_STByIdCompany(idCompany);
            Subscribe_ST subscribe_ST = _sqlEntityFramworke.GetSubscriptionIdCompany(idCompany);
            ResponseStripe responseStripe = stripeApi.GetSubscriptionSTById(subscribe_ST.IdSubscribeST);
            if(!responseStripe.IsError)
            {
                Stripe.Subscription subscription = responseStripe.Content as Stripe.Subscription;
                if(subscription.Status == "canceled")
                {
                    responseStripe = stripeApi.CreateSupsctibe(idPrice, customer_ST);
                    if (!responseStripe.IsError)
                    {
                        _sqlEntityFramworke.UpdateTypeInactiveByIdCompany(idCompany);
                        subscribe_ST = new Subscribe_ST();
                        subscription = responseStripe.Content as Stripe.Subscription;
                        subscribe_ST.IdCustomerST = subscription.CustomerId;
                        subscribe_ST.IdSubscribeST = subscription.Id;
                        subscribe_ST.IdCompany = Convert.ToInt32(idCompany);
                        subscribe_ST.Status = subscription.Status;
                        subscribe_ST.ActiveType = ActiveType.Active;
                        _sqlEntityFramworke.SaveSubscribeST(subscribe_ST);
                    }
                    else
                    {
                        errorStr = responseStripe.Message;
                    }
                    return errorStr;
                }
                stripeApi.UpdateSubscribeCancelAtPeriodEnd(subscription.Id, true);
                Subscribe_ST subscribe_STNext = _sqlEntityFramworke.GetSubscriptionIdCompany(idCompany, ActiveType.NextActive);
                if (subscribe_STNext != null)
                {
                    stripeApi.CanceleSubscribe(subscribe_STNext.IdSubscribeST);
                    _sqlEntityFramworke.UpdateTypeActiveSubById(subscribe_STNext.Id, ActiveType.Inactive);
                }
                subscribe_STNext = new Subscribe_ST();
                Stripe.Subscription subscriptionNext = stripeApi.CreateSupsctibeNext(customer_ST.IdCustomerST, idPrice, priodDays, subscription.CurrentPeriodEnd);
                subscribe_STNext.IdCustomerST = subscriptionNext.CustomerId;
                subscribe_STNext.IdSubscribeST = subscriptionNext.Id;
                subscribe_STNext.IdCompany = Convert.ToInt32(idCompany);
                subscribe_STNext.Status = subscriptionNext.Status;
                subscribe_STNext.ActiveType = ActiveType.NextActive;
                _sqlEntityFramworke.SaveSubscribeST(subscribe_STNext);
            }
            else
            {
                errorStr = responseStripe.Message;
            }
            return errorStr;
        }

        internal bool CheckEmail(string email)
        {
            bool isEmail = _sqlEntityFramworke.CheckEmail(email);
            if (isEmail)
            {
                string token = CreateToken(email);
                int idUser = _sqlEntityFramworke.AddRecoveryPassword(email, token);
                string patern = new PaternSourse().GetPaternRecoveryPassword($"{Config.BaseReqvesteUrl}/Recovery/Password?idUser={idUser}&token={token}");
                Task.Run(async () => await new AuthMessageSender().Execute(email, "Password recovery", patern));
            }
            return isEmail;
        }
        private string CreateToken(string email)
        {
            string token = "";
            for (int i = 0; i < email.Length; i++)
            {
                token += i * new Random().Next(1, 1000) + email[i];
            }
            return token;
        }

        internal List<PaymentMethod> GetpaymentMethod(string idCompany)
        {
            Customer_ST customer_ST = _sqlEntityFramworke.GetCustomer_STByIdCompany(idCompany);
            return stripeApi.GetPaymentMethodsByCustomerST(customer_ST.IdCustomerST);
        }

        internal List<PaymentMethod_ST> GetpaymentMethodsST(string idCompany)
        {
            return _sqlEntityFramworke.GetPaymentMethod_STsByIdCompany(idCompany);
        }

        internal int GetIdProfile(int idTr, string typeTransport)
        {
            int idProfile = 0;
            ITr tr = GetTr(idTr, typeTransport);
            TypeTransportVehikle typeTransportVehikle = tr is Truck ? TypeTransportVehikle.Truck : TypeTransportVehikle.Trailer;
            ProfileSetting profileSetting = _sqlEntityFramworke.GetIdProfile(idTr, typeTransportVehikle.ToString());
            if(profileSetting != null)
            {
                idProfile = profileSetting.Id;
            }
            return idProfile;  
        }

        public string GetTypeTransport(int idTr, string typeTransport)
        {
            ITr tr = GetTr(idTr, typeTransport);
            return tr is Truck ? TypeTransportVehikle.Truck.ToString() : TypeTransportVehikle.Trailer.ToString();
        }

        internal List<Models.Subscription.Subscription> GetSubscriptions()
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

        internal async Task<int> ResetPasswordFoUser(string newPassword, string idUser, string token)
        {
            int isStateActual = _sqlEntityFramworke.ResetPasswordFoUser(newPassword, idUser, token);
            if (isStateActual == 2)
            {
                string emailUser = _sqlEntityFramworke.GetEmailUserDb(idUser);
                string patern = new PaternSourse().GetPaternDataAccountUser(emailUser, newPassword);
                await new AuthMessageSender().Execute(emailUser, "Password changed successfully", patern);
            }
            else
            {
                string emailDriver = _sqlEntityFramworke.GetEmailUserDb(idUser);
                string patern = new PaternSourse().GetPaternNoRestoreDataAccountUser();
                await new AuthMessageSender().Execute(emailDriver, "Password reset attempt failed", patern);
            }
            return isStateActual;
        }

        internal int CheckTokenFoUser(string idUser, string token)
        {
            return _sqlEntityFramworke.CheckTokenFoUserDb(idUser, token);
        }

        internal List<CompanyDTO> GetCompanies()
        {
            return _sqlEntityFramworke.GetCompanies()
                .Select(z => new CompanyDTO()
                {
                    Id = z.Id,
                    Active = z.Active,
                    Name = z.Name,
                    Type = z.Type == TypeCompany.BaseCommpany ? "Home Company" : z.Type == TypeCompany.NormalCompany ? "Regular company" : "Unknown",
                    DateRegistration = z.DateRegistration
                }).ToList();
        }

        internal Users GetUserByEmailAndPasswrod(string email, string password)
        {
            Users users = _sqlEntityFramworke.GetUserByEmailAndPasswrodDB(email, password);
            if(users != null)
            {
                AddUserToDispatch(users);
            }
            return users;
        }

        *//* UserService *//*
        internal void AddUserToDispatch(Users users)
        {
            _sqlEntityFramworke.AddUserToDispatchDB(users);
        }

        internal List<ProfileSettingsDTO> GetSetingsTruck(string idCompany, int idProfile, int idTr, string typeTransport)
        {
            ITr tr = GetTr(idTr, typeTransport);
            TypeTransportVehikle typeTransportVehikle = tr is Truck ? TypeTransportVehikle.Truck : TypeTransportVehikle.Trailer;
            List <ProfileSettingsDTO> profileSettings = new List<ProfileSettingsDTO>();
            List<ProfileSetting> profileSettings1 = _sqlEntityFramworke.GetSetingsDb(idCompany, typeTransportVehikle, idTr);
            profileSettings.Add(new ProfileSettingsDTO()
            {
                Name = "Standart",
                TypeTransportVehikle = typeTransportVehikle.ToString(),
                Id = 0,
                IsSelect = 0 == idProfile,
                IsUsed = profileSettings1.FirstOrDefault(p => p.IsUsed) == null,
                IdCompany = 0
            });
            if (profileSettings1.Count != 0)
            {
                profileSettings.AddRange(profileSettings1.Select(z => new ProfileSettingsDTO()
                { 
                    Id = z.Id,
                    IsChange = true,
                    IsSelect = z.Id == idProfile,
                    Name = z.Name,
                    TransportVehicle = z.TransportVehicle,
                    TypeTransportVehikle = z.TypeTransportVehikle,
                    IsUsed = z.IsUsed,
                    IdCompany = z.IdCompany,
                    IdTr = z.IdTr
                }));
            }
            return profileSettings;
        }

        internal string RefreshTokenDispatch(string idDispatch)
        {
            string token = GetHash();
            _sqlEntityFramworke.RefreshTokenDispatchDB(idDispatch, token);
            return token;
        }

        internal Contact GetContact(int id)
        {
            return _sqlEntityFramworke.GetContactById(id);
        }

        internal void CreateDispatch(string typeDispatcher, string login, string password, int idCompany)
        {
            Dispatcher dispatcher = new Dispatcher()
            {
                Login = login,
                Password = password,
                Type = typeDispatcher,
                IdCompany = idCompany,
                key = GetHash(),
            };
            
            _sqlEntityFramworke.CreateDispatchDB(dispatcher);
        }

        public string GetHash()
        {
            var bytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            string hash = BitConverter.ToString(bytes).Replace("-", "").ToLower();
            return hash;
        }

        private string CreateTokenHashSha256(string token)
        {
            string tokenHashSha256 = null;
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(token));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                tokenHashSha256 = builder.ToString();
            }
            return tokenHashSha256;
        }

        internal void EditContact(int id, string fullName, string emailAddress, string phoneNumbe)
        {
            _sqlEntityFramworke.EditContact(id, fullName, emailAddress, phoneNumbe);
        }

        internal ProfileSettingsDTO GetSelectSetingTruck(string idCompany, int idProfile, int idTr, string typeTransport)
        {
            ProfileSettingsDTO profileSetting = null;
            if(idProfile != 0)
            {
                ProfileSetting profileSetting1 = _sqlEntityFramworke.GetSelectSeting(idCompany, idProfile);
                if (profileSetting1 != null)
                {
                    profileSetting = new ProfileSettingsDTO()
                    {
                        Id = profileSetting1.Id,
                        IsChange = true,
                        IsSelect = idProfile == profileSetting1.Id,
                        Name = profileSetting1.Name,
                        TransportVehicle = profileSetting1.TransportVehicle,
                        TypeTransportVehikle = profileSetting1.TypeTransportVehikle,
                        IsUsed = profileSetting1.IsUsed,
                        IdCompany = profileSetting1.IdCompany
                    };
                }
            }
            else
            {
                ITr tr = GetTr(idTr, typeTransport);
                ITransportVehicle transportVehicle = HelperTransport.GetTransportVehicle(tr.Type);
                profileSetting = new ProfileSettingsDTO()
                {
                    Id = 0,
                    Name = "Standart",
                    TransportVehicle = new TransportVehicle()
                    {
                        CountPhoto = transportVehicle.CountPhoto,
                        Id = 0,
                        Layouts = GetLayoutsByTransportVehicle(transportVehicle),
                        Type = transportVehicle.Type,
                        TypeTransportVehicle = transportVehicle.TypeTransportVehicle
                    },
                    TypeTransportVehikle = "Truck",
                    IsChange = false,
                    IsUsed = true,
                    IdCompany = 0
                };
            }
            profileSetting.TransportVehicle.Layouts = profileSetting.TransportVehicle.Layouts.OrderBy(l => l.OrdinalIndex).ToList();
            
            return profileSetting;
        }

        internal void DeleteContact(int id)
        {
            _sqlEntityFramworke.DeleteContactById(id);
        }

        internal void EditDispatch(int idDispatch, string typeDispatcher, string login, string password)
        {
            _sqlEntityFramworke.EditDispatchById(idDispatch, typeDispatcher, login, password);
        }

        internal Dispatcher GetDispatcher(int idDispatch)
        {
            return _sqlEntityFramworke.GetDispatcherById(idDispatch);
        }

        internal void RemoveDispatch(int idDispatch)
        {
            _sqlEntityFramworke.RemoveByIdDispatcher(idDispatch);
        }

        internal ResponseStripe AddPaymentCard(string idCompany, string number, string name, string expiry, string cvc)
        {
            ResponseStripe responseStripe = stripeApi.CreatePaymentMethod(number.Replace(" ", ""), name, expiry, cvc);
            if (responseStripe != null && !responseStripe.IsError)
            {
                PaymentMethod paymentMethod = (PaymentMethod)responseStripe.Content;
                Customer_ST customer_ST = _sqlEntityFramworke.GetCustomer_STByIdCompany(idCompany);
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
                    _sqlEntityFramworke.AddPaymentMethod_ST(paymentMethod_ST);
                }
            }
            return responseStripe;
        }

        *//* CompanyService *//*
        private bool CheckFirstPaymentMethodInCompany(string idCompany)
        {
            bool isFirstPaymentMethodInCompany = false;
            List<PaymentMethod_ST> paymentMethod_STs = _sqlEntityFramworke.GetPaymentMethod_STsByIdCompany(idCompany);
            if(paymentMethod_STs != null && paymentMethod_STs.Count != 0)
            {
                isFirstPaymentMethodInCompany = true;
            }
            return isFirstPaymentMethodInCompany;
        }
        
        *//* CompanyService *//*
        internal ResponseStripe SelectDefaultPaymentMethod(string idPayment, string idCompany = null, Customer_ST customer_ST = null)
        {
            if(customer_ST == null)
            {
                customer_ST = _sqlEntityFramworke.GetCustomer_STByIdCompany(idCompany);
            }
            ResponseStripe responseStripe = stripeApi.SelectDefaultPaymentMethod(idPayment, customer_ST.IdCustomerST);
            if (responseStripe != null && !responseStripe.IsError)
            {
                _sqlEntityFramworke.UnSelectPaymentMethod_ST(idCompany);
                _sqlEntityFramworke.SelectPaymentMethod_ST(idCompany, idPayment);
            }
            return responseStripe;
        }

        internal Truck GetTruckById(int idTruck)
        {
            return (Truck)_sqlEntityFramworke.GetTruckById(idTruck);
        }

        internal Trailer GetTrailerById(int idTrailer)
        {
            return (Trailer)_sqlEntityFramworke.GetTrailerById(idTrailer);
        }

        *//* CompanyService *//*
        internal void InitStripeForCompany(string nameCommpany, string emailCommpany, int idCompany)
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
            _sqlEntityFramworke.SaveCustomerST(customer_ST);
            _sqlEntityFramworke.SaveSubscribeST(subscribe_ST);
        }

        internal void EditTruck(int idTruck, string nameTruk, string yera, string make, string model, string typeTruk, string state, string exp, string vin, string owner, string plateTruk, string color)
        {
            _sqlEntityFramworke.EditTruckDB(idTruck, nameTruk, yera, make, model, typeTruk, state, exp, vin, owner, plateTruk, color);
        }

        internal void EditTrailer(int idTrailer, string name, string typeTrailer, string year, string make, string howLong, string vin, string owner, string color, string plate, string exp, string annualIns)
        {
            _sqlEntityFramworke.EditTrailerDB(idTrailer, name, typeTrailer, year, make, howLong, vin, owner, color, plate, exp, annualIns);
        }

        internal void DeletePaymentMethod(string idPayment, string idCompany)
        {
            stripeApi.DeletePaymentMethod(idPayment);
            string idPaymentNewSelect = _sqlEntityFramworke.RemovePaymentMethod(idCompany, idPayment);
            if(idPaymentNewSelect != null)
            {
                SelectDefaultPaymentMethod(idPaymentNewSelect, idCompany);
            }
        }
        
        *//* TruckAndTrailerService *//*
        private ITr GetTr(int idTr, string typeTransport)
        {
            ITr tr = null;
            if (typeTransport == "Truck")
            {
                tr = _sqlEntityFramworke.GetTruckById(idTr);
            }
            else if (typeTransport == "Trailer")
            {
                tr = _sqlEntityFramworke.GetTrailerById(idTr);
            }
            return tr;
        }

        *//* UserService *//*
        private List<Layouts> GetLayoutsByTransportVehicle(ITransportVehicle transportVehicle)
        {
            List<Layouts> layouts = new List<Layouts>();
            for(int i = 0; i < transportVehicle.Layouts.Count; i++)
            {
                layouts.Add(new Layouts()
                {
                    Id = 0,
                    Index = transportVehicle.Layouts[i],
                    Name = transportVehicle.NamePatern[i],
                    OrdinalIndex = i+1,
                    IsUsed = true,
                });
            }
            return layouts;
        }

        internal void SelectLayout(int idLayout)
        {
            _sqlEntityFramworke.SelectLayout(idLayout);
        }

        internal void RemoveProfile(string idCompany, int idProfile)
        {
            _sqlEntityFramworke.RemoveProfiledb(idCompany, idProfile);
        }

        internal int AddProfile(string idCompany, int idTr, string typeTransport)
        {
            ITr tr = GetTr(idTr, typeTransport);
            ITransportVehicle transportVehicle = HelperTransport.GetTransportVehicle(tr.Type);
            ProfileSetting profileSetting = new ProfileSetting()
            {
                TransportVehicle = new TransportVehicle()
                {
                    CountPhoto = transportVehicle.CountPhoto,
                    Layouts = GetLayoutsByTransportVehicle(transportVehicle),
                    Type = transportVehicle.Type,
                    TypeTransportVehicle = transportVehicle.TypeTransportVehicle
                },
                IdCompany = Convert.ToInt32(idCompany),
                Name = "Custom",
                TypeTransportVehikle = GetTypeTransport(idTr, typeTransport),
                IdTr = idTr
            };
            return _sqlEntityFramworke.AddProfileDb(profileSetting);
        }

        internal ProfileSettingsDTO GetSelectSetingTrailer(string idCompany, int idProfile)
        {
            ProfileSettingsDTO profileSetting = null;
            if (idProfile != 0)
            {
                ProfileSetting profileSetting1 = _sqlEntityFramworke.GetSelectSeting(idCompany, idProfile);
                if (profileSetting1 != null)
                {
                    profileSetting = new ProfileSettingsDTO()
                    {
                        Id = profileSetting1.Id,
                        IsChange = true,
                        IsSelect = idProfile == profileSetting1.Id,
                        Name = profileSetting1.Name,
                        TransportVehicle = profileSetting1.TransportVehicle,
                        TypeTransportVehikle = profileSetting1.TypeTransportVehikle,
                        IsUsed = profileSetting1.IsUsed,
                        IdCompany = profileSetting1.IdCompany
                    };
                }
            }
            else
            {
                profileSetting = new ProfileSettingsDTO()
                {
                    Id = 0,
                    Name = "Standart",
                    //TransportVehicles = new StandartProfileSettings(TypeTransportVehikle.Trailer).TransportVehicles,
                    TypeTransportVehikle = "Trailer",
                    IsChange = false,
                    IsUsed = true,
                    IdCompany = 0
                };
            }
            profileSetting.TransportVehicle.Layouts = profileSetting.TransportVehicle.Layouts.OrderBy(l => l.OrdinalIndex).ToList();
            return profileSetting;
        }

        internal void LayoutUP(int idLayout, int idTransported)
        {
            _sqlEntityFramworke.LayoutUPDb(idLayout, idTransported);
        }

        internal void SelectProfile(int idProfile, string typeTransport, int idTr, string idCompany)
        {
            TypeTransportVehikle typeTransportVehikle = typeTransport == "Truck" ? TypeTransportVehikle.Truck : TypeTransportVehikle.Trailer;
            List<ProfileSetting> profileSettings = _sqlEntityFramworke.GetSetingsDb(idCompany, typeTransportVehikle, idTr);
            ProfileSetting profileSetting = profileSettings.FirstOrDefault(p => p.IsUsed);
            if (idProfile != 0)
            {
                _sqlEntityFramworke.SelectProfileDb(idProfile);
            }
            if (profileSetting != null)
            {
                _sqlEntityFramworke.SelectProfileDb(profileSetting.Id, false);
            }
        }

        internal void UnSelectLayout(int idLayout)
        {
            _sqlEntityFramworke.UnSelectLayout(idLayout);
        }

        internal bool IsPermission(string key, string idCompany, string route)
        {
            bool isPermission = false;
            Users users = _sqlEntityFramworke.GetUserByKey(key);
            Commpany commpany = _sqlEntityFramworke.GetCompanyById(idCompany);
            if(users != null && commpany != null)
            {
                isPermission = ValidCompanyRoute(commpany.Type, route);
            }
            return isPermission;
        }

        internal string GetTypeNavBar(string key, string idCompany, string typeNav = "Work")
        {
            string typeNavBar = "";
            Users users = _sqlEntityFramworke.GetUserByKey(key);
            Commpany commpany = _sqlEntityFramworke.GetCompanyById(idCompany);
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

        internal void LayoutDown(int idLayout, int idTransported)
        {
            _sqlEntityFramworke.LayoutDownDb(idLayout, idTransported);
        }
        
        *//* UserService *//*
        private bool ValidCompanyRoute(TypeCompany typeCompany, string route)
        {
            bool validCompany = false;
            if(route == "Company")
            {
                if (typeCompany == TypeCompany.BaseCommpany)
                {
                    validCompany = true;
                }
            }
            else
            {
                validCompany = true;
            }
            return validCompany;
        }

        internal async Task<DaoModels.DAO.Models.Shipping> AddNewOrder(string urlPage, Dispatcher dispatcher)
        {
            ITransportationDispatch transportationDispatch = GetTransportationDispatch(dispatcher.Type);
            DaoModels.DAO.Models.Shipping shipping = await transportationDispatch.GetShipping(urlPage, dispatcher);
            if (shipping != null)
            {
                _sqlEntityFramworke.AddOrder(shipping);
            }
            return shipping;
        }
        
        *//* Company service *//*
        private ITransportationDispatch GetTransportationDispatch(string typeDispatch)
        {
            ITransportationDispatch transportationDispatch = null;
            switch (typeDispatch)
            {
                case "Central Dispatch": transportationDispatch = new GetDataCentralDispatch(); break;
            }
            return transportationDispatch;
        }

        public List<Truck> GetTrucks(string idCompany)
        {
            return _sqlEntityFramworke.GetTrucs(idCompany);
        }

        public List<Contact> GetContacts(string idCompany)
        {
            return _sqlEntityFramworke.GetContactsDB(idCompany);
        }

        internal int[] GetIdTruckAdnTrailar(string idDriver)
        {
            return _sqlEntityFramworke.GetIdTruckAdnTrailarDb(idDriver);
        }

        internal async Task<Trailer> GetTrailer(string idDriver)
        {
            return await _sqlEntityFramworke.GetTrailerDb(idDriver);
        }

        internal Commpany GetUserByKeyUser(int key)
        {
            return _sqlEntityFramworke.GetUserByKeyUser(key);
        }

        internal void AddUser(string idCompany, string login, string password)
        {
            Users users = new Users()
            {
                CompanyId = Convert.ToInt32(idCompany),
                Date = DateTime.Now.ToString(),
                Login = login,
                Password = password
            };
            _sqlEntityFramworke.AddUserDb(users);
        }

        internal int GetUserByKey(int key)
        {
            return _sqlEntityFramworke.GetComapnyIdByKeyUser(key);
        }

        internal async Task<Truck> GetTruck(string idDriver)
        {
            return await _sqlEntityFramworke.GetTruckDb(idDriver);
        }

        public async void SaveVechi(string idVech, string VIN, string Year, string Make, string Model, string Type, string Color, string LotNumber)
        {
            VehiclwInformation vehiclwInformation = new VehiclwInformation();
            vehiclwInformation.VIN = VIN;
            vehiclwInformation.Year = Year;
            vehiclwInformation.Make = Make;
            vehiclwInformation.Model = Model;
            vehiclwInformation.Type = Type;
            vehiclwInformation.Color = Color;
            vehiclwInformation.Lot = LotNumber;
            _sqlEntityFramworke.SavevechInDb(idVech, vehiclwInformation);
        }

        internal async void AddCommpany(string nameCommpany, string emailCommpany, IFormFile MCNumberConfirmation, IFormFile IFTA, IFormFile KYU, IFormFile logbookPapers, IFormFile COI, IFormFile permits)
        {
            Commpany commpany = new Commpany()
            {
                Active = true,
                DateRegistration = DateTime.Now.ToString(),
                Name = nameCommpany,
                Type = TypeCompany.NormalCompany
            };
            int id = _sqlEntityFramworke.AddCommpany(commpany);
            InitStripeForCompany(nameCommpany, emailCommpany, id);
            _sqlEntityFramworke.CreateUserForCompanyId(id, nameCommpany, CreateToken(nameCommpany, new Random().Next(10, 1000).ToString()));
            await SaveDocCpmmpany(MCNumberConfirmation, "MC number confirmation", id.ToString());
            if (IFTA != null)
            {
                await SaveDocCpmmpany(IFTA, "IFTA (optional for 26000+)", id.ToString());
            }
            await SaveDocCpmmpany(KYU, "KYU", id.ToString());
            await SaveDocCpmmpany(logbookPapers, "Logbook Papers (manual, certificate, malfunction letter)", id.ToString());
            await SaveDocCpmmpany(COI, "COI (certificate of insurance)", id.ToString());
            await SaveDocCpmmpany(permits, "Permits (optional OR, FL, NM)", id.ToString());

        }

        private Models.Subscription.Subscription GetSubscribeSTById(int idSubscription)
        {
            Models.Subscription.Subscription subscribeST = null;
            List<Models.Subscription.Subscription> subscriptions = GetSubscriptions();
            subscribeST = subscriptions.FirstOrDefault(s => s.Id == idSubscription);
            return subscribeST;
        }

        internal void RemoveUserById(string idUser)
        {
            _sqlEntityFramworke.RemoveUserByIdDb(idUser);
        }

        internal async void RemoveCompany(string idCompany)
        {
            _sqlEntityFramworke.RemoveCompanyDb(idCompany);
            Customer_ST customer_ST = _sqlEntityFramworke.GetCustomer_STByIdCompany(idCompany);
            Customer customer = stripeApi.RemoveCustomer(customer_ST);
            if(customer != null)
            {
                _sqlEntityFramworke.RemoveCustomerST(customer_ST);
                _sqlEntityFramworke.RemoveSubscribeST(customer_ST);
            }
            Task.Run(() =>
            {
                List<Driver> drivers = _sqlEntityFramworke.GetDriversByIdCompany(idCompany);
                foreach(Driver driver in drivers)
                {
                    RemoveDrive(idCompany, new DriverReportModel{Id = driver.Id, Description = "The site administration deleted the company in which this driver worked"});
                }
            });
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

        internal async Task<Truck> GetTruckByPlate(string truckPlate)
        {
            return await _sqlEntityFramworke.GetTruckByPlateDb(truckPlate);
        }

        internal int CheckTokenFoDriver(string idDriver, string token)
        {
            return _sqlEntityFramworke.CheckTokenFoDriverDb(idDriver, token);
        }

        internal async Task<int> ResetPasswordFoDriver(string newPassword, string idDriver, string token)
        {
            int isStateActual = _sqlEntityFramworke.ResetPasswordFoDriver(newPassword, idDriver, token);
            if(isStateActual == 2)
            {
                string emailDriver = _sqlEntityFramworke.GetEmailDriverDb(idDriver);
                string patern = new PaternSourse().GetPaternDataAccountDriver(emailDriver, newPassword);
                await new AuthMessageSender().Execute(emailDriver, "Password changed successfully", patern);
            }
            else
            {
                string emailDriver = _sqlEntityFramworke.GetEmailDriverDb(idDriver);
                string patern = new PaternSourse().GetPaternNoRestoreDataAccountDriver();
                await new AuthMessageSender().Execute(emailDriver, "Password reset attempt failed", patern);
            }
            return isStateActual;
        }

        internal void AddHistory(string key, string idConmpany, string idOrder, string idVech, string idDriver, string action)
        {
            HistoryOrder historyOrder = new HistoryOrder();
            int idUser = _sqlEntityFramworke.GetUserIdByKey(key);
            if(action == "Assign")
            {
                historyOrder.TypeAction = "Assign";
            }
            else if(action == "Unassign")
            {
                idDriver = _sqlEntityFramworke.GetDriverIdByIdOrder(idOrder);
                historyOrder.TypeAction = "Unassign";
            }
            else if (action == "Solved")
            {
                historyOrder.TypeAction = "Solved";
            }
            else if (action == "ArchivedOrder")
            {
                historyOrder.TypeAction = "ArchivedOrder";
            }
            else if (action == "DeletedOrder")
            {
                historyOrder.TypeAction = "DeletedOrder";
            }
            else if (action == "Creat")
            {
                historyOrder.TypeAction = "Creat";
            }
            else if (action == "SavaOrder")
            {
                historyOrder.TypeAction = "SavaOrder";
            }
            else if (action == "SavaVech")
            {
                idOrder = _sqlEntityFramworke.GetIdOrderByIdVech(idVech);
                historyOrder.TypeAction = "SavaVech";
            }
            else if (action == "RemoveVech")
            {
                idOrder = _sqlEntityFramworke.GetIdOrderByIdVech(idVech);
                historyOrder.TypeAction = "RemoveVech";
            }
            else if (action == "AddVech")
            {
                historyOrder.TypeAction = "AddVech";
            }

            historyOrder.IdConmpany = Convert.ToInt32(idConmpany);
            historyOrder.IdDriver = Convert.ToInt32(idDriver);
            historyOrder.IdOreder = idOrder;
            historyOrder.IdVech = Convert.ToInt32(idVech);
            historyOrder.IdUser = idUser;
            historyOrder.DateAction = DateTime.Now.ToString();
            _sqlEntityFramworke.AddHistory(historyOrder);
        }

        internal List<UserDTO> GetUsers()
        {
            List<Commpany> commpanies = _sqlEntityFramworke.GetCompanies();
            return _sqlEntityFramworke.GetUsers()
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

        internal List<UserDTO> GetUsers(int idCompanySelect)
        {
            List<Commpany> commpanies = _sqlEntityFramworke.GetCompanies();
            return _sqlEntityFramworke.GetUsers()
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

        public string GetStrAction(string key, string idConmpany, string idOrder, string idVech, string idDriver, string action)
        {
            string strAction = "";
            //int idUser = _sqlEntityFramworke.GetUserIdByKey(key);
            if (action == "Assign")
            {
                string fullNameUser = _sqlEntityFramworke.GetFullNameUserByKey(key);
                string fullNameDriver = _sqlEntityFramworke.GetFullNameDriverById(idDriver);
                strAction = $"{fullNameUser} assign the driver ordered {fullNameDriver}";
            }
            else if (action == "Unassign")
            {
                string fullNameUser = _sqlEntityFramworke.GetFullNameUserByKey(key);
                string fullNameDriver = _sqlEntityFramworke.GetFullNameDriverById(idDriver);
                strAction = $"{fullNameUser} withdrew an order from {fullNameDriver} driver";
            }
            else if (action == "Solved")
            {
                string fullNameUser = _sqlEntityFramworke.GetFullNameUserByKey(key);
                strAction = $"{fullNameUser} clicked on the \"Solved\" button";
            }
            else if (action == "ArchivedOrder")
            {
                string fullNameUser = _sqlEntityFramworke.GetFullNameUserByKey(key);
                strAction = $"{fullNameUser} transferred the order to the archive";
            }
            else if (action == "DeletedOrder")
            {
                string fullNameUser = _sqlEntityFramworke.GetFullNameUserByKey(key);
                strAction = $"{fullNameUser} transferred the order to deleted orders";
            }
            else if (action == "Creat")
            {
                string fullNameUser = _sqlEntityFramworke.GetFullNameUserByKey(key);
                strAction = $"{fullNameUser} created an order";
            }
            else if (action == "SavaOrder")
            {
                string fullNameUser = _sqlEntityFramworke.GetFullNameUserByKey(key);
                strAction = $"{fullNameUser} edited the order";
            }
            else if (action == "SavaVech")
            {
                string fullNameUser = _sqlEntityFramworke.GetFullNameUserByKey(key);
                VehiclwInformation vehiclwInformation = _sqlEntityFramworke.GetVechById(idVech);
                strAction = $"{fullNameUser} edited the vehicle {vehiclwInformation.Year} y. {vehiclwInformation.Make} {vehiclwInformation.Model}";
            }
            else if (action == "RemoveVech")
            {
                string fullNameUser = _sqlEntityFramworke.GetFullNameUserByKey(key);
                VehiclwInformation vehiclwInformation = _sqlEntityFramworke.GetVechById(idVech);
                strAction = $"{fullNameUser} removed the vehicle {vehiclwInformation.Year} y. {vehiclwInformation.Make} {vehiclwInformation.Make}";
            }
            else if (action == "AddVech")
            {
                string fullNameUser = _sqlEntityFramworke.GetFullNameUserByKey(key);
                strAction = $"{fullNameUser} created a vehicle";
            }
            return strAction;
        }

        internal async Task<List<DucumentDriver>> GetDriverDoc(string id)
        {
            return await _sqlEntityFramworke.GetDriverDoc(id);
        }

        internal int CheckReportDriver(string fullName, string driversLicenseNumber)
        {
            return _sqlEntityFramworke.CheckReportDriverDb(fullName, driversLicenseNumber);
        }

        internal async Task<Trailer> GetTrailerkByPlate(string trailerPlate)
        {
            return await _sqlEntityFramworke.GetTrailerByPlateDb(trailerPlate);
        }

        internal List<DriverReport> GetDriversReport(string nameDriver, string driversLicense)
        {
            return _sqlEntityFramworke.GetDriversReportsDb(nameDriver, driversLicense);
        }

        internal void AddNewReportDriver(string fullName, string driversLicenseNumber, string numberOfAccidents, string english, string returnedEquipmen, string workingEfficiency, string eldKnowledge, string drivingSkills,
            string paymentHandling, string alcoholTendency, string drugTendency, string terminated, string experience, string dotViolations, string description)
        {
            if (driversLicenseNumber != null && driversLicenseNumber != "")
            {
                _sqlEntityFramworke.AddNewReportDriverDb(fullName, driversLicenseNumber, numberOfAccidents, english, returnedEquipmen, workingEfficiency, eldKnowledge, drivingSkills, paymentHandling, alcoholTendency, drugTendency, 
                    terminated, experience, dotViolations, description);
            }
        }

        internal List<Trailer> GetTrailers(string idCompany)
        {
            return _sqlEntityFramworke.GetTrailersDb(idCompany);
        }

        internal void RemoveTruck(string id)
        {
            _sqlEntityFramworke.RemoveTruck(id);
        }

        public async void RemoveVechi(string idVech)
        {
            _sqlEntityFramworke.RemoveVechInDb(idVech);
        }

        public async Task<VehiclwInformation> AddVechi(string idOrder)
        {
            return await _sqlEntityFramworke.AddVechInDb(idOrder);
        }

        public async Task<DaoModels.DAO.Models.Shipping> CreateShiping()
        {
            return await _sqlEntityFramworke.CreateShipping();
        }

        public DaoModels.DAO.Models.Shipping GetShipingCurrentVehiclwIn(string id)
        {
            return _sqlEntityFramworke.GetShipingCurrentVehiclwInDb(id);
        }

        public bool Avthorization(string login, string password)
        {
            return _sqlEntityFramworke.ExistsDataUser(login, password);
        }

        public bool CheckKey(string key)
        {
            return key != null && _sqlEntityFramworke.CheckKeyDb(key);
        }

        public List<Driver> GetDrivers(int pag, string idCompany)
        {
            return _sqlEntityFramworke.GetDrivers(pag, idCompany);
        }

        internal void RemoveDocDriver(string idDock)
        {
            _sqlEntityFramworke.RemoveDocDriverDb(idDock);
        }

        public async void Assign(string idOrder, string idDriver)
        {
            bool isDriverAssign = _sqlEntityFramworke.CheckDriverOnShipping(idOrder);
            string tokenShope = null;
            if (isDriverAssign)
            {
                tokenShope = _sqlEntityFramworke.GerShopTokenForShipping(idOrder);
            }
            List<VehiclwInformation> vehiclwInformations = await _sqlEntityFramworke.AddDriversInOrder(idOrder, idDriver);
             Task.Run(() =>
            {
                ManagerNotifyWeb managerNotify = new ManagerNotifyWeb();
                string tokenShope1 = _sqlEntityFramworke.GerShopTokenForShipping(idOrder);
                if (!isDriverAssign)
                {
                    managerNotify.SendNotyfyAssign(idOrder, tokenShope1, vehiclwInformations);
                }
                else
                {
                    managerNotify.SendNotyfyAssign(idOrder, tokenShope1, vehiclwInformations);
                    managerNotify.SendNotyfyUnassign(idOrder, tokenShope, vehiclwInformations);
                }
            });
        }

        public async void CreateTruk(string nameTruk, string yera, string make, string model, string typeTruk, string state, string exp, string vin, string owner, string plateTruk,
            string color, string idCompany, IFormFile truckRegistrationDoc, IFormFile truckLeaseAgreementDoc, IFormFile truckAnnualInspection, IFormFile bobTailPhysicalDamage, IFormFile nYHUTDoc)
        {
            int id = _sqlEntityFramworke.CreateTrukDb(nameTruk, yera, make, model, typeTruk, state, exp, vin, owner, plateTruk, color, idCompany);
            await SaveDocTruck(truckRegistrationDoc, "Truck registration", id.ToString());
            await SaveDocTruck(truckLeaseAgreementDoc, "Truck lease agreement", id.ToString());
            await SaveDocTruck(truckAnnualInspection, "Truck annual inspection", id.ToString());
            if (bobTailPhysicalDamage != null)
            {
                await SaveDocTruck(bobTailPhysicalDamage, "Bob tail physical damage", id.ToString());
            }
            if (bobTailPhysicalDamage != null)
            {
                await SaveDocTruck(bobTailPhysicalDamage, "NY HUT", id.ToString());
            }
        }

        public void CreateContact(string fullName, string emailAddress, string phoneNumbe, string idCompany)
        {
            Contact contact = new Contact();
            contact.Email = emailAddress;
            contact.Name = fullName;
            contact.Phone = phoneNumbe;
            contact.Phone = phoneNumbe;
            contact.CompanyId = Convert.ToInt32(idCompany);
            _sqlEntityFramworke.SaveNewContact(contact);
        }

        public async void Unassign(string idOrder)
        {
            ManagerNotifyWeb managerNotify = new ManagerNotifyWeb();
            string tokenShope = _sqlEntityFramworke.GerShopTokenForShipping(idOrder);
            List<VehiclwInformation> vehiclwInformations = await _sqlEntityFramworke.RemoveDriversInOrder(idOrder);
            Task.Run(() =>
            {
                managerNotify.SendNotyfyUnassign(idOrder, tokenShope, vehiclwInformations);
            });
        }

        public void Solved(string idOrder)
        {
            _sqlEntityFramworke.Solved(idOrder);
        }

        //public Shipping GetDriver()
        //{
        //    return _sqlEntityFramworke.GetShipping(id);
        //}

        public int Createkey(string login, string password)
        {
            Random random = new Random();
            int key = random.Next(1000, 1000000000);
            _sqlEntityFramworke.SaveKeyDatabays(login, password, key);
            return key;
        }

        internal void RestoreDrive(int id)
        {
            _sqlEntityFramworke.RestoreDriveInDb(id);
        }

        public async Task<int> GetCountPage(string status, string name, string address, string phone, string email, string price)
        {
            return await _sqlEntityFramworke.GetCountPageInDb(status, name, address, phone, email, price);
        }

        internal void RemoveTrailer(string id)
        {
            _sqlEntityFramworke.RemoveTrailerDb(id);
        }

        public async Task<List<DaoModels.DAO.Models.Shipping>> GetOrders(string status, int page, string name, string address, string phone, string email, string price)
        {
            return await _sqlEntityFramworke.GetShippings(status, page, name, address, phone, email, price);
        }

        public DaoModels.DAO.Models.Shipping GetOrder(string id)
        {
            return _sqlEntityFramworke.GetShipping(id);
        }

        public Driver GetDriver(int id)
        {
            return _sqlEntityFramworke.GetDriver(id);
        }

        public Driver GetDriver(string idInspection)
        {
            return _sqlEntityFramworke.GetDriver(idInspection);
        }


        public void Updateorder(string idOrder, string idLoad, string internalLoadID, string driver, string status, string instructions, string nameP, string contactP,
            string addressP, string cityP, string stateP, string zipP, string phoneP, string emailP, string scheduledPickupDateP, string nameD, string contactD, string addressD,
            string cityD, string stateD, string zipD, string phoneD, string emailD, string ScheduledPickupDateD, string paymentMethod, string price, string paymentTerms, string brokerFee,
            string contactId, string phoneC, string faxC, string iccmcC)
        {
            _sqlEntityFramworke.UpdateorderInDb(idOrder, idLoad, internalLoadID, driver, status, instructions, nameP, contactP, addressP, cityP, stateP, zipP,
                        phoneP, emailP, scheduledPickupDateP, nameD, contactD, addressD, cityD, stateD, zipD, phoneD, emailD, ScheduledPickupDateD, paymentMethod,
                        price, paymentTerms, brokerFee, contactId, phoneC, faxC, iccmcC);
        }

        public void CreateOrder(string idOrder, string idLoad, string internalLoadID, string driver, string status, string instructions, string nameP, string contactP,
            string addressP, string cityP, string stateP, string zipP, string phoneP, string emailP, string scheduledPickupDateP, string nameD, string contactD, string addressD,
            string cityD, string stateD, string zipD, string phoneD, string emailD, string ScheduledPickupDateD, string paymentMethod, string price, string paymentTerms, string brokerFee,
            string contactId, string phoneC, string faxC, string iccmcC)
        {
            _sqlEntityFramworke.UpdateorderInDb(idOrder, idLoad, internalLoadID, driver, status, instructions, nameP, contactP, addressP, cityP, stateP, zipP,
                        phoneP, emailP, scheduledPickupDateP, nameD, contactD, addressD, cityD, stateD, zipD, phoneD, emailD, ScheduledPickupDateD, paymentMethod,
                        price, paymentTerms, brokerFee, contactId, phoneC, faxC, iccmcC);
        }

        internal async void CreateTrailer(string name, string typeTrailer, string year, string make, string howLong, string vin, string owner, string color, string plate, string exp, string annualIns,
           string idCompany, IFormFile trailerRegistrationDoc, IFormFile trailerAnnualInspectionDoc, IFormFile leaseAgreementDoc)
        {
            int id = _sqlEntityFramworke.CreateTrailerDb(name, typeTrailer, year, make, howLong, vin, owner, color, plate, exp, annualIns, idCompany);
            await SaveDocTrailer(trailerRegistrationDoc, "Trailer registration", id.ToString());
            await SaveDocTrailer(trailerAnnualInspectionDoc, "Trailer annual inspection", id.ToString());
            if (leaseAgreementDoc != null)
            {
                await SaveDocTrailer(leaseAgreementDoc, "Lease agreement", id.ToString());
            }
        }

        public async void CreateDriver(string fullName, string emailAddress, string password, string phoneNumbe, string trailerCapacity, string driversLicenseNumber, string idCompany,
            IFormFile dLDoc, IFormFile medicalCardDoc, IFormFile sSNDoc, IFormFile proofOfWorkAuthorizationOrGCDoc, IFormFile dQLDoc, IFormFile contractDoc, IFormFile drugTestResultsDo)
        {
            Driver driver = new Driver();
            driver.FullName = fullName;
            driver.EmailAddress = emailAddress;
            driver.Password = password;
            driver.PhoneNumber = phoneNumbe;
            driver.TrailerCapacity = trailerCapacity;
            driver.DriversLicenseNumber = driversLicenseNumber;
            driver.DateRegistration = DateTime.Now.ToString();
            driver.CompanyId = Convert.ToInt32(idCompany);
            int id = _sqlEntityFramworke.AddDriver(driver);
            await SaveDocDriver(dLDoc, "DL", id.ToString());
            await SaveDocDriver(medicalCardDoc, "Medical card", id.ToString());
            await SaveDocDriver(sSNDoc, "SSN", id.ToString());
            await SaveDocDriver(proofOfWorkAuthorizationOrGCDoc, "Proof of work Authorization or GC", id.ToString());
            await SaveDocDriver(dQLDoc, "DQL (driver qualification file)", id.ToString());
            await SaveDocDriver(contractDoc, "Contract (company and driver)", id.ToString());
            if (drugTestResultsDo != null)
            {
                await SaveDocDriver(drugTestResultsDo, "Drug test results", id.ToString());
            }
            Task.Run(() => UpdatePlanSubscribe(idCompany));
        }

        private void UpdatePlanSubscribe(string idCompany)
        {
            TypeCompany typeCurrentCompany = _sqlEntityFramworke.GetCompanyById(idCompany).Type;
            if(typeCurrentCompany != TypeCompany.BaseCommpany)
            {
                int counDriver =  _sqlEntityFramworke.GetDriversByIdCompany(idCompany).Count;
                string idItemSubscribe = _sqlEntityFramworke.GetSubscriptionIdCompany(idCompany).IdItemSubscribeST;
                stripeApi.UpdateSupsctibe(counDriver, idItemSubscribe);
            }
        }

        internal void SavePath(string id, string path)
        {
            _sqlEntityFramworke.SavePathDb(id, path);
        }

        public List<InspectionDriver> GetInspectionTrucks(string idDriver, string idTruck, string idTrailer, string date)
        {
            return _sqlEntityFramworke.GetInspectionTrucksDb(idDriver, idTruck, idTrailer, date);
        }

        public void RemoveDrive(string idCompany, DriverReportModel model)
        {
            _sqlEntityFramworke.RemoveDriveInDb(model);
            Task.Run(() => UpdatePlanSubscribe(idCompany));
        }

        internal async Task<string> GetDocument(string id)
        {
            return _sqlEntityFramworke.GetDocumentDb(id);
        }

        public void EditDrive(int id, string fullName, string emailAddress, string password, string phoneNumbe, string trailerCapacity, string driversLicenseNumber)
        {;
            _sqlEntityFramworke.UpdateDriver(id, fullName, emailAddress, password, phoneNumbe, trailerCapacity, driversLicenseNumber);
        }

        public async Task<List<DocumentTruckAndTrailers>> GetTruckDoc(string id)
        {
            return await _sqlEntityFramworke.GetTruckDocDB(id);
        }

        public async Task<List<DucumentCompany>> GetCompanyDoc(string id)
        {
            return await _sqlEntityFramworke.GetCompanyDocDB(id);
        }

        public InspectionDriver GetInspectionTruck(string idInspection)
        {
            return _sqlEntityFramworke.GetInspectionTruck(idInspection);
        }

        internal async Task SaveDocTruck(IFormFile uploadedFile, string nameDoc, string id)
        {
            string path = $"../Document/Truck/{id}/" + uploadedFile.FileName;
            if(!Directory.Exists("../Document/Truck"))
            {
                Directory.CreateDirectory($"../Document/Truck");
            }
            if (!Directory.Exists($"../Document/Truck/{id}"))
            {
                Directory.CreateDirectory($"../Document/Truck/{id}");
            }
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                uploadedFile.CopyTo(fileStream);
            }
            _sqlEntityFramworke.SaveDocTruckDb(path, id, nameDoc);
        }

        internal async Task SaveDocTrailer(IFormFile uploadedFile, string nameDoc, string id)
        {
            string path = $"../Document/Traile/{id}/" + uploadedFile.FileName;
            if (!Directory.Exists("../Document/Traile"))
            {
                Directory.CreateDirectory($"../Document/Traile");
            }
            if (!Directory.Exists($"../Document/Traile/{id}"))
            {
                Directory.CreateDirectory($"../Document/Traile/{id}");
            }
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                uploadedFile.CopyTo(fileStream);
            }
            _sqlEntityFramworke.SaveDocTrailekDb(path, id, nameDoc);
        }

        internal async Task SaveDocCpmmpany(IFormFile uploadedFile, string nameDoc, string id)
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
            _sqlEntityFramworke.SaveDocCommpanyDb(path, id, nameDoc);
        }

        internal async Task SaveDocDriver(IFormFile uploadedFile, string nameDoc, string id)
        {
            string path = $"../Document/Driver/{id}/" + uploadedFile.FileName;
            if (!Directory.Exists("../Document/Driver"))
            {
                Directory.CreateDirectory($"../Document/Driver");
            }
            if (!Directory.Exists($"../Document/Driver/{id}"))
            {
                Directory.CreateDirectory($"../Document/Driver/{id}");
            }
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                uploadedFile.CopyTo(fileStream);
            }
            _sqlEntityFramworke.SaveDocDriverDb(path, id, nameDoc);
        }

        internal async Task<List<DocumentTruckAndTrailers>> GetTraileDoc(string id)
        {
            return await _sqlEntityFramworke.GetTrailerDocDB(id);
        }

        internal void RemoveDoc(string idDock)
        {
            _sqlEntityFramworke.RemoveDocDb(idDock);
        }

        internal void RemoveDocCompany(string idDock)
        {
            _sqlEntityFramworke.RemoveDocCompanyDb(idDock);
        }

        internal bool SendRemindInspection(int idDriver)
        {
            ManagerNotifyWeb managerNotifyWeb = new ManagerNotifyWeb();
            bool isInspactionDriverToDay = _sqlEntityFramworke.CheckInspactionDriverToDay(idDriver);
            if (!isInspactionDriverToDay)
            {
                string tokenShiping = _sqlEntityFramworke.GerShopToken(idDriver.ToString());
                managerNotifyWeb.SendSendNotyfyRemindInspection(tokenShiping);
            }
            return isInspactionDriverToDay;
        }

        internal List<HistoryOrder> GetHistoryOrder(string idOrder)
        {
            return _sqlEntityFramworke.GetHistoryOrderByIdOrder(idOrder);
        }
    }
}*/