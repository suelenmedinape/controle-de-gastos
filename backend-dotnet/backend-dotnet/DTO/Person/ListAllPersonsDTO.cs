using backend_dotnet.Entities;

namespace backend_dotnet.DTO.Person;

public class ListAllPersonsDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Balance { get; set; }
}