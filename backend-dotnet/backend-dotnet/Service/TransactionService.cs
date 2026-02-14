using AutoMapper;
using backend_dotnet.DTO.Categories;
using backend_dotnet.DTO.Person;
using backend_dotnet.DTO.Person.Transaction;
using backend_dotnet.Entities;
using backend_dotnet.Enum;
using backend_dotnet.Interface;
using FluentResults;
using Microsoft.EntityFrameworkCore;

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
            var pessoa = await unit.PersonRepository.findById(dto.PersonId) ??
                         throw new Exception("Pessoa não encontrada");
            if (pessoa.Age < 18 && dto.Type != Finance.Expenses)
            {
                throw new Exception("Pessosas menores de 18 podem ter apenas despesas");
            }

            var category = await unit.CategoriesRepository.findById(dto.CategoryId) ??
                           throw new Exception("Categoria não encontrada");

            if (dto.Type == Finance.Expenses && category.Purpose == Finance.Income)
            {
                throw new Exception("Não é possível usar uma categoria de Receita para uma transação de Despesa.");
            }

            if (dto.Type == Finance.Income && category.Purpose == Finance.Expenses)
            {
                throw new Exception("Não é possível usar uma categoria de Despesa para uma transação de Receita.");
            }

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

    private async Task<List<ListAllPersonsDTO>> TotalsByPerson()
    {
        return await unit.PersonRepository
            .Query()
            .Select(p => new ListAllPersonsDTO
            {
                Id = p.Id,
                Name = p.Name,
                Age = p.Age,
                TotalIncome = p.Transactions
                    .Where(t => t.Type == Finance.Income)
                    .Sum(t => (decimal?)t.Value) ?? 0,
                TotalExpenses = p.Transactions
                    .Where(t => t.Type == Finance.Expenses)
                    .Sum(t => (decimal?)t.Value) ?? 0,
            })
            .Select(x => new ListAllPersonsDTO
            {
                Id = x.Id,
                Name = x.Name,
                Age = x.Age,
                TotalIncome = x.TotalIncome,
                TotalExpenses = x.TotalExpenses,
                Balance = x.TotalIncome - x.TotalExpenses
            })
            .ToListAsync();
    }

    public async Task<Result> GetTotalsByPerson()
    {
        try
        {
            var financialTotals = await TotalsByPerson();

            var totalSummary = new SummaryDTO
            {
                totalIncome = financialTotals.Sum(x => x.TotalIncome),
                totalExpenses = financialTotals.Sum(x => x.TotalExpenses),
                netBalance = financialTotals.Sum(x => x.Balance)
            };

            var report = new ReportDTO<ListAllPersonsDTO>
            {
                FinancialTotals = financialTotals,
                TotalSummary = totalSummary
            };

            return new Result()
                .WithSuccess(new Success("Busca realizada com sucesso")
                    .WithMetadata("data", report));
        }
        catch (Exception e)
        {
            return new Result().WithError(new Error(e.Message));
        }
    }

    private async Task<List<ListCategoryDTO>> TotalsByCategory()
    {
        return await unit.CategoriesRepository
            .Query()
            .Select(p => new ListCategoryDTO()
            {
                Id = p.Id,
                Description = p.Description,
                Purpose = p.Purpose,
                TotalIncome = p.Transactions
                    .Where(t => t.Type == Finance.Income)
                    .Sum(t => (decimal?)t.Value) ?? 0,
                TotalExpenses = p.Transactions
                    .Where(t => t.Type == Finance.Expenses)
                    .Sum(t => (decimal?)t.Value) ?? 0,
            })
            .Select(x => new ListCategoryDTO
            {
                Id = x.Id,
                Description = x.Description,
                Purpose = x.Purpose,
                TotalIncome = x.TotalIncome,
                TotalExpenses = x.TotalExpenses,
                Balance = x.TotalIncome - x.TotalExpenses
            })
            .ToListAsync();
    }

    public async Task<Result> GetTotalsByCategory()
    {
        try
        {
            var financialTotals = await TotalsByCategory();

            var totalSummary = new SummaryDTO
            {
                totalIncome = financialTotals.Sum(x => x.TotalIncome),
                totalExpenses = financialTotals.Sum(x => x.TotalExpenses),
                netBalance = financialTotals.Sum(x => x.Balance)
            };

            var report = new ReportDTO<ListCategoryDTO>
            {
                FinancialTotals = financialTotals,
                TotalSummary = totalSummary
            };

            return new Result()
                .WithSuccess(new Success("Busca realizada com sucesso")
                    .WithMetadata("data", report));
        }
        catch (Exception e)
        {
            return new Result().WithError(new Error(e.Message));
        }
    }
}