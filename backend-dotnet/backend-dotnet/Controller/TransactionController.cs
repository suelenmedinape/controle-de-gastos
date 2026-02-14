using backend_dotnet.DTO.Person.Transaction;
using backend_dotnet.Service;
using Microsoft.AspNetCore.Mvc;

namespace backend_dotnet.Controller;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class TransactionController : ControllerBase
{
    private readonly UnitOfService service;
    
    public TransactionController(UnitOfService service)
    {
        this.service = service;
    }

    /// <summary>
    /// Listar todas as transações
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> ListTransactions()
    {
        var result = await service.TransactionService.ListTransactions();
        return result.IsSuccess
            ? Ok(result.Successes.FirstOrDefault())
            : StatusCode(404, result.Errors.FirstOrDefault());
    }

    /// <summary>
    /// Criar uma nova transação
    /// </summary>
    /// <param name="dto">Dados da transação</param>
    /// <returns>Retorna o id da transação criada</returns>
    /// <response code="201">transação criada com sucesso</response>
    /// <response code="400">Um ou mais erros de validação ocorreram</response>
    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDTO dto)
    {
        var result = await service.TransactionService.CreateTransaction(dto);
        return result.IsSuccess
            ? Created("", result.Successes.FirstOrDefault())
            : StatusCode(400, result.Errors.FirstOrDefault());
    }

    /// <summary>
    /// Listar todas as pessoas cadastradas com os dados dos gastos
    /// </summary>
    /// <returns>Retorna uma lista de pessoas</returns>
    /// <response code="201">Sucesso ao buscar dados</response>
    /// <response code="404">Nenhuma pessoa encontrada</response>
    [HttpGet("ReportPerson")]
    public async Task<IActionResult> GetTotalsByPerson()
    {
        var result = await service.TransactionService.GetTotalsByPerson();
        return result.IsSuccess
            ? Ok(result.Successes.FirstOrDefault())
            : StatusCode(404, result.Errors.FirstOrDefault());
    }
    
    /// <summary>
    /// Listar todas as categorias cadastradas com os dados dos gastos
    /// </summary>
    /// <returns>Retorna uma lista de categorias</returns>
    /// <response code="201">Sucesso ao buscar dados</response>
    /// <response code="404">Nenhuma pessoa encontrada</response>
    [HttpGet("ReportCategory")]
    public async Task<IActionResult> GetTotalsByCategory()
    {
        var result = await service.TransactionService.GetTotalsByCategory();
        return result.IsSuccess
            ? Ok(result.Successes.FirstOrDefault())
            : StatusCode(404, result.Errors.FirstOrDefault());
    }
}