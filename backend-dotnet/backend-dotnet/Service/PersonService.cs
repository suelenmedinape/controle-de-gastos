using AutoMapper;
using backend_dotnet.Data;
using backend_dotnet.DTO.Person;
using backend_dotnet.Entities;
using backend_dotnet.Interface;
using FluentResults;

namespace backend_dotnet.Service;

public class PersonService
{
    private readonly IUnitOfWork unit;
    private readonly IMapper mapper;

    public PersonService(IUnitOfWork unit, IMapper mapper)
    {
        this.unit = unit;
        this.mapper = mapper;
    }
    
    public async Task<Result> CreatePerson(CreatePersonDTO dto)
    {
        try
        {
            var person = mapper.Map<Person>(dto);
            
            unit.PersonRepository.Add(person);
            await unit.CommitAsync();
            
            return new Result()
                .WithSuccess(new Success("Cadastro realizado com sucesso").WithMetadata("data", person.Id));
        }
        catch (Exception e)
        {
            return new Result().WithError(new Error(e.Message));
        }
    }
}