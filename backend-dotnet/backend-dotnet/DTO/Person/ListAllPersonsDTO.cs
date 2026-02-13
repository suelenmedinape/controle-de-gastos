using backend_dotnet.Entities;

namespace backend_dotnet.DTO.Person;

public class ListAllPersonsDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public int TotalIncome { get; set; }
    public int TotalExpenses { get; set; }
    public int Balance { get; set; }
}