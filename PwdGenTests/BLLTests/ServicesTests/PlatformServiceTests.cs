using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using PwdGenBLL.Converters;
using PwdGenBLL.Services;

using PwdGenDLL;
using PwdGenDLL.Models;
using PwdGenDLL.Repositories.Implementations;

namespace PwdGenTests.BLL.Services
{
    public class PlatformServiceTests
    {
        private AppDbContext _dbContext;
        private PlatformService _service;
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

            PlatformRepository repository = new(_dbContext);
            KeyConverter keyConverter = new();
            EncryptionConverter encryptionConverter = new();
            SettingsConverter settingsConverter = new(encryptionConverter, keyConverter);
            PasswordHistoryConverter passwordHistoryConverter = new(settingsConverter);
            PlatformConverter platformConverter = new(passwordHistoryConverter);
            _service = new PlatformService(repository, platformConverter);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
            _connection.Close();
        }

        [Test]
        public void Add_ValidPlatformDTO()
        {
            // Arrange
            var encryptionDTO = new EncryptionDTO("AES");
            var keyDTO = new KeyDTO("TestKeyDTO");
            var settingsDTO = new SettingsDTO(encryptionDTO, keyDTO);
            var passwordHistoryDTO = new PasswordHistoryDTO("EncryptionTest", 1, "SourceText", DateTime.Now, settingsDTO);
            var platformDTO = new PlatformDTO("PlatformTest", passwordHistoryDTO, 1, "C:/Icons/icon.ico");

            // Act
            platformDTO = _service.Add(platformDTO);

            // Assert
            var result = _service.Get(platformDTO!.Id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(platformDTO));
        }

        [Test]
        public void Add_InvalidPlatformDTO()
        {
            // Arrange
            var passwordHistoryDTO = new PasswordHistoryDTO("TestEncryptedText");
            var longNamePlatformDTO = new PlatformDTO("ThisIsAPlatformDTOWithVeryLongEncryptedTextValueThatExceedsTheMaximumLengthAllowedByTheDatabaseModelHaveAGoodDayNowSir", passwordHistoryDTO);

            // Act and Assert
            Assert.That(() => _service.Add(longNamePlatformDTO), Throws.Exception);
        }

