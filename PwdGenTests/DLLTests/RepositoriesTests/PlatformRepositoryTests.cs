using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using PwdGenDLL;
using PwdGenDLL.Models;
using PwdGenDLL.Repositories.Implementations;

namespace PwdGenTests.DLL.Repositories
{
    public class PlatformServiceTests
    {
        private AppDbContext _dbContext;
        private PlatformRepository _repository;
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

            _repository = new PlatformRepository(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
            _connection.Close();
        }

        [Test]
        public void Add_ValidPlatform()
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
            var platform = new Platform
            {
                Name = "test",
                PasswordHistory = passwordHistory,
                IconPath = "C:/test.ico"
            };

            // Act
            _repository.Add(platform);

            // Assert
            var result = _repository.Get(platform.Id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(platform));
        }

        [Test]
        public void Add_InvalidPlatform()
        {
            // Arrange
            var emptyPlatform = new Platform();
            var passwordHistory = new PasswordHistory { Id = 1, EncryptedText = "TestEncryptedText" };
            var withoutNamePlatform = new Platform
            {
                PasswordHistory = passwordHistory
            };
            var longNamePlatform = new Platform
            {
                PasswordHistory = passwordHistory,
                Name = "ThisIsAPlatformWithVeryLongEncryptedTextValueThatExceedsTheMaximumLengthAllowedByTheDatabaseModelHaveAGoodDayNowSir"
            };
            var withoutPHPlatform = new Platform
            {
                Name = "TestPlatformName"
            };

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _repository.Add(emptyPlatform), Throws.Exception);
                Assert.That(() => _repository.Add(withoutNamePlatform), Throws.Exception);
                Assert.That(() => _repository.Add(longNamePlatform), Throws.Exception);
                Assert.That(() => _repository.Add(withoutPHPlatform), Throws.Exception);
            });
        }

        [Test]
        public void Get_ReturnsAllPlatforms()
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
            var platform1 = new Platform
            {
                Name = "TestPlatform1",
                PasswordHistory = passwordHistory1
            };
            var platform2 = new Platform
            {
                Name = "TestPlatform2",
                PasswordHistory = passwordHistory2
            };

            _repository.Add(platform1);
            _repository.Add(platform2);

            // Act
            var platforms = _repository.Get().ToList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(platforms, Has.Count.EqualTo(2));
                Assert.That(platforms, Contains.Item(platform1));
                Assert.That(platforms, Contains.Item(platform2));
                Assert.That(platform1.PasswordHistory.Settings?.Key, Is.EqualTo(key));
                Assert.That(platform1.PasswordHistory.Settings?.Encryption, Is.EqualTo(encryption));
                Assert.That(platform2.PasswordHistory.Settings, Is.Null);
            });
        }

        [Test]
        public void Get_ValidPlatform_ReturnsPlatform()
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
            var platform1 = new Platform
            {
                Name = "TestPlatform1",
                PasswordHistory = passwordHistory1
            };
            var platform2 = new Platform
            {
                Name = "TestPlatform2",
                PasswordHistory = passwordHistory2
            };

            _repository.Add(platform1);
            _repository.Add(platform2);

            // Act
            var resultPlatform1 = _repository.Get(platform1.Id);
            var resultPlatform2 = _repository.Get(p => p.Id == platform2.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultPlatform1, Is.Not.Null);
                Assert.That(resultPlatform2, Is.Not.Empty);
                Assert.That(resultPlatform1, Is.EqualTo(platform1));
                Assert.That(resultPlatform2.First(), Is.EqualTo(platform2));
            });
        }

        [Test]
        public void Get_InvalidPlatform_ReturnsNullOrEmpty()
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
            var platform1 = new Platform
            {
                Name = "TestPlatform1",
                PasswordHistory = passwordHistory1
            };
            var platform2 = new Platform
            {
                Name = "TestPlatform2",
                PasswordHistory = passwordHistory2
            };

            _repository.Add(platform1);
            _repository.Add(platform2);

            // Act
            var result1 = _repository.Get(platform1.Id + 2);
            var result2 = _repository.Get(p => p.Id == (platform2.Id + 1));

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result1, Is.Null);
                Assert.That(result2, Is.Empty);
            });
        }

        [Test]
        public void Update_ValidPlatform()
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
            var platform = new Platform
            {
                Name = "TestPlatform",
                PasswordHistory = passwordHistory
            };
            _repository.Add(platform);

            // Act
            platform.Name = "UpdatedTestPlatform";
            platform.PasswordHistory = new PasswordHistory { EncryptedText = "NewPasswordHistory" };
            platform.IconPath = "C:/Icons/icon.ico";
            _repository.Update(platform);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(platform, Is.Not.Null);
                Assert.That(platform.Name, Is.EqualTo("UpdatedTestPlatform"));
                Assert.That(platform.PasswordHistory.EncryptedText, Is.EqualTo("NewPasswordHistory"));
                Assert.That(platform.IconPath, Is.EqualTo("C:/Icons/icon.ico"));
            });
        }

        [Test]
        public void Update_InvalidPlatform_ThrowsException()
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
            var platform = new Platform
            {
                Name = "TestPlatform",
                PasswordHistory = passwordHistory
            };
            _repository.Add(platform);

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _repository.Update(new Platform { Id = 2 }), Throws.Exception);
                platform.Name = "ThisIsAPlatformWithVeryLongNameValueThatExceedsTheMaximumLengthAllowedByTheDatabaseModelHaveAGoodDayNowSir";
                Assert.That(() => _repository.Update(platform), Throws.Exception);
            });
        }

        [Test]
        public void Delete_ValidPlatform()
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
            var platform1 = new Platform
            {
                Name = "TestPlatform1",
                PasswordHistory = passwordHistory1
            };

            _repository.Add(platform1);

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _repository.Delete(platform1.Id), Throws.Nothing);
                var savedPasswordHistories = _dbContext.Platforms.ToList();
                Assert.That(savedPasswordHistories, Is.Empty);
            });
        }

        [Test]
        public void Delete_InvalidPlatform_ThrowsException()
        {
            // Act and Assert
            Assert.That(() => _repository.Delete(1), Throws.Exception);
        }
    }
}
