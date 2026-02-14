using AutoMapper;
using backend_dotnet.DTO.Categories;
using backend_dotnet.Entities;
using backend_dotnet.Enum;
using backend_dotnet.Interface;
using backend_dotnet.Service;
using Moq;

namespace backend_dotnet_tests.CategoriesTests;

public class CategoriesServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CategoriesService _categoriesService;

    public CategoriesServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _categoriesService = new CategoriesService(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    #region ListCategories Tests

    [Fact]
    public async Task ListCategories_WithValidData_ShouldReturnSuccessWithCategories()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var categories = new List<Categories>
        {
            new Categories
            {
                Id = categoryId,
                Description = "Alimentação",
                Purpose = Finance.Expenses
            }
        };

        var categoriesDto = new List<ListCategoryDTO>
        {
            new ListCategoryDTO
            {
                Id = categoryId,
                Description = "Alimentação",
                Purpose = Finance.Expenses
            }
        };

        _mockUnitOfWork
            .Setup(u => u.CategoriesRepository.ListAll())
            .ReturnsAsync((List<Categories>)categories.AsEnumerable());

        _mockMapper
            .Setup(m => m.Map<List<ListCategoryDTO>>(It.IsAny<IEnumerable<Categories>>()))
            .Returns(categoriesDto);

        // Act
        var result = await _categoriesService.ListCategories();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Successes);
        Assert.Single(result.Successes);
        
        var successMessage = result.Successes.First();
        Assert.Equal("Categorias listadas com sucesso", successMessage.Message);
        
        var metadata = successMessage.Metadata["data"];
        Assert.NotNull(metadata);
        Assert.Equal(categoriesDto, (List<ListCategoryDTO>)metadata);

        _mockUnitOfWork.Verify(u => u.CategoriesRepository.ListAll(), Times.Once);
        _mockMapper.Verify(m => m.Map<List<ListCategoryDTO>>(It.IsAny<IEnumerable<Categories>>()), Times.Once);
    }

    [Fact]
    public async Task ListCategories_WithEmptyList_ShouldReturnSuccessWithEmptyData()
    {
        // Arrange
        var categories = new List<Categories>();
        var categoriesDto = new List<ListCategoryDTO>();

        _mockUnitOfWork
            .Setup(u => u.CategoriesRepository.ListAll())
            .ReturnsAsync((List<Categories>)categories.AsEnumerable());

        _mockMapper
            .Setup(m => m.Map<List<ListCategoryDTO>>(It.IsAny<IEnumerable<Categories>>()))
            .Returns(categoriesDto);

        // Act
        var result = await _categoriesService.ListCategories();

        // Assert
        Assert.True(result.IsSuccess);
        var metadata = result.Successes.First().Metadata["data"];
        var returnedCategories = (List<ListCategoryDTO>)metadata;
        Assert.Empty(returnedCategories);
    }

    [Fact]
    public async Task ListCategories_WithMultipleCategories_ShouldReturnAllCategories()
    {
        // Arrange
        var categories = new List<Categories>
        {
            new Categories { Id = Guid.NewGuid(), Description = "Alimentação", Purpose = Finance.Expenses },
            new Categories { Id = Guid.NewGuid(), Description = "Transporte", Purpose = Finance.Expenses },
            new Categories { Id = Guid.NewGuid(), Description = "Salário", Purpose = Finance.Income }
        };

        var categoriesDto = new List<ListCategoryDTO>
        {
            new ListCategoryDTO { Id = categories[0].Id, Description = "Alimentação", Purpose = Finance.Expenses },
            new ListCategoryDTO { Id = categories[1].Id, Description = "Transporte", Purpose = Finance.Expenses },
            new ListCategoryDTO { Id = categories[2].Id, Description = "Salário", Purpose = Finance.Income }
        };

        _mockUnitOfWork
            .Setup(u => u.CategoriesRepository.ListAll())
            .ReturnsAsync((List<Categories>)categories.AsEnumerable());

        _mockMapper
            .Setup(m => m.Map<List<ListCategoryDTO>>(It.IsAny<IEnumerable<Categories>>()))
            .Returns(categoriesDto);

        // Act
        var result = await _categoriesService.ListCategories();

        // Assert
        Assert.True(result.IsSuccess);
        var metadata = result.Successes.First().Metadata["data"];
        var returnedCategories = (List<ListCategoryDTO>)metadata;
        Assert.Equal(3, returnedCategories.Count);
    }

    [Fact]
    public async Task ListCategories_WhenRepositoryThrowsException_ShouldReturnError()
    {
        // Arrange
        var exceptionMessage = "Erro ao acessar o banco de dados";
        _mockUnitOfWork
            .Setup(u => u.CategoriesRepository.ListAll())
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _categoriesService.ListCategories();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        Assert.Single(result.Errors);
        Assert.Equal(exceptionMessage, result.Errors.First().Message);
    }

    [Fact]
    public async Task ListCategories_WhenMapperThrowsException_ShouldReturnError()
    {
        // Arrange
        var categories = new List<Categories> 
        { 
            new Categories { Id = Guid.NewGuid(), Description = "Test", Purpose = Finance.Expenses } 
        };
        var exceptionMessage = "Erro ao mapear categorias";

        _mockUnitOfWork
            .Setup(u => u.CategoriesRepository.ListAll())
            .ReturnsAsync((List<Categories>)categories.AsEnumerable());

        _mockMapper
            .Setup(m => m.Map<List<ListCategoryDTO>>(It.IsAny<IEnumerable<Categories>>()))
            .Throws(new Exception(exceptionMessage));

        // Act
        var result = await _categoriesService.ListCategories();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(exceptionMessage, result.Errors.First().Message);
    }

    #endregion

    #region CreateCategory Tests

    [Fact]
    public async Task CreateCategory_WithValidData_ShouldReturnSuccessWithCategoryId()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var createCategoryDto = new CreateCategoryDTO
        {
            Description = "Alimentação",
            Purpose = Finance.Expenses
        };

        var category = new Categories
        {
            Id = categoryId,
            Description = "Alimentação",
            Purpose = Finance.Expenses
        };

        _mockMapper
            .Setup(m => m.Map<Categories>(createCategoryDto))
            .Returns(category);

        _mockUnitOfWork
            .Setup(u => u.CategoriesRepository.Add(category))
            .Verifiable();

        _mockUnitOfWork
            .Setup(u => u.CommitAsync())
            .Verifiable();

        // Act
        var result = await _categoriesService.CreateCategory(createCategoryDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Successes);
        Assert.Single(result.Successes);

        var successMessage = result.Successes.First();
        Assert.Equal("Categoria criada com sucesso", successMessage.Message);

        var returnedId = successMessage.Metadata["data"];
        Assert.Equal(categoryId, (Guid)returnedId);

        _mockMapper.Verify(m => m.Map<Categories>(createCategoryDto), Times.Once);
        _mockUnitOfWork.Verify(u => u.CategoriesRepository.Add(category), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateCategory_WithIncomeCategory_ShouldReturnSuccessWithCategoryId()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var createCategoryDto = new CreateCategoryDTO
        {
            Description = "Salário",
            Purpose = Finance.Income
        };

        var category = new Categories
        {
            Id = categoryId,
            Description = "Salário",
            Purpose = Finance.Income
        };

        _mockMapper
            .Setup(m => m.Map<Categories>(createCategoryDto))
            .Returns(category);

        _mockUnitOfWork
            .Setup(u => u.CategoriesRepository.Add(category));

        _mockUnitOfWork
            .Setup(u => u.CommitAsync());

        // Act
        var result = await _categoriesService.CreateCategory(createCategoryDto);

        // Assert
        Assert.True(result.IsSuccess);
        var returnedId = result.Successes.First().Metadata["data"];
        Assert.Equal(categoryId, (Guid)returnedId);
    }

    [Fact]
    public async Task CreateCategory_WhenMapperThrowsException_ShouldReturnError()
    {
        // Arrange
        var createCategoryDto = new CreateCategoryDTO
        {
            Description = "Alimentação",
            Purpose = Finance.Expenses
        };

        var exceptionMessage = "Erro ao mapear DTO para entidade";
        _mockMapper
            .Setup(m => m.Map<Categories>(createCategoryDto))
            .Throws(new Exception(exceptionMessage));

        // Act
        var result = await _categoriesService.CreateCategory(createCategoryDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(exceptionMessage, result.Errors.First().Message);
        _mockUnitOfWork.Verify(u => u.CategoriesRepository.Add(It.IsAny<Categories>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateCategory_WhenRepositoryThrowsException_ShouldReturnError()
    {
        // Arrange
        var createCategoryDto = new CreateCategoryDTO
        {
            Description = "Alimentação",
            Purpose = Finance.Expenses
        };

        var category = new Categories
        {
            Id = Guid.NewGuid(),
            Description = "Alimentação",
            Purpose = Finance.Expenses
        };

        var exceptionMessage = "Erro ao adicionar categoria";
        _mockMapper
            .Setup(m => m.Map<Categories>(createCategoryDto))
            .Returns(category);

        _mockUnitOfWork
            .Setup(u => u.CategoriesRepository.Add(category))
            .Throws(new Exception(exceptionMessage));

        // Act
        var result = await _categoriesService.CreateCategory(createCategoryDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(exceptionMessage, result.Errors.First().Message);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateCategory_WhenCommitThrowsException_ShouldReturnError()
    {
        // Arrange
        var createCategoryDto = new CreateCategoryDTO
        {
            Description = "Alimentação",
            Purpose = Finance.Expenses
        };

        var category = new Categories
        {
            Id = Guid.NewGuid(),
            Description = "Alimentação",
            Purpose = Finance.Expenses
        };

        var exceptionMessage = "Erro ao confirmar transação";
        _mockMapper
            .Setup(m => m.Map<Categories>(createCategoryDto))
            .Returns(category);

        _mockUnitOfWork
            .Setup(u => u.CategoriesRepository.Add(category));

        _mockUnitOfWork
            .Setup(u => u.CommitAsync())
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _categoriesService.CreateCategory(createCategoryDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(exceptionMessage, result.Errors.First().Message);
    }

    [Theory]
    [InlineData("Alimentação", Finance.Expenses)]
    [InlineData("Transporte", Finance.Expenses)]
    [InlineData("Salário", Finance.Income)]
    [InlineData("Bônus", Finance.Income)]
    public async Task CreateCategory_WithDifferentCategoriesAndPurposes_ShouldCreateSuccessfully(
        string description, Finance purpose)
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var createCategoryDto = new CreateCategoryDTO
        {
            Description = description,
            Purpose = purpose
        };

        var category = new Categories
        {
            Id = categoryId,
            Description = description,
            Purpose = purpose
        };

        _mockMapper
            .Setup(m => m.Map<Categories>(createCategoryDto))
            .Returns(category);

        _mockUnitOfWork
            .Setup(u => u.CategoriesRepository.Add(category));

        _mockUnitOfWork
            .Setup(u => u.CommitAsync());

        // Act
        var result = await _categoriesService.CreateCategory(createCategoryDto);

        // Assert
        Assert.True(result.IsSuccess);
        var returnedId = result.Successes.First().Metadata["data"];
        Assert.Equal(categoryId, (Guid)returnedId);
    }

    [Fact]
    public async Task CreateCategory_VerifyMappingIsCalledWithCorrectDto()
    {
        // Arrange
        var createCategoryDto = new CreateCategoryDTO
        {
            Description = "Test Category",
            Purpose = Finance.Expenses
        };

        var category = new Categories
        {
            Id = Guid.NewGuid(),
            Description = "Test Category",
            Purpose = Finance.Expenses
        };

        _mockMapper
            .Setup(m => m.Map<Categories>(It.IsAny<CreateCategoryDTO>()))
            .Returns(category);

        _mockUnitOfWork
            .Setup(u => u.CommitAsync());

        // Act
        await _categoriesService.CreateCategory(createCategoryDto);

        // Assert - Verify the mapper was called with the exact DTO
        _mockMapper.Verify(
            m => m.Map<Categories>(It.Is<CreateCategoryDTO>(dto =>
                dto.Description == createCategoryDto.Description &&
                dto.Purpose == createCategoryDto.Purpose)),
            Times.Once);
    }

    #endregion
}