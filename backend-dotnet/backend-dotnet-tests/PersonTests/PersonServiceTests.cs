using AutoMapper;
using backend_dotnet.DTO.Person;
using backend_dotnet.Entities;
using backend_dotnet.Interface;
using backend_dotnet.Service;
using Moq;

namespace backend_dotnet_tests.PersonTests;

public class PersonServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly PersonService _personService;

    public PersonServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _personService = new PersonService(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    #region Helper Methods

    /// <summary>
    /// Cria uma pessoa de teste com valores padrão
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
            Age = age
        };
    }

    /// <summary>
    /// Cria um DTO de criação de pessoa de teste
    /// </summary>
    private CreatePersonDTO CreateTestCreatePersonDTO(
        string name = "João Silva",
        int age = 30)
    {
        return new CreatePersonDTO
        {
            Name = name,
            Age = age
        };
    }

    /// <summary>
    /// Cria um DTO de atualização de pessoa de teste
    /// </summary>
    private UpdatePersonDTO CreateTestUpdatePersonDTO(
        string name = "João Silva Atualizado",
        int age = 31)
    {
        return new UpdatePersonDTO
        {
            Name = name,
            Age = age
        };
    }

    /// <summary>
    /// Configura mocks para sucesso na criação de pessoa
    /// </summary>
    private void SetupSuccessfulCreatePersonMocks(CreatePersonDTO dto, Person person)
    {
        _mockMapper
            .Setup(m => m.Map<Person>(dto))
            .Returns(person);

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.Add(person));

        _mockUnitOfWork
            .Setup(u => u.CommitAsync());
    }

    /// <summary>
    /// Configura mocks para sucesso na atualização de pessoa
    /// </summary>
    private void SetupSuccessfulUpdatePersonMocks(Person existingPerson, UpdatePersonDTO dto, Person updatedPerson)
    {
        _mockUnitOfWork
            .Setup(u => u.PersonRepository.findById(It.IsAny<Guid>()))
            .ReturnsAsync(existingPerson);

        _mockMapper
            .Setup(m => m.Map(dto, existingPerson))
            .Returns(updatedPerson);

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.Update(It.IsAny<Person>()));

        _mockUnitOfWork
            .Setup(u => u.CommitAsync());
    }

    /// <summary>
    /// Configura mocks para sucesso na deleção de pessoa
    /// </summary>
    private void SetupSuccessfulDeletePersonMocks(Person person)
    {
        _mockUnitOfWork
            .Setup(u => u.PersonRepository.findById(It.IsAny<Guid>()))
            .ReturnsAsync(person);

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.Delete(It.IsAny<Person>()));

        _mockUnitOfWork
            .Setup(u => u.CommitAsync());
    }

    #endregion

    #region ListAllPersons Tests

    [Fact]
    public async Task ListAllPersons_WithValidData_ShouldReturnSuccessWithPersons()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var persons = new List<Person>
        {
            CreateTestPerson(id: personId)
        };

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.ListAll())
            .ReturnsAsync((List<Person>)persons.AsEnumerable());

        // Act
        var result = await _personService.ListAllPersons();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Successes);
        Assert.Single(result.Successes);

        var successMessage = result.Successes.First();
        Assert.Equal("Sucesso ao buscar pessoas", successMessage.Message);

        var metadata = successMessage.Metadata["data"];
        Assert.NotNull(metadata);
        Assert.Equal(persons.AsEnumerable(), (IEnumerable<Person>)metadata);

        _mockUnitOfWork.Verify(u => u.PersonRepository.ListAll(), Times.Once);
    }

    [Fact]
    public async Task ListAllPersons_WithEmptyList_ShouldReturnSuccessWithEmptyData()
    {
        // Arrange
        var persons = new List<Person>();

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.ListAll())
            .ReturnsAsync((List<Person>)persons.AsEnumerable());

        // Act
        var result = await _personService.ListAllPersons();

        // Assert
        Assert.True(result.IsSuccess);
        var metadata = result.Successes.First().Metadata["data"];
        var returnedPersons = (IEnumerable<Person>)metadata;
        Assert.Empty(returnedPersons);
    }

    [Fact]
    public async Task ListAllPersons_WithMultiplePersons_ShouldReturnAllPersons()
    {
        // Arrange
        var persons = new List<Person>
        {
            CreateTestPerson("João Silva", 30),
            CreateTestPerson("Maria Santos", 28),
            CreateTestPerson("Pedro Costa", 35)
        };

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.ListAll())
            .ReturnsAsync((List<Person>)persons.AsEnumerable());

        // Act
        var result = await _personService.ListAllPersons();

        // Assert
        Assert.True(result.IsSuccess);
        var metadata = result.Successes.First().Metadata["data"];
        var returnedPersons = ((IEnumerable<Person>)metadata).ToList();
        Assert.Equal(3, returnedPersons.Count);
    }

    [Fact]
    public async Task ListAllPersons_WhenRepositoryThrowsException_ShouldReturnError()
    {
        // Arrange
        var exceptionMessage = "Erro ao acessar o banco de dados";
        _mockUnitOfWork
            .Setup(u => u.PersonRepository.ListAll())
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _personService.ListAllPersons();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        Assert.Single(result.Errors);
        Assert.Equal(exceptionMessage, result.Errors.First().Message);
    }

    #endregion

    #region CreatePerson Tests

    [Fact]
    public async Task CreatePerson_WithValidData_ShouldReturnSuccessWithPersonId()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var createPersonDto = CreateTestCreatePersonDTO();
        var person = CreateTestPerson(id: personId);

        SetupSuccessfulCreatePersonMocks(createPersonDto, person);

        // Act
        var result = await _personService.CreatePerson(createPersonDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Successes);
        Assert.Single(result.Successes);

        var successMessage = result.Successes.First();
        Assert.Equal("Cadastro realizado com sucesso", successMessage.Message);

        var returnedId = successMessage.Metadata["data"];
        Assert.Equal(personId, (Guid)returnedId);

        _mockMapper.Verify(m => m.Map<Person>(createPersonDto), Times.Once);
        _mockUnitOfWork.Verify(u => u.PersonRepository.Add(person), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task CreatePerson_WithDifferentAges_ShouldCreateSuccessfully()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var createPersonDto = CreateTestCreatePersonDTO("Maria", 25);
        var person = CreateTestPerson("Maria", 25, personId);

        SetupSuccessfulCreatePersonMocks(createPersonDto, person);

        // Act
        var result = await _personService.CreatePerson(createPersonDto);

        // Assert
        Assert.True(result.IsSuccess);
        var returnedId = result.Successes.First().Metadata["data"];
        Assert.Equal(personId, (Guid)returnedId);
    }

    [Fact]
    public async Task CreatePerson_WhenMapperThrowsException_ShouldReturnError()
    {
        // Arrange
        var createPersonDto = CreateTestCreatePersonDTO();
        var exceptionMessage = "Erro ao mapear DTO para entidade";

        _mockMapper
            .Setup(m => m.Map<Person>(createPersonDto))
            .Throws(new Exception(exceptionMessage));

        // Act
        var result = await _personService.CreatePerson(createPersonDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(exceptionMessage, result.Errors.First().Message);
        _mockUnitOfWork.Verify(u => u.PersonRepository.Add(It.IsAny<Person>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task CreatePerson_WhenRepositoryThrowsException_ShouldReturnError()
    {
        // Arrange
        var createPersonDto = CreateTestCreatePersonDTO();
        var person = CreateTestPerson();
        var exceptionMessage = "Erro ao adicionar pessoa";

        _mockMapper
            .Setup(m => m.Map<Person>(createPersonDto))
            .Returns(person);

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.Add(person))
            .Throws(new Exception(exceptionMessage));

        // Act
        var result = await _personService.CreatePerson(createPersonDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(exceptionMessage, result.Errors.First().Message);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task CreatePerson_WhenCommitThrowsException_ShouldReturnError()
    {
        // Arrange
        var createPersonDto = CreateTestCreatePersonDTO();
        var person = CreateTestPerson();
        var exceptionMessage = "Erro ao confirmar transação";

        _mockMapper
            .Setup(m => m.Map<Person>(createPersonDto))
            .Returns(person);

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.Add(person));

        _mockUnitOfWork
            .Setup(u => u.CommitAsync())
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _personService.CreatePerson(createPersonDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(exceptionMessage, result.Errors.First().Message);
    }

    [Fact]
    public async Task CreatePerson_ShouldCallRepositoryAddBeforeCommit()
    {
        // Arrange
        var createPersonDto = CreateTestCreatePersonDTO();
        var person = CreateTestPerson();
        var callOrder = new List<string>();

        _mockMapper
            .Setup(m => m.Map<Person>(createPersonDto))
            .Returns(person);

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.Add(It.IsAny<Person>()))
            .Callback(() => callOrder.Add("Add"));

        _mockUnitOfWork
            .Setup(u => u.CommitAsync())
            .Callback(() => callOrder.Add("Commit"));

        // Act
        var result = await _personService.CreatePerson(createPersonDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(new[] { "Add", "Commit" }, callOrder);
        
        _mockUnitOfWork.Verify(u => u.PersonRepository.Add(It.IsAny<Person>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Theory]
    [InlineData("João Silva", 25)]
    [InlineData("Maria Santos", 30)]
    [InlineData("Pedro Costa", 45)]
    [InlineData("Ana Oliveira", 28)]
    public async Task CreatePerson_WithDifferentNamesAndAges_ShouldCreateSuccessfully(string name, int age)
    {
        // Arrange
        var createPersonDto = CreateTestCreatePersonDTO(name, age);
        var person = CreateTestPerson(name, age);

        SetupSuccessfulCreatePersonMocks(createPersonDto, person);

        // Act
        var result = await _personService.CreatePerson(createPersonDto);

        // Assert
        Assert.True(result.IsSuccess);
        var returnedId = result.Successes.First().Metadata["data"];
        Assert.Equal(person.Id, (Guid)returnedId);
    }

    #endregion

    #region UpdatePerson Tests

    [Fact]
    public async Task UpdatePerson_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var existingPerson = CreateTestPerson(id: personId);
        var updatePersonDto = CreateTestUpdatePersonDTO();
        var updatedPerson = CreateTestPerson(updatePersonDto.Name, updatePersonDto.Age, personId);

        SetupSuccessfulUpdatePersonMocks(existingPerson, updatePersonDto, updatedPerson);

        // Act
        var result = await _personService.UpdatePerson(personId, updatePersonDto);

        // Assert
        Assert.True(result.IsSuccess);
        var successMessage = result.Successes.First();
        Assert.Equal("Dados atualizados com sucesso", successMessage.Message);
        
        var returnedId = successMessage.Metadata["data"];
        Assert.Equal(personId, (Guid)returnedId);

        _mockUnitOfWork.Verify(u => u.PersonRepository.findById(personId), Times.Once);
        _mockMapper.Verify(m => m.Map(updatePersonDto, existingPerson), Times.Once);
        _mockUnitOfWork.Verify(u => u.PersonRepository.Update(It.IsAny<Person>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdatePerson_WhenPersonNotFound_ShouldReturnError()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var updatePersonDto = CreateTestUpdatePersonDTO();

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.findById(personId))
            .ReturnsAsync((Person)null);

        // Act
        var result = await _personService.UpdatePerson(personId, updatePersonDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Pessoa não encontrada", result.Errors.First().Message);
        _mockUnitOfWork.Verify(u => u.PersonRepository.Update(It.IsAny<Person>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdatePerson_WhenMapperThrowsException_ShouldReturnError()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var existingPerson = CreateTestPerson(id: personId);
        var updatePersonDto = CreateTestUpdatePersonDTO();
        var exceptionMessage = "Erro ao mapear DTO";

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.findById(personId))
            .ReturnsAsync(existingPerson);

        _mockMapper
            .Setup(m => m.Map(updatePersonDto, existingPerson))
            .Throws(new Exception(exceptionMessage));

        // Act
        var result = await _personService.UpdatePerson(personId, updatePersonDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(exceptionMessage, result.Errors.First().Message);
        _mockUnitOfWork.Verify(u => u.PersonRepository.Update(It.IsAny<Person>()), Times.Never);
    }

    [Fact]
    public async Task UpdatePerson_WhenRepositoryThrowsException_ShouldReturnError()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var existingPerson = CreateTestPerson(id: personId);
        var updatePersonDto = CreateTestUpdatePersonDTO();
        var exceptionMessage = "Erro ao atualizar pessoa";

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.findById(personId))
            .ReturnsAsync(existingPerson);

        _mockMapper
            .Setup(m => m.Map(updatePersonDto, existingPerson))
            .Returns(existingPerson);

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.Update(It.IsAny<Person>()))
            .Throws(new Exception(exceptionMessage));

        // Act
        var result = await _personService.UpdatePerson(personId, updatePersonDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(exceptionMessage, result.Errors.First().Message);
    }

    [Fact]
    public async Task UpdatePerson_WhenCommitThrowsException_ShouldReturnError()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var existingPerson = CreateTestPerson(id: personId);
        var updatePersonDto = CreateTestUpdatePersonDTO();
        var exceptionMessage = "Erro ao confirmar transação";

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.findById(personId))
            .ReturnsAsync(existingPerson);

        _mockMapper
            .Setup(m => m.Map(updatePersonDto, existingPerson))
            .Returns(existingPerson);

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.Update(It.IsAny<Person>()));

        _mockUnitOfWork
            .Setup(u => u.CommitAsync())
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _personService.UpdatePerson(personId, updatePersonDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(exceptionMessage, result.Errors.First().Message);
    }

    [Fact]
    public async Task UpdatePerson_ShouldCallFindByIdBeforeUpdate()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var existingPerson = CreateTestPerson(id: personId);
        var updatePersonDto = CreateTestUpdatePersonDTO();
        var callOrder = new List<string>();

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.findById(It.IsAny<Guid>()))
            .Callback(() => callOrder.Add("FindById"))
            .ReturnsAsync(existingPerson);

        _mockMapper
            .Setup(m => m.Map(updatePersonDto, existingPerson))
            .Returns(existingPerson);

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.Update(It.IsAny<Person>()))
            .Callback(() => callOrder.Add("Update"));

        _mockUnitOfWork
            .Setup(u => u.CommitAsync())
            .Callback(() => callOrder.Add("Commit"));

        // Act
        var result = await _personService.UpdatePerson(personId, updatePersonDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(new[] { "FindById", "Update", "Commit" }, callOrder);
    }

    #endregion

    #region DeletePerson Tests

    [Fact]
    public async Task DeletePerson_WithValidId_ShouldReturnSuccess()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var person = CreateTestPerson(id: personId);

        SetupSuccessfulDeletePersonMocks(person);

        // Act
        var result = await _personService.DeletePerson(personId);

        // Assert
        Assert.True(result.IsSuccess);
        var successMessage = result.Successes.First();
        Assert.Equal("Pessoa deletada com sucesso", successMessage.Message);

        _mockUnitOfWork.Verify(u => u.PersonRepository.findById(personId), Times.Once);
        _mockUnitOfWork.Verify(u => u.PersonRepository.Delete(It.IsAny<Person>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task DeletePerson_WhenPersonNotFound_ShouldReturnError()
    {
        // Arrange
        var personId = Guid.NewGuid();

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.findById(personId))
            .ReturnsAsync((Person)null);

        // Act
        var result = await _personService.DeletePerson(personId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Pessoa não encontrada", result.Errors.First().Message);
        _mockUnitOfWork.Verify(u => u.PersonRepository.Delete(It.IsAny<Person>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task DeletePerson_WhenRepositoryThrowsException_ShouldReturnError()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var person = CreateTestPerson(id: personId);
        var exceptionMessage = "Erro ao deletar pessoa";

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.findById(personId))
            .ReturnsAsync(person);

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.Delete(It.IsAny<Person>()))
            .Throws(new Exception(exceptionMessage));

        // Act
        var result = await _personService.DeletePerson(personId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(exceptionMessage, result.Errors.First().Message);
    }

    [Fact]
    public async Task DeletePerson_WhenCommitThrowsException_ShouldReturnError()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var person = CreateTestPerson(id: personId);
        var exceptionMessage = "Erro ao confirmar transação";

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.findById(personId))
            .ReturnsAsync(person);

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.Delete(It.IsAny<Person>()));

        _mockUnitOfWork
            .Setup(u => u.CommitAsync())
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _personService.DeletePerson(personId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(exceptionMessage, result.Errors.First().Message);
    }

    [Fact]
    public async Task DeletePerson_ShouldCallFindByIdBeforeDelete()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var person = CreateTestPerson(id: personId);
        var callOrder = new List<string>();

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.findById(It.IsAny<Guid>()))
            .Callback(() => callOrder.Add("FindById"))
            .ReturnsAsync(person);

        _mockUnitOfWork
            .Setup(u => u.PersonRepository.Delete(It.IsAny<Person>()))
            .Callback(() => callOrder.Add("Delete"));

        _mockUnitOfWork
            .Setup(u => u.CommitAsync())
            .Callback(() => callOrder.Add("Commit"));

        // Act
        var result = await _personService.DeletePerson(personId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(new[] { "FindById", "Delete", "Commit" }, callOrder);
    }

    #endregion
}