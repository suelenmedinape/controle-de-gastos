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
            : StatusCode(500, result.Errors.FirstOrDefault());
    }

    /// <summary>
    /// Criar uma nova transação
    /// </summary>
    /// <param name="dto">Dados da transação</param>
    /// <returns>Retorna o id da transação criada</returns>
    /// <response code="201">transação criada com sucesso</response>
    /// <response code="400">Um ou mais erros de validação ocorreram</response>
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateTransactionDTO dto)
    {
        var result = await service.TransactionService.CreateTransaction(dto);
        return result.IsSuccess
            ? Created("", result.Successes.FirstOrDefault())
            : StatusCode(400, result.Errors.FirstOrDefault());
    }
}