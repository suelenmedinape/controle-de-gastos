using AutoMapper;
using backend_dotnet.DTO.Person.Transaction;
using backend_dotnet.Entities;
using backend_dotnet.Enum;
using backend_dotnet.Interface;
using backend_dotnet.Service;
using Moq;

namespace backend_dotnet_tests.TransactionTests;

public class TransactionServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly TransactionService _transactionService;

    public TransactionServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _transactionService = new TransactionService(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    #region Helper Methods

    /// <summary>
    /// Cria uma transação de teste com valores padrão
    /// </summary>
    private Transaction CreateTestTransaction(
        string description = "Compra de alimentos",
        decimal value = 100.50m,
        Finance type = Finance.Expenses,
        Guid? id = null,
        Guid? personId = null,
        Guid? categoryId = null)
    {
        return new Transaction
        {
            Id = id ?? Guid.NewGuid(),
            Description = description,
            Value = value,
            Type = type,
            PersonId = personId ?? Guid.NewGuid(),
            CategoryId = categoryId ?? Guid.NewGuid()
        };
    }

    /// <summary>
    /// Cria um DTO de criação de transação de teste
    /// </summary>
    private CreateTransactionDTO CreateTestCreateTransactionDTO(
        string description = "Compra de alimentos",
        decimal value = 100.50m,
        Finance type = Finance.Expenses,
        Guid? personId = null,
        Guid? categoryId = null)
    {
        return new CreateTransactionDTO
        {
            Description = description,
            Value = value,
            Type = type,
            PersonId = personId ?? Guid.NewGuid(),
            CategoryId = categoryId ?? Guid.NewGuid()
        };
    }

    /// <summary>
    /// Cria uma pessoa de teste
    /// </summary>
    private Person CreateTestPerson(
        string name = "João Silva",
        int age = 30,
        Guid? id = null)
    {
        return new Person
        {
            Id = id ?? Guid.NewGuid(),
            Name = name,
            Age = age,
            Transactions = new List<Transaction>()
        };
    }

    /// <summary>
    /// Cria uma categoria de teste
    /// </summary>
    private Categories CreateTestCategory(
        string description = "Alimentação",
        Finance purpose = Finance.Expenses,
        Guid? id = null)
    {
        return new Categories
        {
            Id = id ?? Guid.NewGuid(),
            Description = description,
            Purpose = purpose,
            Transactions = new List<Transaction>()
        };
    }

    /// <summary>
    /// Configura mocks para sucesso na criação de transação
    /// </summary>
    private void SetupSuccessfulCreateTransactionMocks(
        CreateTransactionDTO dto,
        Person person,
        Categories category,
        Transaction transaction)
    {
        _mockUnitOfWork
            .Setup(u => u.PersonRepository.findById(dto.PersonId))
            .ReturnsAsync(person);

        _mockUnitOfWork
            .Setup(u => u.CategoriesRepository.findById(dto.CategoryId))
            .ReturnsAsync(category);

        _mockMapper
            .Setup(m => m.Map<Transaction>(dto))
            .Returns(transaction);

        _mockUnitOfWork
            .Setup(u => u.TransactionRepository.Add(transaction));

        _mockUnitOfWork
            .Setup(u => u.CommitAsync());
    }

    #endregion

    #region ListTransactions Tests

    [Fact]
    public async Task ListTransactions_WithValidData_ShouldReturnSuccessWithTransactions()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var transactions = new List<Transaction>
        {
            CreateTestTransaction(
                personId: personId,
                categoryId: categoryId)
        };

        var transactionsDto = new List<ListAllTransactionsDTO>
        {
            new ListAllTransactionsDTO
            {
                Id = transactions[0].Id,
                Description = "Compra de alimentos",
                Value = 100.50m,
                Type = Finance.Expenses
            }
        };

        _mockUnitOfWork
            .Setup(u => u.TransactionRepository.ListAll())
            .ReturnsAsync((List<Transaction>)transactions.AsEnumerable());

        _mockMapper
            .Setup(m => m.Map<List<ListAllTransactionsDTO>>(It.IsAny<IEnumerable<Transaction>>()))
            .Returns(transactionsDto);

        // Act
        var result = await _transactionService.ListTransactions();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Successes);
        Assert.Single(result.Successes);

        var successMessage = result.Successes.First();
        Assert.Equal("Categorias listadas com sucesso", successMessage.Message);

        var metadata = successMessage.Metadata["data"];
        Assert.NotNull(metadata);
        Assert.Equal(transactionsDto, (List<ListAllTransactionsDTO>)metadata);

        _mockUnitOfWork.Verify(u => u.TransactionRepository.ListAll(), Times.Once);
        _mockMapper.Verify(m => m.Map<List<ListAllTransactionsDTO>>(It.IsAny<IEnumerable<Transaction>>()), Times.Once);
    }

    [Fact]
    public async Task ListTransactions_WithEmptyList_ShouldReturnSuccessWithEmptyData()
    {
        // Arrange
        var transactions = new List<Transaction>();
        var transactionsDto = new List<ListAllTransactionsDTO>();

        _mockUnitOfWork
            .Setup(u => u.TransactionRepository.ListAll())
            .ReturnsAsync((List<Transaction>)transactions.AsEnumerable());

        _mockMapper
            .Setup(m => m.Map<List<ListAllTransactionsDTO>>(It.IsAny<IEnumerable<Transaction>>()))
            .Returns(transactionsDto);

        // Act
        var result = await _transactionService.ListTransactions();

        // Assert
        Assert.True(result.IsSuccess);
        var metadata = result.Successes.First().Metadata["data"];
        var returnedTransactions = (List<ListAllTransactionsDTO>)metadata;
        Assert.Empty(returnedTransactions);
    }

    [Fact]
    public async Task ListTransactions_WithMultipleTransactions_ShouldReturnAllTransactions()
    {
        // Arrange
        var transactions = new List<Transaction>
        {
            CreateTestTransaction("Compra 1", 100m),
            CreateTestTransaction("Compra 2", 200m),
            CreateTestTransaction("Salário", 3000m, Finance.Income)
        };

        var transactionsDto = new List<ListAllTransactionsDTO>
        {
            new ListAllTransactionsDTO { Id = transactions[0].Id, Description = "Compra 1", Value = 100m, Type = Finance.Expenses },
            new ListAllTransactionsDTO { Id = transactions[1].Id, Description = "Compra 2", Value = 200m, Type = Finance.Expenses },
            new ListAllTransactionsDTO { Id = transactions[2].Id, Description = "Salário", Value = 3000m, Type = Finance.Income }
        };

        _mockUnitOfWork
            .Setup(u => u.TransactionRepository.ListAll())
            .ReturnsAsync((List<Transaction>)transactions.AsEnumerable());

        _mockMapper
            .Setup(m => m.Map<List<ListAllTransactionsDTO>>(It.IsAny<IEnumerable<Transaction>>()))
            .Returns(transactionsDto);

        // Act
        var result = await _transactionService.ListTransactions();

        // Assert
        Assert.True(result.IsSuccess);
        var metadata = result.Successes.First().Metadata["data"];
        var returnedTransactions = (List<ListAllTransactionsDTO>)metadata;
        Assert.Equal(3, returnedTransactions.Count);
    }

    [Fact]
    public async Task ListTransactions_WhenRepositoryThrowsException_ShouldReturnError()
    {
        // Arrange
        var exceptionMessage = "Erro ao acessar o banco de dados";
        _mockUnitOfWork
            .Setup(u => u.TransactionRepository.ListAll())
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _transactionService.ListTransactions();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        Assert.Single(result.Errors);
        Assert.Equal(exceptionMessage, result.Errors.First().Message);
    }

    [Fact]
    public async Task ListTransactions_WhenMapperThrowsException_ShouldReturnError()
    {
        // Arrange
        var transactions = new List<Transaction> { CreateTestTransaction() };
        var exceptionMessage = "Erro ao mapear transações";

        _mockUnitOfWork
            .Setup(u => u.TransactionRepository.ListAll())
            .ReturnsAsync((List<Transaction>)transactions.AsEnumerable());

        _mockMapper
            .Setup(m => m.Map<List<ListAllTransactionsDTO>>(It.IsAny<IEnumerable<Transaction>>()))
            .Throws(new Exception(exceptionMessage));

        // Act
        var result = await _transactionService.ListTransactions();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(exceptionMessage, result.Errors.First().Message);
    }

    #endregion

    #region CreateTransaction Tests

    [Fact]
    public async Task CreateTransaction_WithValidData_ShouldReturnSuccessWithTransactionId()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var person = CreateTestPerson(id: personId);
        var category = CreateTestCategory(purpose: Finance.Expenses, id: categoryId);
        var createTransactionDto = CreateTestCreateTransactionDTO(
            personId: personId,
            categoryId: categoryId,
            type: Finance.Expenses);
        var transaction = CreateTestTransaction(
            id: transactionId,
            personId: personId,
            categoryId: categoryId,
            type: Finance.Expenses);

        SetupSuccessfulCreateTransactionMocks(createTransactionDto, person, category, transaction);

        // Act
        var result = await _transactionService.CreateTransaction(createTransactionDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Successes);
        Assert.Single(result.Successes);

        var successMessage = result.Successes.First();
        Assert.Equal("Transação criada com sucesso", successMessage.Message);

        var returnedId = successMessage.Metadata["data"];
        Assert.Equal(transactionId, (Guid)returnedId);

        _mockUnitOfWork.Verify(u => u.PersonRepository.findById(personId), Times.Once);
        _mockUnitOfWork.Verify(u => u.CategoriesRepository.findById(categoryId), Times.Once);
        _mockMapper.Verify(m => m.Map<Transaction>(createTransactionDto), Times.Once);
        _mockUnitOfWork.Verify(u => u.TransactionRepository.Add(It.IsAny<Transaction>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateTransaction_WithIncomeType_ShouldReturnSuccess()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var person = CreateTestPerson(id: personId, age: 35);
        var category = CreateTestCategory(purpose: Finance.Income, id: categoryId);
        var createTransactionDto = CreateTestCreateTransactionDTO(
            personId: personId,
            categoryId: categoryId,
            type: Finance.Income,
            value: 3000m);
        var transaction = CreateTestTransaction(
            id: transactionId,
            personId: personId,
            categoryId: categoryId,
            type: Finance.Income,
            value: 3000m);

        SetupSuccessfulCreateTransactionMocks(createTransactionDto, person, category, transaction);

        // Act
        var result = await _transactionService.CreateTransaction(createTransactionDto);

        // Assert
        Assert.True(result.IsSuccess);
        var returnedId = result.Successes.First().Metadata["data"];
        Assert.Equal(transactionId, (Guid)returnedId);
    }

    [Fact]
    public async Task CreateTransaction_WhenPersonNotFound_ShouldReturnError()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var createTransactionDto = CreateTestCreateTransactionDTO(personId: personId);

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.findById(personId))
            .ReturnsAsync((Person)null);

        // Act
        var result = await _transactionService.CreateTransaction(createTransactionDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Pessoa não encontrada", result.Errors.First().Message);
        _mockUnitOfWork.Verify(u => u.CategoriesRepository.findById(It.IsAny<Guid>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.TransactionRepository.Add(It.IsAny<Transaction>()), Times.Never);
    }

    [Fact]
    public async Task CreateTransaction_WhenMinorTryingIncomeTransaction_ShouldReturnError()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var minorPerson = CreateTestPerson(age: 16, id: personId);
        var createTransactionDto = CreateTestCreateTransactionDTO(
            personId: personId,
            categoryId: categoryId,
            type: Finance.Income);

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.findById(personId))
            .ReturnsAsync(minorPerson);

        // Act
        var result = await _transactionService.CreateTransaction(createTransactionDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Pessosas menores de 18 podem ter apenas despesas", result.Errors.First().Message);
        _mockUnitOfWork.Verify(u => u.CategoriesRepository.findById(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task CreateTransaction_WhenCategoryNotFound_ShouldReturnError()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var person = CreateTestPerson(id: personId);
        var createTransactionDto = CreateTestCreateTransactionDTO(
            personId: personId,
            categoryId: categoryId);

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.findById(personId))
            .ReturnsAsync(person);

        _mockUnitOfWork
            .Setup(u => u.CategoriesRepository.findById(categoryId))
            .ReturnsAsync((Categories)null);

        // Act
        var result = await _transactionService.CreateTransaction(createTransactionDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Categoria não encontrada", result.Errors.First().Message);
        _mockUnitOfWork.Verify(u => u.TransactionRepository.Add(It.IsAny<Transaction>()), Times.Never);
    }

    [Fact]
    public async Task CreateTransaction_WhenExpenseTransactionWithIncomeCategory_ShouldReturnError()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var person = CreateTestPerson(id: personId);
        var incomeCategoryWrongType = CreateTestCategory(purpose: Finance.Income, id: categoryId);
        var createTransactionDto = CreateTestCreateTransactionDTO(
            personId: personId,
            categoryId: categoryId,
            type: Finance.Expenses);

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.findById(personId))
            .ReturnsAsync(person);

        _mockUnitOfWork
            .Setup(u => u.CategoriesRepository.findById(categoryId))
            .ReturnsAsync(incomeCategoryWrongType);

        // Act
        var result = await _transactionService.CreateTransaction(createTransactionDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Não é possível usar uma categoria de Receita para uma transação de Despesa.", result.Errors.First().Message);
        _mockUnitOfWork.Verify(u => u.TransactionRepository.Add(It.IsAny<Transaction>()), Times.Never);
    }

    [Fact]
    public async Task CreateTransaction_WhenIncomeTransactionWithExpenseCategory_ShouldReturnError()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var person = CreateTestPerson(id: personId, age: 25);
        var expenseCategoryWrongType = CreateTestCategory(purpose: Finance.Expenses, id: categoryId);
        var createTransactionDto = CreateTestCreateTransactionDTO(
            personId: personId,
            categoryId: categoryId,
            type: Finance.Income);

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.findById(personId))
            .ReturnsAsync(person);

        _mockUnitOfWork
            .Setup(u => u.CategoriesRepository.findById(categoryId))
            .ReturnsAsync(expenseCategoryWrongType);

        // Act
        var result = await _transactionService.CreateTransaction(createTransactionDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Não é possível usar uma categoria de Despesa para uma transação de Receita.", result.Errors.First().Message);
        _mockUnitOfWork.Verify(u => u.TransactionRepository.Add(It.IsAny<Transaction>()), Times.Never);
    }

    [Fact]
    public async Task CreateTransaction_WhenMapperThrowsException_ShouldReturnError()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var person = CreateTestPerson(id: personId);
        var category = CreateTestCategory(id: categoryId);
        var createTransactionDto = CreateTestCreateTransactionDTO(
            personId: personId,
            categoryId: categoryId);
        var exceptionMessage = "Erro ao mapear DTO para entidade";

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.findById(personId))
            .ReturnsAsync(person);

        _mockUnitOfWork
            .Setup(u => u.CategoriesRepository.findById(categoryId))
            .ReturnsAsync(category);

        _mockMapper
            .Setup(m => m.Map<Transaction>(createTransactionDto))
            .Throws(new Exception(exceptionMessage));

        // Act
        var result = await _transactionService.CreateTransaction(createTransactionDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(exceptionMessage, result.Errors.First().Message);
        _mockUnitOfWork.Verify(u => u.TransactionRepository.Add(It.IsAny<Transaction>()), Times.Never);
    }

    [Fact]
    public async Task CreateTransaction_WhenRepositoryThrowsException_ShouldReturnError()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var person = CreateTestPerson(id: personId);
        var category = CreateTestCategory(id: categoryId);
        var createTransactionDto = CreateTestCreateTransactionDTO(
            personId: personId,
            categoryId: categoryId);
        var transaction = CreateTestTransaction();
        var exceptionMessage = "Erro ao adicionar transação";

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.findById(personId))
            .ReturnsAsync(person);

        _mockUnitOfWork
            .Setup(u => u.CategoriesRepository.findById(categoryId))
            .ReturnsAsync(category);

        _mockMapper
            .Setup(m => m.Map<Transaction>(createTransactionDto))
            .Returns(transaction);

        _mockUnitOfWork
            .Setup(u => u.TransactionRepository.Add(It.IsAny<Transaction>()))
            .Throws(new Exception(exceptionMessage));

        // Act
        var result = await _transactionService.CreateTransaction(createTransactionDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(exceptionMessage, result.Errors.First().Message);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateTransaction_WhenCommitThrowsException_ShouldReturnError()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var person = CreateTestPerson(id: personId);
        var category = CreateTestCategory(id: categoryId);
        var createTransactionDto = CreateTestCreateTransactionDTO(
            personId: personId,
            categoryId: categoryId);
        var transaction = CreateTestTransaction();
        var exceptionMessage = "Erro ao confirmar transação";

        SetupSuccessfulCreateTransactionMocks(createTransactionDto, person, category, transaction);

        _mockUnitOfWork
            .Setup(u => u.CommitAsync())
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _transactionService.CreateTransaction(createTransactionDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(exceptionMessage, result.Errors.First().Message);
    }

    [Fact]
    public async Task CreateTransaction_ShouldValidatePersonBeforeCategory()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var callOrder = new List<string>();

        var createTransactionDto = CreateTestCreateTransactionDTO(
            personId: personId,
            categoryId: categoryId);

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.findById(personId))
            .Callback(() => callOrder.Add("FindPerson"))
            .ReturnsAsync((Person)null);

        _mockUnitOfWork
            .Setup(u => u.CategoriesRepository.findById(categoryId))
            .Callback(() => callOrder.Add("FindCategory"))
            .ReturnsAsync((Categories)null);

        // Act
        var result = await _transactionService.CreateTransaction(createTransactionDto);

        // Assert
        Assert.Equal(new[] { "FindPerson" }, callOrder);
        _mockUnitOfWork.Verify(u => u.CategoriesRepository.findById(It.IsAny<Guid>()), Times.Never);
    }

    [Theory]
    [InlineData("Compra no mercado", 150.50, Finance.Expenses)]
    [InlineData("Salário mensal", 3000, Finance.Income)]
    [InlineData("Conta de energia", 250, Finance.Expenses)]
    [InlineData("Freelance", 500, Finance.Income)]
    public async Task CreateTransaction_WithDifferentValuesAndTypes_ShouldCreateSuccessfully(
        string description, decimal value, Finance type)
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var person = CreateTestPerson(id: personId, age: 30);
        var category = CreateTestCategory(purpose: type, id: categoryId);
        var createTransactionDto = CreateTestCreateTransactionDTO(
            description: description,
            value: value,
            type: type,
            personId: personId,
            categoryId: categoryId);
        var transaction = CreateTestTransaction(
            id: transactionId,
            description: description,
            value: value,
            type: type,
            personId: personId,
            categoryId: categoryId);

        SetupSuccessfulCreateTransactionMocks(createTransactionDto, person, category, transaction);

        // Act
        var result = await _transactionService.CreateTransaction(createTransactionDto);

        // Assert
        Assert.True(result.IsSuccess);
        var returnedId = result.Successes.First().Metadata["data"];
        Assert.Equal(transactionId, (Guid)returnedId);
    }

    #endregion
}