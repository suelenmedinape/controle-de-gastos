namespace backend_dotnet.DTO.Categories;

public class ListCategoryDTO
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public int Purpose { get; set; }
    public int TotalIncome { get; set; }
    public int TotalExpenses { get; set; }
    public int Balance { get; set; }
}