using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using PwdGenBLL.Converters;
using PwdGenBLL.Services;

using PwdGenDLL;
using PwdGenDLL.Models;
using PwdGenDLL.Repositories.Implementations;

namespace PwdGenTests.BLL.Services
{
    public class EncryptionServiceTests
    {
        private AppDbContext _dbContext;
        private EncryptionService _service;
        private SqliteConnection _connection;

        [SetUp]
        public void Setup()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            _dbContext = new AppDbContext(options);
            _dbContext.Database.EnsureCreated();

            EncryptionRepository repository = new(_dbContext);
            EncryptionConverter converter = new();
            _service = new EncryptionService(repository, converter);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
            _connection.Close();
        }

        [Test]
        public void Add_ValidEncryptionDTO()
        {
            // Arrange
            var encryption = new EncryptionDTO("TestEncryption");

            // Act
            encryption = _service.Add(encryption);

            // Assert
            var result = _service.Get(encryption!.Id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(encryption));
        }

        [Test]
        public void Add_InvalidEncryptionDTO_ThrowsException()
        {
            // Arrange
            var longEncryptionDTO = new EncryptionDTO("ThisIsAEncryptionDTOWithVeryLongTextNameValueThatExceedsTheMaximumLengthAllowedByTheDatabaseModelHaveAGoodDayNowSir");
            var existingEncryptionDTO = new EncryptionDTO("123");
            existingEncryptionDTO = _service.Add(existingEncryptionDTO);

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _service.Add(longEncryptionDTO), Throws.Exception);
                Assert.That(() => _service.Add(new EncryptionDTO(existingEncryptionDTO!.Name)), Throws.Exception);
            });
        }

        [Test]
        public void Get_ReturnsAllEncryptionDTOs()
        {
            // Arrange
            var encryption1 = new EncryptionDTO("EncryptionDTO1");
            var encryption2 = new EncryptionDTO("EncryptionDTO2");
            encryption1 = _service.Add(encryption1);
            encryption2 = _service.Add(encryption2);

            // Act
            var encryptions = _service.Get().ToList();

            // Assert
            Assert.That(encryptions, Has.Count.EqualTo(2));
            Assert.That(encryptions, Contains.Item(encryption1));
            Assert.That(encryptions, Contains.Item(encryption2));
        }

        [Test]
        public void Get_ValidEncryptionDTO_ReturnsEncryptionDTO()
        {
            // Arrange
            var encryption1 = new EncryptionDTO("TestEncryptionDTO1");
            var encryption2 = new EncryptionDTO("TestEncryptionDTO2");
            encryption1 = _service.Add(encryption1);
            encryption2 = _service.Add(encryption2);

            // Act
            var resultEncryptionDTO1 = _service.Get(encryption1?.Id ?? 0);
            var resultEncryptionDTO2 = _service.Get(e => e.Id == encryption2!.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultEncryptionDTO1, Is.Not.Null);
                Assert.That(resultEncryptionDTO2, Is.Not.Empty);
                Assert.That(resultEncryptionDTO1, Is.EqualTo(encryption1));
                Assert.That(resultEncryptionDTO2.First(), Is.EqualTo(encryption2));
            });
        }

        [Test]
        public void Get_InvalidEncryptionDTO_ReturnsNullOrEmpty()
        {
            // Arrange
            var encryption = new EncryptionDTO("TestEncryptionDTO");
            encryption = _service.Add(encryption);

            // Act
            var result1 = _service.Get(encryption!.Id + 1);
            var result2 = _service.Get(e => e.Id == (encryption.Id + 1));

            // Assert
            Assert.That(result1, Is.Null);
            Assert.That(result2, Is.Empty);
        }

        [Test]
        public void Update_ValidEncryptionDTO()
        {
            // Arrange
            var encryption = new EncryptionDTO("TestEncryptionDTO");
            encryption = _service.Add(encryption);

            // Act
            encryption!.Name = "UpdatedName";
            _service.Update(encryption);

            // Assert
            Assert.That(encryption, Is.Not.Null);
            Assert.That(encryption.Name, Is.EqualTo("UpdatedName"));
        }

        [Test]
        public void Update_InvalidEncryptionDTO_ThrowsException()
        {
            // Arrange
            var notExistingEncryptionDTO = new EncryptionDTO("notExistingEncryptionDTO");
            var shortEncryptionDTO = new EncryptionDTO("shortEncryptionDTO");
            var existingEncryptionDTOOrigin = new EncryptionDTO("existingEncryptionDTOOrigin");
            var existingEncryptionDTOCopy = new EncryptionDTO("existingEncryptionDTOCopy");

            _service.Add(shortEncryptionDTO);
            _service.Add(existingEncryptionDTOOrigin);
            _service.Add(existingEncryptionDTOCopy);

            shortEncryptionDTO.Name = "ThisIsAEncryptionDTOWithVeryLongTextNameValueThatExceedsTheMaximumLengthAllowedByTheDatabaseModelHaveAGoodDayNowSir";
            existingEncryptionDTOCopy.Name = existingEncryptionDTOOrigin.Name;

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _service.Update(shortEncryptionDTO), Throws.Exception);
                Assert.That(() => _service.Update(existingEncryptionDTOCopy), Throws.Exception);
                Assert.That(() => _service.Update(notExistingEncryptionDTO), Throws.Exception);
            });
        }

        [Test]
        public void Delete_ValidEncryptionDTO()
        {
            // Arrange
            var encryption1 = new EncryptionDTO("TestEncryptionDTO1");
            encryption1 = _service.Add(encryption1);

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _service.Delete(encryption1!.Id), Throws.Nothing);
                var savedEncryptionDTOs = _dbContext.Encryptions.ToList();
                Assert.That(savedEncryptionDTOs, Is.Empty);
            });
        }

        [Test]
        public void Delete_InvalidEncryptionDTO_ThrowsException()
        {
            // Act and Assert
            Assert.That(() => _service.Delete(1), Throws.Exception);
        }
    }
}
