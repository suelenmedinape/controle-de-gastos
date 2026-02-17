using backend_dotnet.Enum;

namespace backend_dotnet.DTO.Person.Transaction;

public class ListAllTransactionsDTO
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public decimal Value { get; set; }
    public Finance Type { get; set; }
    public string CategoryDescription { get; set; }
    public string PersonName { get; set; }
}