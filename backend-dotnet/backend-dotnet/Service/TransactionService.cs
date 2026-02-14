using AutoMapper;
using backend_dotnet.DTO.Person.Transaction;
using backend_dotnet.Entities;
using backend_dotnet.Enum;
using backend_dotnet.Interface;
using FluentResults;

namespace backend_dotnet.Service;

public class TransactionService
{
    private readonly IUnitOfWork unit;
    private readonly IMapper mapper;

    public TransactionService(IUnitOfWork unit, IMapper mapper)
    {
        this.unit = unit;
        this.mapper = mapper;
    }
    
    public async Task<Result> ListTransactions()
    {
        try
        {
            var transactions = await unit.TransactionRepository.ListAll();
            var transactionDto = mapper.Map<List<ListAllTransactionsDTO>>(transactions);
            
            return new Result()
                .WithSuccess(new Success("Categorias listadas com sucesso")
                    .WithMetadata("data", transactionDto));
        }
        catch (Exception e)
        {
            return new Result().WithError(new Error(e.Message));
        }
    }
    
    public async Task<Result> CreateTransaction(CreateTransactionDTO dto)
    {
        try
        {
            
            var pessoa = await unit.PersonRepository.findById(dto.PersonId) ?? throw new Exception("Pessoa não encontrada");
            if (pessoa.Age < 18 && dto.Type != Finance.Despesas)
            {
                throw new Exception("Pessosas menores de 18 podem ter apenas despesas");
            }
            
            var category = await unit.CategoriesRepository.findById(dto.CategoryId) ?? throw new Exception("Categoria não encontrada");
            
            var transaction = mapper.Map<Transaction>(dto);
            
            unit.TransactionRepository.Add(transaction);
            await unit.CommitAsync();
            
            return new Result()
                .WithSuccess(new Success("Transação criada com sucesso")
                    .WithMetadata("data", transaction.Id));
        }
        catch (Exception e)
        {
            return new Result().WithError(new Error(e.Message));
        }
    }
}