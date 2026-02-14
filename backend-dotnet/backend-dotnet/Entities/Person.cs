using System.ComponentModel.DataAnnotations;

namespace backend_dotnet.Entities;

public class Person
{
    public Guid Id { get; set; }
    
    [Required (ErrorMessage = "O nome não pode estar vazio")]
    [MaxLength(200, ErrorMessage = "O nome não pode ter mais de 200 caracteres")]
    public string Name { get; set; }
    
    [Required (ErrorMessage = "A idade não pode estar vazia")]
    [Range(0, int.MaxValue, ErrorMessage = "A idade deve ser um valor válido")]
    public int Age { get; set; }
    
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}