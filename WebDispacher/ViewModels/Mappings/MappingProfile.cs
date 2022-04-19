using AutoMapper;
using DaoModels.DAO.Models;
using WebDispacher.Models;

namespace WebDispacher.ViewModels.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DriverReportModel, DriverReport>()
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
            
        }
    }
}