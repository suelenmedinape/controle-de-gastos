using AutoMapper;
using backend_dotnet.DTO.Categories;
using backend_dotnet.Entities;
using backend_dotnet.Interface;
using FluentResults;

namespace backend_dotnet.Service;

public class CategoriesService
{
    private readonly IUnitOfWork unit;
    private readonly IMapper mapper;

    public CategoriesService(IUnitOfWork unit, IMapper mapper)
    {
        this.unit = unit;
        this.mapper = mapper;
    }
    
    public async Task<Result> ListCategories()
    {
        try
        {
            // FALTA TERMINAR
            var categories = await unit.CategoriesRepository.ListAll();
            var categoriesDto = mapper.Map<List<ListCategoryDTO>>(categories);
            
            return new Result()
                .WithSuccess(new Success("Categorias listadas com sucesso")
                    .WithMetadata("data", categoriesDto));
        }
        catch (Exception e)
        {
            return new Result().WithError(new Error(e.Message));
        }
    }

    public async Task<Result> CreateCategory(CreateCategoryDTO dto)
    {
        try
        {
            var category = mapper.Map<Categories>(dto);
            
            unit.CategoriesRepository.Add(category);
            await unit.CommitAsync();
            
            return new Result()
                .WithSuccess(new Success("Categoria criada com sucesso")
                    .WithMetadata("data", category.Id));
        }
        catch (Exception e)
        {
            return new Result().WithError(new Error(e.Message));
        }
    }
}