using System.ComponentModel.DataAnnotations;
using backend_dotnet.Enum;

namespace backend_dotnet.Entities;

public class Categories
{
    public Guid Id { get; set; }
    
    [Required (ErrorMessage = "A descrição não pode estar vazia")]
    [MaxLength(400, ErrorMessage = "A descrição não pode ter mais de 400 caracteres")]
    public string Description { get; set; }
    
    [Required(ErrorMessage = "A finalidade não pode estar vazia")]
    public Finance Purpose  { get; set; }
    
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}