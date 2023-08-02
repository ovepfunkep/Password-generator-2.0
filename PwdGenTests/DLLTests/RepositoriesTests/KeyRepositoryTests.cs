using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using PwdGenDLL;
using PwdGenDLL.Models;
using PwdGenDLL.Repositories.Implementations;

namespace PwdGenTests.DLL.Repositories
{
    public class KeyRepositoryTests
    {
        private AppDbContext _dbContext;
        private KeyRepository _repository;
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

            _repository = new KeyRepository(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
            _connection.Close(); 
        }

        [Test]
        public void Add_ValidKey()
        {
            // Arrange
            var key = new Key
            {
                Id = 1,
                Value = "TestKey"
            };

            // Act
            _repository.Add(key);

            // Assert
            var result = _repository.Get(key.Id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(key));
        }

        [Test]
        public void Add_InvalidKey_ThrowsException()
        {
            // Arrange
            var emptyKey = new Key();
            var longKey = new Key { Value = "ThisIsAKeyWithVeryLongTextValueThatExceedsTheMaximumLengthAllowedByTheDatabaseModelHaveAGoodDayNowSir" };
            var existingKey = new Key { Id = 1, Value = "123" };
            _repository.Add(existingKey);

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _repository.Add(emptyKey), Throws.Exception);
                Assert.That(() => _repository.Add(longKey), Throws.Exception);
                Assert.That(() => _repository.Add(new Key { Value = existingKey.Value }), Throws.Exception);
            });
        }

        [Test]
        public void Get_ReturnsAllKeys()
        {
            // Arrange
            var key1 = new Key { Value = "Key1" };
            var key2 = new Key { Value = "Key2" };
            _repository.Add(key1);
            _repository.Add(key2);

            // Act
            var keys = _repository.Get().ToList();

            // Assert
            Assert.That(keys, Has.Count.EqualTo(2));
            Assert.That(keys, Contains.Item(key1));
            Assert.That(keys, Contains.Item(key2));
        }

        [Test]
        public void Get_ValidKey_ReturnsKey()
        {
            // Arrange
            var key1 = new Key { Value = "TestKey1" };
            var key2 = new Key { Id = 2, Value = "TestKey2" };
            _repository.Add(key1);
            _repository.Add(key2);

            // Act
            var resultKey1 = _repository.Get(key1?.Id ?? 0);
            var resultKey2 = _repository.Get(k => k.Id == key2.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultKey1, Is.Not.Null);
                Assert.That(resultKey2, Is.Not.Empty);
                Assert.That(resultKey1, Is.EqualTo(key1));
                Assert.That(resultKey2.First(), Is.EqualTo(key2));
            });
        }

        [Test]
        public void Get_InvalidKey_ReturnsNull()
        {
            // Arrange
            var key = new Key { Value = "TestKey" };
            _repository.Add(key);

            // Act
            var result1 = _repository.Get(key.Id + 1);
            var result2 = _repository.Get(k => k.Id == (key.Id + 1));

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result1, Is.Null);
                Assert.That(result2, Is.Empty);
            });
        }

        [Test]
        public void Update_ValidKey()
        {
            // Arrange
            var key = new Key { Value = "TestKey" };
            _repository.Add(key);

            // Act
            key.Value = "UpdatedValue";
            _repository.Update(key);

            // Assert
            Assert.That(key, Is.Not.Null);
            Assert.That(key.Value, Is.EqualTo("UpdatedValue"));
        }

        [Test]
        public void Update_InvalidKey_ThrowsException()
        {
            // Arrange
            var notExistingKey = new Key { Value = "notExistingKey" };
            var shortKey = new Key { Id = 1, Value = "shortKey" };
            var existingKeyOrigin = new Key { Id = 2, Value = "existingKeyOrigin" };
            var existingKeyCopy = new Key { Id = 3, Value = "existingKeyCopy" };

            _repository.Add(shortKey);
            _repository.Add(existingKeyOrigin);
            _repository.Add(existingKeyCopy);

            shortKey.Value = "ThisIsAKeyWithVeryLongTextValueThatExceedsTheMaximumLengthAllowedByTheDatabaseModelHaveAGoodDayNowSir";
            existingKeyCopy.Value = existingKeyOrigin.Value;

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _repository.Update(shortKey), Throws.Exception);
                Assert.That(() => _repository.Update(existingKeyCopy), Throws.Exception);
                Assert.That(() => _repository.Update(notExistingKey), Throws.Exception);
            });
        }

        [Test]
        public void Delete_ValidKey_ThrowsException()
        {
            // Arrange
            var key1 = new Key { Value = "TestKey1" };
            var key2 = new Key { Value = "TestKey2" };
            _repository.Add(key1);
            _repository.Add(key2);

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _repository.Delete(key1.Id), Throws.Nothing);
                Assert.That(() => _repository.Delete(key2), Throws.Nothing);
                var savedKeys = _dbContext.Keys.ToList();
                Assert.That(savedKeys, Is.Empty);

            });
        }

        [Test]
        public void Delete_InvalidKey_ThrowsException()
        {
            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _repository.Delete(1), Throws.Exception);
                Assert.That(() => _repository.Delete(new Key() { Id = 1 }), Throws.Exception);
            });
        }
    }
}
