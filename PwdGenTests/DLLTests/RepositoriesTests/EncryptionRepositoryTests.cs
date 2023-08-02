using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using PwdGenDLL;
using PwdGenDLL.Models;
using PwdGenDLL.Repositories.Implementations;

namespace PwdGenTests.DLL.Repositories
{
    public class EncryptionRepositoryTests
    {
        private AppDbContext _dbContext;
        private EncryptionRepository _repository;
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

            _repository = new EncryptionRepository(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
            _connection.Close();
        }

        [Test]
        public void Add_ValidEncryption()
        {
            // Arrange
            var encryption = new Encryption
            {
                Id = 1,
                Name = "test"
            };

            // Act
            _repository.Add(encryption);

            // Assert
            var result = _repository.Get(encryption.Id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(encryption));
        }

        [Test]
        public void Add_InvalidEncryption_ThrowsException()
        {
            // Arrange
            var emptyEncryption = new Encryption();
            var longEncryption = new Encryption { Name = "ThisIsAEncryptionWithVeryLongTextNameValueThatExceedsTheMaximumLengthAllowedByTheDatabaseModelHaveAGoodDayNowSir" };
            var existingEncryption = new Encryption { Id = 1, Name = "123" };
            _repository.Add(existingEncryption);

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _repository.Add(emptyEncryption), Throws.Exception);
                Assert.That(() => _repository.Add(longEncryption), Throws.Exception);
                Assert.That(() => _repository.Add(new Encryption { Name = existingEncryption.Name }), Throws.Exception);
            });
        }

        [Test]
        public void Get_ReturnsAllEncryptions()
        {
            // Arrange
            var encryption1 = new Encryption { Name = "Encryption1" };
            var encryption2 = new Encryption { Name = "Encryption2" };
            _repository.Add(encryption1);
            _repository.Add(encryption2);

            // Act
            var encryptions = _repository.Get().ToList();

            // Assert
            Assert.That(encryptions, Has.Count.EqualTo(2));
            Assert.That(encryptions, Contains.Item(encryption1));
            Assert.That(encryptions, Contains.Item(encryption2));
        }

        [Test]
        public void Get_ValidEncryption_ReturnsEncryption()
        {
            // Arrange
            var encryption1 = new Encryption { Name = "TestEncryption1" };
            var encryption2 = new Encryption { Id = 2, Name = "TestEncryption2" };
            _repository.Add(encryption1);
            _repository.Add(encryption2);

            // Act
            var resultEncryption1 = _repository.Get(encryption1?.Id ?? 0);
            var resultEncryption2 = _repository.Get(e => e.Id == encryption2.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultEncryption1, Is.Not.Null);
                Assert.That(resultEncryption2, Is.Not.Empty);
                Assert.That(resultEncryption1, Is.EqualTo(encryption1));
                Assert.That(resultEncryption2.First(), Is.EqualTo(encryption2));
            });
        }

        [Test]
        public void Get_InvalidEncryption_ReturnsNull()
        {
            // Arrange
            var encryption = new Encryption { Name = "TestEncryption" };
            _repository.Add(encryption);

            // Act
            var result1 = _repository.Get(encryption.Id + 1);
            var result2 = _repository.Get(e => e.Id == (encryption.Id + 1));

            // Assert
            Assert.That(result1, Is.Null);
            Assert.That(result2, Is.Empty);
        }

        [Test]
        public void Update_ValidEncryption()
        {
            // Arrange
            var encryption = new Encryption { Name = "TestEncryption" };
            _repository.Add(encryption);

            // Act
            encryption.Name = "UpdatedName";
            _repository.Update(encryption);

            // Assert
            Assert.That(encryption, Is.Not.Null);
            Assert.That(encryption.Name, Is.EqualTo("UpdatedName"));
        }

        [Test]
        public void Update_InvalidEncryption_ThrowsException()
        {
            // Arrange
            var notExistingEncryption = new Encryption { Name = "notExistingEncryption" };
            var shortEncryption = new Encryption { Id = 1, Name = "shortEncryption" };
            var existingEncryptionOrigin = new Encryption { Id = 2, Name = "existingEncryptionOrigin" };
            var existingEncryptionCopy = new Encryption { Id = 3, Name = "existingEncryptionCopy" };

            _repository.Add(shortEncryption);
            _repository.Add(existingEncryptionOrigin);
            _repository.Add(existingEncryptionCopy);

            shortEncryption.Name = "ThisIsAEncryptionWithVeryLongTextNameValueThatExceedsTheMaximumLengthAllowedByTheDatabaseModelHaveAGoodDayNowSir";
            existingEncryptionCopy.Name = existingEncryptionOrigin.Name;

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _repository.Update(shortEncryption), Throws.Exception);
                Assert.That(() => _repository.Update(existingEncryptionCopy), Throws.Exception);
                Assert.That(() => _repository.Update(notExistingEncryption), Throws.Exception);
            });
        }

        [Test]
        public void Delete_ValidEncryption_ThrowsException()
        {
            // Arrange
            var encryption1 = new Encryption { Name = "TestEncryption1" };
            var encryption2 = new Encryption { Name = "TestEncryption2" };
            _repository.Add(encryption1);
            _repository.Add(encryption2);
            
            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _repository.Delete(encryption1.Id), Throws.Nothing);
                Assert.That(() => _repository.Delete(encryption2), Throws.Nothing);
                var savedEncryptions = _dbContext.Encryptions.ToList();
                Assert.That(savedEncryptions, Is.Empty);
            });
        }

        [Test]
        public void Delete_InvalidEncryption_ThrowsException()
        {
            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _repository.Delete(1), Throws.Exception);
                Assert.That(() => _repository.Delete(new Encryption() { Id = 1 }), Throws.Exception);
            });
        }
    }
}
