using backend_dotnet.DTO.Categories;
using backend_dotnet.Service;
using Microsoft.AspNetCore.Mvc;

namespace backend_dotnet.Controller;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly UnitOfService service;
    
    public CategoriesController(UnitOfService service)
    {
        this.service = service;
    }
    
    /// <summary>
    /// Listar todas as categorias
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> ListCategories()
    {
        var result = await service.CategoriesService.ListCategories();
        return result.IsSuccess
            ? Ok(result.Successes.FirstOrDefault())
            : StatusCode(500, result.Errors.FirstOrDefault());
    }

    /// <summary>
    /// Criar uma nova categoria
    /// </summary>
    /// <param name="dto">Dados da categoria</param>
    /// <returns>Retorna o id da categoria criada</returns>
    /// <response code="201">Categoria criada com sucesso</response>
    /// <response code="400">Um ou mais erros de validação ocorreram</response>
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDTO dto)
    {
        var result = await service.CategoriesService.CreateCategory(dto);
        return result.IsSuccess
            ? Created("", result.Successes.FirstOrDefault())
            : StatusCode(400, result.Errors.FirstOrDefault());
    }
}