using AutoMapper;
using backend_dotnet.DTO.Person;
using backend_dotnet.Entities;

namespace backend_dotnet.Utils;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreatePersonDTO, Person>();
        CreateMap<UpdatePersonDTO, Person>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}