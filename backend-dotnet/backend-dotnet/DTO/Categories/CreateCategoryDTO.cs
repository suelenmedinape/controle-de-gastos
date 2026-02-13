using System.ComponentModel.DataAnnotations;
using backend_dotnet.Enum;

namespace backend_dotnet.DTO.Categories;

public class CreateCategoryDTO
{
    [Required (ErrorMessage = "A descrição não pode estar vazia")]
    [MaxLength(400, ErrorMessage = "A descrição não pode ter mais de 400 caracteres")]
    public string Description { get; set; }
    
    [Required(ErrorMessage = "A finalidade não pode estar vazia")]
    public Finance Purpose  { get; set; }
}