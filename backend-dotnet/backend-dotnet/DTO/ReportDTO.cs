namespace backend_dotnet.DTO.Person;

public class ReportDTO<T> where T : class
{
    public List<T> FinancialTotals { get; set; }
    public SummaryDTO TotalSummary { get; set; }
}
