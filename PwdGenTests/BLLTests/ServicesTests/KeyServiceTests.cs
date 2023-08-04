using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using PwdGenBLL.Converters;
using PwdGenBLL.Services;

using PwdGenDLL;
using PwdGenDLL.Models;
using PwdGenDLL.Repositories.Implementations;

namespace PwdGenTests.BLL.Services
{
    public class KeyServiceTests
    {
        private AppDbContext _dbContext;
        private KeyService _service;
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

            KeyRepository repository = new(_dbContext);
            KeyConverter converter = new();
            _service = new KeyService(repository, converter);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
            _connection.Close();
        }

        [Test]
        public void Add_ValidKeyDTO()
        {
            // Arrange
            var key = new KeyDTO("TestKey", 1);

            // Act
            key = _service.Add(key);

            // Assert
            Assert.That(key, Is.Not.Null);
            Assert.That(key.Id, Is.EqualTo(1));
        }

        [Test]
        public void Add_InvalidKeyDTO_ThrowsException()
        {
            // Arrange
            var longKeyDTO = new KeyDTO("ThisIsAKeyDTOWithVeryLongTextValueThatExceedsTheMaximumLengthAllowedByTheDatabaseModelHaveAGoodDayNowSir");
            var existingKeyDTO = new KeyDTO("ExistingKey");
            existingKeyDTO = _service.Add(existingKeyDTO);

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _service.Add(longKeyDTO), Throws.Exception);
                Assert.That(() => _service.Add(new KeyDTO(existingKeyDTO!.Value)), Throws.Exception);
            });
        }

        [Test]
        public void Get_ReturnsAllKeyDTOs()
        {
            // Arrange
            var key1 = new KeyDTO("KeyDTO1");
            var key2 = new KeyDTO("KeyDTO2");
            key1 = _service.Add(key1);
            key2 = _service.Add(key2);

            // Act
            var keys = _service.Get().ToList();

            // Assert
            Assert.That(keys, Has.Count.EqualTo(2));
            Assert.That(keys, Contains.Item(key1));
            Assert.That(keys, Contains.Item(key2));
        }

        [Test]
        public void Get_ValidKeyDTO_ReturnsKeyDTO()
        {
            // Arrange
            var key1 = new KeyDTO("TestKeyDTO1");
            var key2 = new KeyDTO("TestKeyDTO2");
            key1 = _service.Add(key1);
            key2 = _service.Add(key2);

            // Act
            var resultKeyDTO1 = _service.Get(key1?.Id ?? 0);
            var resultKeyDTO2 = _service.Get(k => k.Id == key2!.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultKeyDTO1, Is.Not.Null);
                Assert.That(resultKeyDTO2, Is.Not.Empty);
                Assert.That(resultKeyDTO1, Is.EqualTo(key1));
                Assert.That(resultKeyDTO2.First(), Is.EqualTo(key2));
            });
        }

        [Test]
        public void Get_InvalidKeyDTO_ReturnsNullOrEmpty()
        {
            // Arrange
            var key = new KeyDTO("TestKeyDTO");
            key = _service.Add(key);

            // Act
            var result1 = _service.Get(key!.Id + 1);
            var result2 = _service.Get(k => k.Id == (key.Id + 1));

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result1, Is.Null);
                Assert.That(result2, Is.Empty);
            });
        }

        [Test]
        public void Update_ValidKeyDTO()
        {
            // Arrange
            var key = new KeyDTO("TestKeyDTO");
            key = _service.Add(key);

            // Act
            key!.Value = "UpdatedValue";
            _service.Update(key);

            // Assert
            Assert.That(key, Is.Not.Null);
            Assert.That(key.Value, Is.EqualTo("UpdatedValue"));
        }

        [Test]
        public void Update_InvalidKeyDTO_ThrowsException()
        {
            // Arrange
            var notExistingKeyDTO = new KeyDTO("notExistingKeyDTO");
            var shortKeyDTO = new KeyDTO("shortKeyDTO");
            var existingKeyDTOOrigin = new KeyDTO("existingKeyDTOOrigin");
            var existingKeyDTOCopy = new KeyDTO("existingKeyDTOCopy");

            shortKeyDTO = _service.Add(shortKeyDTO);
            existingKeyDTOOrigin = _service.Add(existingKeyDTOOrigin);
            existingKeyDTOCopy = _service.Add(existingKeyDTOCopy);

            shortKeyDTO!.Value = "ThisIsAKeyDTOWithVeryLongTextValueThatExceedsTheMaximumLengthAllowedByTheDatabaseModelHaveAGoodDayNowSir";
            existingKeyDTOCopy!.Value = existingKeyDTOOrigin!.Value;

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _service.Update(shortKeyDTO), Throws.Exception);
                Assert.That(() => _service.Update(existingKeyDTOCopy), Throws.Exception);
                Assert.That(() => _service.Update(notExistingKeyDTO), Throws.Exception);
            });
        }

        [Test]
        public void Delete_ValidKeyDTO()
        {
            // Arrange
            var key1 = new KeyDTO("TestKeyDTO1");
            var addedKey1 = _service.Add(key1);

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _service.Delete(addedKey1!.Id), Throws.Nothing);
                var savedKeyDTOs = _dbContext.Keys.ToList();
                Assert.That(savedKeyDTOs, Is.Empty);

            });
        }

        [Test]
        public void Delete_InvalidKeyDTO_ThrowsException()
        {
            // Act and Assert
            Assert.That(() => _service.Delete(1), Throws.Exception);
        }
    }
}
