using AutoMapper;
using backend_dotnet.DTO.Person;
using backend_dotnet.Service;
using backend_dotnet.Utils;
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
    
    /// <summary>
    /// Lista todas as pessoas
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> ListPersonById(Guid id)
    {
        var result = await service.PersonService.ListPersonsById(id);
        return result.IsSuccess
            ? Ok(result.Successes.FirstOrDefault())
            : StatusCode(404, result.Errors.FirstOrDefault()); 
    }
    
    /// <summary>
    /// Lista todas as pessoas
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> ListAllPerson()
    {
        var result = await service.PersonService.ListAllPersons();
        return result.IsSuccess
            ? Ok(result.Successes.FirstOrDefault())
            : StatusCode(404, result.Errors.FirstOrDefault()); 
    }

    /// <summary>
    /// Registrar uma pessoa no sistema
    /// </summary>
    /// <param name="dto">Dados para cadastro</param>
    /// <returns>Retorna o id da pessoa criada</returns>
    /// <response code="201">Cadastro realizado com sucesso</response>
    /// <response code="400">Um ou mais erros de validação ocorreram</response>
    [HttpPost]
    public async Task<IActionResult> CreatePerson([FromBody] CreatePersonDTO dto)
    {
        var result = await service.PersonService.CreatePerson(dto);
        return result.IsSuccess
            ? Created("", result.Successes.FirstOrDefault())
            : StatusCode(400, result.Errors.FirstOrDefault());
    }

    /// <summary>
    /// Atualizar os dados
    /// </summary>
    /// <param name="id">Id da pessoa</param>
    /// <param name="dto">Dados para atualizar</param>
    /// <returns>Retorna uma mensagem</returns>
    /// <response code="201">Cadastro realizado com sucesso</response>
    /// <response code="400">Um ou mais erros de validação ocorreram</response>
    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> UpdatePerson(Guid id, [FromBody] UpdatePersonDTO dto)
    {
        var result = await service.PersonService.UpdatePerson(id, dto);
        return result.IsSuccess
            ? Ok(result.Successes.FirstOrDefault())
            : StatusCode(400, result.Errors.FirstOrDefault()); 
    }
}