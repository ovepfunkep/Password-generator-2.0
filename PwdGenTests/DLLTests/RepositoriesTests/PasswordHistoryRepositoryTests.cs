using NUnit.Framework;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PwdGenDLL;
using PwdGenDLL.Models;
using PwdGenDLL.Repositories.Implementations;
using System;
using System.Linq;

namespace PwdGenTests.DLL.Repositories
{
    public class PasswordHistoryRepositoryTests
    {
        private AppDbContext _dbContext;
        private PasswordHistoryRepository _repository;
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

            _repository = new PasswordHistoryRepository(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
            _connection.Close();
        }

        [Test]
        public void Add_ValidPasswordHistory()
        {
            // Arrange
            var encryption = new Encryption { Id = 1, Name = "AES" };
            var key = new Key { Id = 1, Value = "TestKey" };
            var settings = new Settings { Id = 1, Encryption = encryption, Key = key };
            var passwordHistory = new PasswordHistory
            {
                Id = 1,
                SourceText = "TestSourceText",
                EncryptedText = "TestEncryptedText",
                Date = DateTime.Now,
                Settings = settings
            };

            // Act
            _repository.Add(passwordHistory);

            // Assert
            var result = _repository.Get(passwordHistory.Id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(passwordHistory));
        }

        [Test]
        public void Add_InvalidPasswordHistory()
        {
            // Arrange
            var emptyPasswordHistory = new PasswordHistory();
            var withoutEncryptedTextPasswordHistory = new PasswordHistory
            {
                Date = DateTime.Now,
                SourceText = "Test",
                SettingsId = 1
            };
            var longSourceTextPasswordHistory = new PasswordHistory
            {
                Date = DateTime.Now,
                SourceText = "ThisIsAPasswordHistoryWithVeryLongSourceTextValueThatExceedsTheMaximumLengthAllowedByTheDatabaseModelHaveAGoodDayNowSir",
                SettingsId = 1
            };

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _repository.Add(emptyPasswordHistory), Throws.Exception);
                Assert.That(() => _repository.Add(withoutEncryptedTextPasswordHistory), Throws.Exception);
                Assert.That(() => _repository.Add(longSourceTextPasswordHistory), Throws.Exception);
            });
        }

        [Test]
        public void Get_ReturnsAllPasswordHistories()
        {
            // Arrange
            var encryption = new Encryption { Id = 1, Name = "AES" };
            var key = new Key { Id = 1, Value = "TestKey" };
            var settings = new Settings { Id = 1, Encryption = encryption, Key = key };
            var passwordHistory1 = new PasswordHistory
            {
                Id = 1,
                SourceText = "TestSourceText1",
                EncryptedText = "TestEncryptedText1",
                Date = DateTime.Now,
                Settings = settings
            };
            var passwordHistory2 = new PasswordHistory
            {
                Id = 2,
                SourceText = "TestSourceText2",
                EncryptedText = "TestEncryptedText2",
                Date = DateTime.Now
            };
            _repository.Add(passwordHistory1);
            _repository.Add(passwordHistory2);

            // Act
            var passwordHistories = _repository.Get().ToList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(passwordHistories, Has.Count.EqualTo(2));
                Assert.That(passwordHistories, Contains.Item(passwordHistory1));
                Assert.That(passwordHistories, Contains.Item(passwordHistory2));
                Assert.That(passwordHistory1.Settings.Key, Is.EqualTo(key));
                Assert.That(passwordHistory1.Settings.Encryption, Is.EqualTo(encryption));
                Assert.That(passwordHistory2.Settings, Is.Null);
            });
        }

        [Test]
        public void Get_ValidPasswordHistory_ReturnsPasswordHistory()
        {
            // Arrange
            var encryption = new Encryption { Id = 1, Name = "AES" };
            var key = new Key { Id = 1, Value = "TestKey" };
            var settings = new Settings { Id = 1, Encryption = encryption, Key = key };
            var passwordHistory1 = new PasswordHistory
            {
                Id = 1,
                SourceText = "TestSourceText1",
                EncryptedText = "TestEncryptedText1",
                Date = DateTime.Now,
                Settings = settings
            };
            var passwordHistory2 = new PasswordHistory
            {
                Id = 2,
                SourceText = "TestSourceText2",
                EncryptedText = "TestEncryptedText2",
                Date = DateTime.Now,
                Settings = settings
            };
            _repository.Add(passwordHistory1);
            _repository.Add(passwordHistory2);

            // Act
            var resultPasswordHistory1 = _repository.Get(passwordHistory1.Id);
            var resultPasswordHistory2 = _repository.Get(p => p.Id == passwordHistory2.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultPasswordHistory1, Is.Not.Null);
                Assert.That(resultPasswordHistory2, Is.Not.Empty);
                Assert.That(resultPasswordHistory1, Is.EqualTo(passwordHistory1));
                Assert.That(resultPasswordHistory2.First(), Is.EqualTo(passwordHistory2));
            });
        }

        [Test]
        public void Get_InvalidPasswordHistory_ReturnsNull()
        {
            // Arrange
            var encryption = new Encryption { Id = 1, Name = "AES" };
            var key = new Key { Id = 1, Value = "TestKey" };
            var settings = new Settings { Id = 1, Encryption = encryption, Key = key };
            var passwordHistory1 = new PasswordHistory
            {
                Id = 1,
                SourceText = "TestSourceText1",
                EncryptedText = "TestEncryptedText1",
                Date = DateTime.Now,
                Settings = settings
            };
            var passwordHistory2 = new PasswordHistory
            {
                Id = 2,
                SourceText = "TestSourceText2",
                EncryptedText = "TestEncryptedText2",
                Date = DateTime.Now,
                Settings = settings
            };
            _repository.Add(passwordHistory1);
            _repository.Add(passwordHistory2);

            // Act
            var result1 = _repository.Get(passwordHistory1.Id + 2);
            var result2 = _repository.Get(p => p.Id == (passwordHistory2.Id + 1));

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result1, Is.Null);
                Assert.That(result2, Is.Empty);
            });
        }

        [Test]
        public void Update_ValidPasswordHistory()
        {
            // Arrange
            var encryption = new Encryption { Id = 1, Name = "AES" };
            var key = new Key { Id = 1, Value = "TestKey" };
            var settings = new Settings { Id = 1, Encryption = encryption, Key = key };
            var passwordHistory = new PasswordHistory
            {
                Id = 1,
                SourceText = "TestSourceText",
                EncryptedText = "TestEncryptedText",
                Date = DateTime.Now,
                Settings = settings
            };
            _repository.Add(passwordHistory);

            // Act
            passwordHistory.SourceText = "UpdatedSourceText";
            passwordHistory.EncryptedText = "UpdatedEncryptedText";
            passwordHistory.Date = DateTime.Now.AddDays(1);
            _repository.Update(passwordHistory);

            // Assert
            var updatedPasswordHistory = _repository.Get(passwordHistory.Id);
            Assert.Multiple(() =>
            {
                Assert.That(updatedPasswordHistory, Is.Not.Null);
                Assert.That(updatedPasswordHistory?.SourceText, Is.EqualTo("UpdatedSourceText"));
                Assert.That(updatedPasswordHistory?.EncryptedText, Is.EqualTo("UpdatedEncryptedText"));
                Assert.That(updatedPasswordHistory?.Date, Is.EqualTo(passwordHistory.Date));
            });
        }

        [Test]
        public void Update_InvalidPasswordHistory_ThrowsException()
        {
            // Arrange
            var encryption = new Encryption { Id = 1, Name = "AES" };
            var key = new Key { Id = 1, Value = "TestKey" };
            var settings = new Settings { Id = 1, Encryption = encryption, Key = key };
            var passwordHistory = new PasswordHistory
            {
                Id = 1,
                SourceText = "TestSourceText",
                EncryptedText = "TestEncryptedText",
                Date = DateTime.Now,
                Settings = settings
            };
            _repository.Add(passwordHistory);

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _repository.Update(new PasswordHistory { Id = 2 }), Throws.Exception);
                passwordHistory.SourceText = "ThisIsAPasswordHistoryWithVeryLongEncryptedTextValueThatExceedsTheMaximumLengthAllowedByTheDatabaseModelHaveAGoodDayNowSir";
                Assert.That(() => _repository.Update(passwordHistory), Throws.Exception);
            });
        }

        [Test]
        public void Delete_ValidPasswordHistory_ThrowsException()
        {
            // Arrange
            var encryption = new Encryption { Id = 1, Name = "AES" };
            var key = new Key { Id = 1, Value = "TestKey" };
            var settings = new Settings { Id = 1, Encryption = encryption, Key = key };
            var passwordHistory1 = new PasswordHistory
            {
                Id = 1,
                SourceText = "TestSourceText1",
                EncryptedText = "TestEncryptedText1",
                Date = DateTime.Now,
                Settings = settings
            };
            var passwordHistory2 = new PasswordHistory
            {
                Id = 2,
                SourceText = "TestSourceText2",
                EncryptedText = "TestEncryptedText2",
                Date = DateTime.Now,
                Settings = settings
            };
            _repository.Add(passwordHistory1);
            _repository.Add(passwordHistory2);

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _repository.Delete(passwordHistory1.Id), Throws.Nothing);
                Assert.That(() => _repository.Delete(passwordHistory2), Throws.Nothing);
                var savedPasswordHistories = _dbContext.PasswordHistories.ToList();
                Assert.That(savedPasswordHistories, Is.Empty);
            });
        }

        [Test]
        public void Delete_InvalidPasswordHistory_ThrowsException()
        {
            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _repository.Delete(1), Throws.Exception);
                Assert.That(() => _repository.Delete(new PasswordHistory() { Id = 1 }), Throws.Exception);
            });
        }
    }
}
