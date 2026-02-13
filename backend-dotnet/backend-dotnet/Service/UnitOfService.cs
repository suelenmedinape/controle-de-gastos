using AutoMapper;
using backend_dotnet.Interface;

namespace backend_dotnet.Service;

public class UnitOfService
{
    private readonly IUnitOfWork unit;
    private readonly IMapper mapper;

    private PersonService personService;
    
    public UnitOfService(UnitOfWork unit, IMapper mapper)
    {
        this.unit = unit;
        this.mapper = mapper;
    }
    
    public PersonService PersonService => personService ??= new PersonService(unit, mapper);
}