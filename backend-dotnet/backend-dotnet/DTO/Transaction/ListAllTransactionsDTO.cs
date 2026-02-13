using backend_dotnet.Enum;

namespace backend_dotnet.DTO.Person.Transaction;

public class ListAllTransactionsDTO
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public decimal Value { get; set; }
    public Finance Type { get; set; }
    public Guid CategoryId { get; set; }
    public Guid PersonId { get; set; }
}