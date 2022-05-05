using AutoMapper;
using WebDispacher.ViewModels.Mappings;

namespace WebDispacher
{
    public class AutoMapperConfig
    {
        public static void RegisterMappers()
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile<MappingProfile>();
            });

            mapperConfig.AssertConfigurationIsValid();
        }
    }
}