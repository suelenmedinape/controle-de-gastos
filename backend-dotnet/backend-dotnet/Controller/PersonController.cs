using AutoMapper;
using backend_dotnet.DTO.Person;
using backend_dotnet.Service;
using Microsoft.AspNetCore.Mvc;

namespace backend_dotnet.Controller;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class PersonController : ControllerBase
{
    private readonly UnitOfService service;
    
    public PersonController(UnitOfService service)
    {
        this.service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePerson([FromBody] CreatePersonDTO dto)
    {
        var result = await service.PersonService.CreatePerson(dto);
        return result.IsSuccess
            ? Created("Criado", result.Successes.FirstOrDefault())
            : StatusCode(400, result.Errors.FirstOrDefault());
    }
}