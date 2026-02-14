using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend_dotnet.Enum;

namespace backend_dotnet.Entities;

public class Transaction
{
    public Guid Id { get; set; }
    
    [Required (ErrorMessage = "A descrição não pode estar vazia")]
    [MaxLength(400, ErrorMessage = "A descrição não pode ter mais de 400 caracteres")]
    public string Description { get; set; }
    
    [Required (ErrorMessage = "O valor não pode estar vazio")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser positivo")]
    public decimal Value { get; set; }
    
    [Required(ErrorMessage = "O tipo não pode estar vazio")]
    public Finance Type { get; set; }
    
    [Required(ErrorMessage = "A categoria é obrigatória")]
    public Guid CategoryId { get; set; }
    [Required(ErrorMessage = "A pessoa é obrigatória")]
    public Guid PersonId { get; set; }
        
    [ForeignKey("CategoryId")]
    public virtual Categories Category { get; set; }
    [ForeignKey("PersonId")]
    public virtual Person Person { get; set; }
}