        [Test]
        public void Get_ReturnsAllPlatformDTOs()
        {
            // Arrange
            var encryptionDTO = new EncryptionDTO("AES");
            var keyDTO = new KeyDTO("TestKeyDTO");
            var settingsDTO = new SettingsDTO(encryptionDTO, keyDTO);
            var passwordHistoryDTO1 = new PasswordHistoryDTO("EncryptionTest1", 1, "SourceText1", DateTime.Now, settingsDTO);
            var passwordHistoryDTO2 = new PasswordHistoryDTO("EncryptionTest2", 2, "SourceText2", DateTime.Now);
            var platformDTO1 = new PlatformDTO("TestPlatform1", passwordHistoryDTO1);
            var platformDTO2 = new PlatformDTO("TestPlatform2", passwordHistoryDTO2);

            platformDTO1 = _service.Add(platformDTO1);
            platformDTO2 = _service.Add(platformDTO2);

            // Act
            var platformDTOs = _service.Get().ToList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(platformDTOs, Has.Count.EqualTo(2));
                Assert.That(platformDTOs, Contains.Item(platformDTO1));
                Assert.That(platformDTOs, Contains.Item(platformDTO2));
                Assert.That(platformDTO1!.PasswordHistoryDTO.SettingsDTO?.KeyDTO, Is.Not.Null);
                Assert.That(platformDTO1.PasswordHistoryDTO.SettingsDTO?.EncryptionDTO, Is.Not.Null);
                Assert.That(platformDTO2!.PasswordHistoryDTO.SettingsDTO, Is.Null);
            });
        }

        [Test]
        public void Get_ValidPlatformDTO_ReturnsPlatformDTO()
        {
            // Arrange
            var encryptionDTO1 = new EncryptionDTO("AES1");
            var encryptionDTO2 = new EncryptionDTO("AES2");
            var keyDTO1 = new KeyDTO("TestKeyDTO1");
            var keyDTO2 = new KeyDTO("TestKeyDTO2");
            var settingsDTO1 = new SettingsDTO(encryptionDTO1, keyDTO1);
            var settingsDTO2 = new SettingsDTO(encryptionDTO2, keyDTO2);
            var passwordHistoryDTO1 = new PasswordHistoryDTO("EncryptionTest1", 1, "SourceText1", DateTime.Now, settingsDTO1);
            var passwordHistoryDTO2 = new PasswordHistoryDTO("EncryptionTest2", 2, "SourceText2", DateTime.Now, settingsDTO2);
            var platformDTO1 = new PlatformDTO("TestPlatform1", passwordHistoryDTO1);
            var platformDTO2 = new PlatformDTO("TestPlatform2", passwordHistoryDTO2);

            platformDTO1 = _service.Add(platformDTO1);
            platformDTO2 = _service.Add(platformDTO2);

            // Act
            var resultPlatformDTO1 = _service.Get(platformDTO1!.Id);
            var resultPlatformDTO2 = _service.Get(p => p.Id == platformDTO2!.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultPlatformDTO1, Is.Not.Null);
                Assert.That(resultPlatformDTO2, Is.Not.Empty);
                Assert.That(resultPlatformDTO1, Is.EqualTo(platformDTO1));
                Assert.That(resultPlatformDTO2.First(), Is.EqualTo(platformDTO2));
            });
        }

        [Test]
        public void Get_InvalidPlatformDTO_ReturnsNullOrEmpty()
        {
            // Arrange
            var encryptionDTO1 = new EncryptionDTO("AES1");
            var keyDTO1 = new KeyDTO("TestKeyDTO1");
            var encryptionDTO2 = new EncryptionDTO("AES2");
            var keyDTO2 = new KeyDTO("TestKeyDTO2");
            var settingsDTO1 = new SettingsDTO(encryptionDTO1, keyDTO1);
            var settingsDTO2 = new SettingsDTO(encryptionDTO2, keyDTO2);
            var passwordHistoryDTO1 = new PasswordHistoryDTO("EncryptionTest1", 1, "SourceText1", DateTime.Now, settingsDTO1);
            var passwordHistoryDTO2 = new PasswordHistoryDTO("EncryptionTest2", 2, "SourceText2", DateTime.Now, settingsDTO2);
            var platformDTO1 = new PlatformDTO("TestPlatform1", passwordHistoryDTO1);
            var platformDTO2 = new PlatformDTO("TestPlatform2", passwordHistoryDTO2);

            platformDTO1 = _service.Add(platformDTO1);
            platformDTO2 = _service.Add(platformDTO2);

            // Act
            var result1 = _service.Get(platformDTO1!.Id + 2);
            var result2 = _service.Get(p => p.Id == (platformDTO2!.Id + 1));

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result1, Is.Null);
                Assert.That(result2, Is.Empty);
            });
        }

        [Test]
        public void Update_ValidPlatformDTO()
        {
            // Arrange
            var encryptionDTO = new EncryptionDTO("AES");
            var keyDTO = new KeyDTO("TestKeyDTO");
            var settingsDTO = new SettingsDTO(encryptionDTO, keyDTO);
            var passwordHistoryDTO = new PasswordHistoryDTO("EncryptionTest1", 1, "SourceText1", DateTime.Now, settingsDTO);
            var platformDTO = new PlatformDTO("PlatformTest", passwordHistoryDTO);
            platformDTO = _service.Add(platformDTO);
            _dbContext.PasswordHistories.Add(new PasswordHistory { EncryptedText = "EncryptedText" });

            // Act
            platformDTO!.Name = "UpdatedTestPlatformDTO";
            platformDTO.PasswordHistoryDTO = new PasswordHistoryDTO("NewPasswordHistoryDTO", 2);
            platformDTO.IconPath = "C:/Icons/icon.ico";
            _service.Update(platformDTO);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(platformDTO, Is.Not.Null);
                Assert.That(platformDTO.Name, Is.EqualTo("UpdatedTestPlatformDTO"));
                Assert.That(platformDTO.PasswordHistoryDTO.EncryptedText, Is.EqualTo("NewPasswordHistoryDTO"));
                Assert.That(platformDTO.IconPath, Is.EqualTo("C:/Icons/icon.ico"));
            });
        }

        [Test]
        public void Update_InvalidPlatformDTO_ThrowsException()
        {
            // Arrange
            var encryptionDTO = new EncryptionDTO("AES");
            var keyDTO = new KeyDTO("TestKeyDTO");
            var settingsDTO = new SettingsDTO(encryptionDTO, keyDTO);
            var passwordHistoryDTO = new PasswordHistoryDTO("EncryptionTest", 1, "SourceText", DateTime.Now, settingsDTO);
            var platformDTO = new PlatformDTO("PlatformTest", passwordHistoryDTO);
            platformDTO = _service.Add(platformDTO);

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _service.Update(new PlatformDTO("PlatformTest", passwordHistoryDTO, 3)), Throws.Exception);
                platformDTO!.Name = "ThisIsAPlatformDTOWithVeryLongNameValueThatExceedsTheMaximumLengthAllowedByTheDatabaseModelHaveAGoodDayNowSir";
                Assert.That(() => _service.Update(platformDTO), Throws.Exception);
            });
        }

        [Test]
        public void Delete_ValidPlatformDTO()
        {
            // Arrange
            var encryptionDTO = new EncryptionDTO("AES");
            var keyDTO = new KeyDTO("TestKeyDTO");
            var settingsDTO = new SettingsDTO(encryptionDTO, keyDTO);
            var passwordHistoryDTO = new PasswordHistoryDTO("EncryptionTest1", 1, "SourceText1", DateTime.Now, settingsDTO);
            var platformDTO = new PlatformDTO("TestPlatform", passwordHistoryDTO);

            platformDTO = _service.Add(platformDTO);

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _service.Delete(platformDTO!.Id), Throws.Nothing);
                var savedPasswordHistories = _dbContext.Platforms.ToList();
                Assert.That(savedPasswordHistories, Is.Empty);
            });
        }

        [Test]
        public void Delete_InvalidPlatformDTO_ThrowsException()
        {
            // Act and Assert
            Assert.That(() => _service.Delete(1), Throws.Exception);
        }
    }
}
