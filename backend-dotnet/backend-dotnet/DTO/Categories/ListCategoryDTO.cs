using backend_dotnet.Enum;

namespace backend_dotnet.DTO.Categories;

public class ListCategoryDTO
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public Finance Purpose { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Balance { get; set; }
}