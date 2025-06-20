using AutoMapper;

namespace CoffeeBeanApi.Models;
public class CoffeeBeanProfile : Profile
{
    public CoffeeBeanProfile()
    {
        CreateMap<CoffeeBeanCreateInput, CoffeeBean>()
            .ForMember(dest => dest.OriginalId, opt => opt.Ignore())
            .ForMember(dest => dest.IsBeanOfTheDay, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Country, opt => opt.Ignore())
            .ForMember(dest => dest.Colour, opt => opt.Ignore());

        CreateMap<CoffeeBeanUpdateInput, CoffeeBean>()
            .IncludeBase<CoffeeBeanCreateInput, CoffeeBean>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}