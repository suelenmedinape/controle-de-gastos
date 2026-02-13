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

    public async Task<Result> ListPersonsById(Guid id)
    {
        try
        {
            var persons = await unit.PersonRepository.findById(id) ?? throw new Exception("Pessoa não encontrada");
            
            return new Result()
                .WithSuccess(new Success("Sucesso ao buscar pessoas").WithMetadata("data", persons));
        }
        catch (Exception e)
        {
            return new Result().WithError(new Error(e.Message));
        }
    }
    
    public async Task<Result> ListAllPersons()
    {
        try
        { 
            // AINDA NÃO ESTÁ PRONTO
            var persons = await unit.PersonRepository.ListAll();
            
            return new Result()
                .WithSuccess(new Success("Sucesso ao buscar pessoas").WithMetadata("data", persons));
        }
        catch (Exception e)
        {
            return new Result().WithError(new Error(e.Message));
        }
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

    public async Task<Result> UpdatePerson(Guid id, UpdatePersonDTO dto)
    {
        try
        {
            var person = await unit.PersonRepository.findById(id) ?? throw new Exception("Pessoa não encontrada");
            
            mapper.Map(dto, person);
            unit.PersonRepository.Update(person);
            await unit.CommitAsync();
            
            return new Result()
                .WithSuccess(new Success("Dados atualizados com sucesso").WithMetadata("data", id));
        }
        catch (Exception e)
        {
            return new Result().WithError(new Error(e.Message));
        }
    }
}