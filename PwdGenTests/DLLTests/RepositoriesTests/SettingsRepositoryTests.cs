using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using PwdGenDLL;
using PwdGenDLL.Models;
using PwdGenDLL.Repositories.Implementations;

namespace PwdGenTests.DLL.Repositories
{
    public class SettingsRepositoryTests
    {
        private AppDbContext _dbContext;
        private SettingsRepository _repository;
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

            _repository = new SettingsRepository(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
            _connection.Close();
        }

        [Test]
        public void Add_ValidSettings()
        {
            // Arrange
            var encryption = new Encryption { Id = 1, Name = "AES" };
            var key = new Key { Id = 1, Value = "TestKey" };
            var settings = new Settings
            {
                Id = 1,
                Encryption = encryption,
                Key = key
            };

            // Act
            _repository.Add(settings);

            // Assert
            var result = _repository.Get(settings.Id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(settings));
        }

        [Test]
        public void Add_InvalidSettings_ThrowsException()
        {
            // Arrange
            var encryption = new Encryption { Id = 1, Name = "AES" };
            var key = new Key { Id = 1, Value = "TestKey" };
            var emptySettings = new Settings();
            var withoutEncryptionSettings = new Settings { Key = key };
            var withoutKeySettings = new Settings { Encryption = encryption };
            var sameSettings1 = new Settings { Key = key, Encryption = encryption };
            var sameSettings2 = new Settings { Key = key, Encryption = encryption };
            _repository.Add(sameSettings1);

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _repository.Add(emptySettings), Throws.Exception);
                Assert.That(() => _repository.Add(withoutEncryptionSettings), Throws.Exception);
                Assert.That(() => _repository.Add(withoutKeySettings), Throws.Exception);
                Assert.That(() => _repository.Add(sameSettings2), Throws.Exception);
            });
        }

        [Test]
        public void Get_ReturnsAllSettings()
        {
            // Arrange
            var encryption = new Encryption { Id = 1, Name = "AES" };
            var key1 = new Key { Id = 1, Value = "TestKey1" };
            var key2 = new Key { Id = 2, Value = "TestKey2" };
            var settings1 = new Settings { Id = 1, Encryption = encryption, Key = key1 };
            var settings2 = new Settings { Id = 2, Encryption = encryption, Key = key2 };
            _repository.Add(settings1);
            _repository.Add(settings2);

            // Act
            var settings = _repository.Get().ToList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(settings, Has.Count.EqualTo(2));
                Assert.That(settings, Contains.Item(settings1));
                Assert.That(settings, Contains.Item(settings2));
                Assert.That(!string.IsNullOrEmpty(settings.FirstOrDefault(s => s.Id == 1)?.Key.Value));
                Assert.That(!string.IsNullOrEmpty(settings.FirstOrDefault(s => s.Id == 2)?.Key.Value));
            });
        }

        [Test]
        public void Get_ValidSettings_ReturnsSettings()
        {
            // Arrange
            var encryption = new Encryption { Id = 1, Name = "AES" };
            var key1 = new Key { Id = 1, Value = "TestKey1" };
            var key2 = new Key { Id = 2, Value = "TestKey2" };
            var settings1 = new Settings { Id = 1, Encryption = encryption, Key = key1 };
            var settings2 = new Settings { Id = 2, Encryption = encryption, Key = key2 };
            _repository.Add(settings1);
            _repository.Add(settings2);

            // Act
            var resultSettings = _repository.Get().ToList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultSettings.FirstOrDefault(s => s.Id == 1), Is.Not.Null);
                Assert.That(resultSettings.FirstOrDefault(s => s.Id == 2), Is.Not.Null);
                Assert.That(resultSettings.FirstOrDefault(s => s.Id == 1), Is.EqualTo(settings1));
                Assert.That(resultSettings.FirstOrDefault(s => s.Id == 2), Is.EqualTo(settings2));
                Assert.That(resultSettings.FirstOrDefault(s => s.Id == 1)?.Encryption.Name, Is.Not.Null);
                Assert.That(resultSettings.FirstOrDefault(s => s.Id == 2)?.Key.Value, Is.Not.Null);
            });
        }

        [Test]
        public void Get_InvalidSettings_ReturnsNull()
        {
            // Arrange
            var encryption = new Encryption { Id = 1, Name = "AES" };
            var key = new Key { Id = 1, Value = "TestKey" };
            var settings1 = new Settings { Id = 1, Encryption = encryption, Key = key };
            _repository.Add(settings1);

            // Act
            var result1 = _repository.Get(settings1.Id + 1);
            var result2 = _repository.Get(s => s.Id == settings1.Id + 1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result1, Is.Null);
                Assert.That(result2, Is.Empty);
            });
        }

        [Test]
        public void Update_ValidSettings()
        {
            // Arrange
            var encryption1 = new Encryption { Id = 1, Name = "AES1" };
            var key1 = new Key { Id = 1, Value = "TestKey1" };
            var encryption2 = new Encryption { Id = 2, Name = "AES2" };
            var key2 = new Key { Id = 2, Value = "TestKey2" };
            var settings = new Settings { Id = 1, Encryption = encryption1, Key = key1 };
            _repository.Add(settings);
            _dbContext.Add(key2);
            _dbContext.Add(encryption2);

            // Act
            settings.EncryptionId = 2;
            settings.KeyId = 2;
            _repository.Update(settings);

            // Assert
            var updatedSettings = _repository.Get(settings.Id);
            Assert.Multiple(() =>
            {
                Assert.That(updatedSettings, Is.Not.Null);
                Assert.That(updatedSettings?.EncryptionId, Is.EqualTo(2));
                Assert.That(updatedSettings?.KeyId, Is.EqualTo(2));
            });
        }

        [Test]
        public void Update_InvalidSettings_ThrowsException()
        {
            // Arrange
            var encryption1 = new Encryption { Id = 1, Name = "AES1" };
            var encryption2 = new Encryption { Id = 2, Name = "AES2" };
            var key1 = new Key { Id = 1, Value = "TestKey1" };
            var key2 = new Key { Id = 2, Value = "TestKey2" };
            var settings1 = new Settings { Id = 1, Encryption = encryption1, Key = key1 };
            var settings2 = new Settings { Id = 2, Encryption = encryption2, Key = key2 };
            _repository.Add(settings1);
            _repository.Add(settings2);

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _repository.Update(new Settings { Id = 3, EncryptionId = 1, KeyId = 1 }), Throws.Exception);
                settings1.Encryption = settings2.Encryption;
                settings1.Key = settings2.Key;
                Assert.That(() => _repository.Update(settings1), Throws.Exception);
            });
        }

        [Test]
        public void Delete_ValidSettings()
        {
            // Arrange
            var encryption1 = new Encryption { Id = 1, Name = "AES1" };
            var encryption2 = new Encryption { Id = 2, Name = "AES2" };
            var key = new Key { Id = 1, Value = "TestKey" };
            var settings1 = new Settings { Id = 1, Encryption = encryption1, Key = key };
            var settings2 = new Settings { Id = 2, Encryption = encryption2, Key = key };
            _repository.Add(settings1);
            _repository.Add(settings2);

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _repository.Delete(settings1.Id), Throws.Nothing);
                Assert.That(() => _repository.Delete(settings2), Throws.Nothing);
                var savedSettings = _dbContext.Settings.ToList();
                Assert.That(savedSettings, Is.Empty);
            });
        }

        [Test]
        public void Delete_InvalidSettings_ThrowsException()
        {
            // Act and Assert
            Assert.That(() => _repository.Delete(1), Throws.Exception);
            Assert.That(() => _repository.Delete(new Settings() { Id = 1}), Throws.Exception);
        }
    }
}
