using AutoMapper;
using DaoModels.DAO.Models;
using WebDispacher.Models;
using WebDispacher.ViewModels.Contact;
using WebDispacher.ViewModels.Dispatcher;
using WebDispacher.ViewModels.Driver;
using WebDispacher.ViewModels.Settings;
using WebDispacher.ViewModels.Trailer;
using WebDispacher.ViewModels.Truck;

namespace WebDispacher.ViewModels.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DriverReportModel, DriverReport>()
                .ForMember(x => x.Id, opt => opt.MapFrom(dr => dr.Id))
                .ForMember(x => x.Comment, opt => opt.MapFrom(dr => dr.Description))
                .ForMember(x => x.AlcoholTendency, opt => opt.MapFrom(dr => dr.AlcoholTendency))
                .ForMember(x => x.DrivingSkills, opt => opt.MapFrom(dr => dr.DrivingSkills))
                .ForMember(x => x.DrugTendency, opt => opt.MapFrom(dr => dr.DrugTendency))
                .ForMember(x => x.EldKnowledge, opt => opt.MapFrom(dr => dr.EldKnowledge))
                .ForMember(x => x.English, opt => opt.MapFrom(dr => dr.English))
                .ForMember(x => x.Experience, opt => opt.MapFrom(dr => dr.Experience))
                .ForMember(x => x.PaymentHandling, opt => opt.MapFrom(dr => dr.PaymentHandling))
                .ForMember(x => x.ReturnedEquipmen, opt => opt.MapFrom(dr => dr.ReturnedEquipmen))
                .ForMember(x => x.Terminated, opt => opt.MapFrom(dr => dr.Terminated))
                .ForMember(x => x.WorkingEfficiency, opt => opt.MapFrom(dr => dr.WorkingEfficiency))
                .ForMember(x => x.DotViolations, opt => opt.MapFrom(dr => dr.DotViolations))
                .ForMember(x => x.NumberOfAccidents, opt => opt.MapFrom(dr => dr.NumberOfAccidents))
                .ReverseMap();
            CreateMap<SettingsUserViewModel, Users>()
                .ForMember(x => x.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(x => x.Date, opt => opt.MapFrom(s => s.Date))
                .ForMember(x => x.Login, opt => opt.MapFrom(s => s.Login))
                .ForMember(x => x.Password, opt => opt.MapFrom(s => s.Password))
                .ForMember(x => x.CompanyId, opt => opt.MapFrom(s => s.CompanyId))
                .ForMember(x => x.KeyAuthorized, opt => opt.MapFrom(s => s.KeyAuthorized))
                .ReverseMap();
            CreateMap<DriverReportViewModel, DriverReport>()
                .ForMember(x => x.Id, opt => opt.MapFrom(d => d.Id))
                .ForMember(x => x.IdCompany, opt => opt.MapFrom(d => d.IdCompany))
                .ForMember(x => x.Comment, opt => opt.MapFrom(d => d.Comment))
                .ForMember(x => x.English, opt => opt.MapFrom(d => d.English))
                .ForMember(x => x.Experience, opt => opt.MapFrom(d => d.Experience))
                .ReverseMap();
            CreateMap<DriverViewModel, DaoModels.DAO.Models.Driver>()
                .ForMember(x => x.Id, opt => opt.MapFrom(cd => cd.Id))
                .ForMember(x => x.Password, opt => opt.MapFrom(cd => cd.Password))
                .ForMember(x => x.FullName, opt => opt.MapFrom(cd => cd.FullName))
                .ForMember(x => x.EmailAddress, opt => opt.MapFrom(cd => cd.EmailAddress))
                .ForMember(x => x.PhoneNumber, opt => opt.MapFrom(cd => cd.PhoneNumber))
                .ForMember(x => x.TrailerCapacity, opt => opt.MapFrom(cd => cd.TrailerCapacity))
                .ForMember(x => x.DriversLicenseNumber, opt => opt.MapFrom(cd => cd.DriversLicenseNumber))
                .ForMember(x => x.DateRegistration, opt => opt.MapFrom(cd => cd.DateRegistration))
                .ForMember(x => x.CompanyId, opt => opt.MapFrom(cd => cd.CompanyId))
                .ReverseMap();
            CreateMap<TrailerViewModel, DaoModels.DAO.Models.Trailer>()
                .ForMember(x => x.Id, opt => opt.MapFrom(t => t.Id))
                .ForMember(x => x.CompanyId, opt => opt.MapFrom(t => t.CompanyId))
                .ForMember(x => x.Name, opt => opt.MapFrom(t => t.Name))
                .ForMember(x => x.Year, opt => opt.MapFrom(t => t.Year))
                .ForMember(x => x.Make, opt => opt.MapFrom(t => t.Make))
                .ForMember(x => x.HowLong, opt => opt.MapFrom(t => t.HowLong))
                .ForMember(x => x.Vin, opt => opt.MapFrom(t => t.Vin))
                .ForMember(x => x.Owner, opt => opt.MapFrom(t => t.Owner))
                .ForMember(x => x.Color, opt => opt.MapFrom(t => t.Color))
                .ForMember(x => x.Plate, opt => opt.MapFrom(t => t.Plate))
                .ForMember(x => x.Exp, opt => opt.MapFrom(t => t.Exp))
                .ForMember(x => x.AnnualIns, opt => opt.MapFrom(t => t.AnnualIns))
                .ForMember(x => x.Type, opt => opt.MapFrom(t => t.Type))
                .ReverseMap();
            CreateMap<TruckViewModel, DaoModels.DAO.Models.Truck>()
                .ForMember(x => x.Id, opt => opt.MapFrom(t => t.Id))
                .ForMember(x => x.CompanyId, opt => opt.MapFrom(t => t.CompanyId))
                .ForMember(x => x.NameTruk, opt => opt.MapFrom(t => t.NameTruck))
                .ForMember(x => x.Yera, opt => opt.MapFrom(t => t.Year))
                .ForMember(x => x.Make, opt => opt.MapFrom(t => t.Make))
                .ForMember(x => x.Model, opt => opt.MapFrom(t => t.Model))
                .ForMember(x => x.Satet, opt => opt.MapFrom(t => t.State))
                .ForMember(x => x.Exp, opt => opt.MapFrom(t => t.Exp))
                .ForMember(x => x.Vin, opt => opt.MapFrom(t => t.Vin))
                .ForMember(x => x.Owner, opt => opt.MapFrom(t => t.Owner))
                .ForMember(x => x.PlateTruk, opt => opt.MapFrom(t => t.PlateTruck))
                .ForMember(x => x.ColorTruk, opt => opt.MapFrom(t => t.ColorTruck))
                .ForMember(x => x.Type, opt => opt.MapFrom(t => t.Type))
                .ReverseMap();
            CreateMap<ContactViewModel, DaoModels.DAO.Models.Contact>()
                .ForMember(x => x.ID, opt => opt.MapFrom(c => c.Id))
                .ForMember(x => x.Email, opt => opt.MapFrom(c => c.Email))
                .ForMember(x => x.Name, opt => opt.MapFrom(c => c.Name))
                .ForMember(x => x.Phone, opt => opt.MapFrom(c => c.Phone))
                .ForMember(x => x.CompanyId, opt => opt.MapFrom(c => c.CompanyId))
                .ReverseMap();
            CreateMap<DispatcherViewModel, DaoModels.DAO.Models.Dispatcher>()
                .ForMember(x => x.Id, opt => opt.MapFrom(d => d.Id))
                .ForMember(x => x.Login, opt => opt.MapFrom(d => d.Login))
                .ForMember(x => x.Password, opt => opt.MapFrom(d => d.Password))
                .ForMember(x => x.Type, opt => opt.MapFrom(d => d.Type))
                .ForMember(x => x.key, opt => opt.MapFrom(d => d.Key))
                .ForMember(x => x.IdCompany, opt => opt.MapFrom(d => d.IdCompany))
                .ReverseMap();
            CreateMap<ShippingViewModel, Shipping>()
                .ForMember(x => x.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(x => x.idOrder, opt => opt.MapFrom(s => s.IdOrder))
                .ReverseMap();
        }
    }
}