using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using PwdGenBLL.Converters;
using PwdGenBLL.Services;

using PwdGenDLL;
using PwdGenDLL.Models;
using PwdGenDLL.Repositories.Implementations;

namespace PwdGenTests.BLL.Services
{
    public class PasswordHistoryServiceTests
    {
        private AppDbContext _dbContext;
        private PasswordHistoryService _service;
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

            PasswordHistoryRepository repository = new(_dbContext);
            PasswordHistoryConverter converter = new(new SettingsConverter(new EncryptionConverter(), new KeyConverter()));
            _service = new PasswordHistoryService(repository, converter);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
            _connection.Close();
        }

        [Test]
        public void Add_ValidPasswordHistoryDTO()
        {
            // Arrange
            var encryptionDTO = new EncryptionDTO("EncryptionTest");
            var keyDTO = new KeyDTO("KeyTest");
            var settingsDTO = new SettingsDTO(encryptionDTO, keyDTO);
            var passwordHistoryDTO = new PasswordHistoryDTO("EncryptedText", 1, "SourceText", DateTime.Now, settingsDTO);

            // Act
            passwordHistoryDTO = _service.Add(passwordHistoryDTO);

            // Assert
            var result = _service.Get(passwordHistoryDTO!.Id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(passwordHistoryDTO));
        }

        [Test]
        public void Add_InvalidPasswordHistoryDTO()
        {
            // Arrange
            var longSourceTextPasswordHistoryDTO = new PasswordHistoryDTO("EncryptedText", sourceText: "ThisIsAPasswordHistoryDTOWithVeryLongSourceTextValueThatExceedsTheMaximumLengthAllowedByTheDatabaseModelHaveAGoodDayNowSir");

            // Act and Assert
            Assert.That(() => _service.Add(longSourceTextPasswordHistoryDTO), Throws.Exception);
        }

        [Test]
        public void Get_ReturnsAllPasswordHistories()
        {
            // Arrange
            var encryptionDTO = new EncryptionDTO("EncryptionTest");
            var keyDTO = new KeyDTO("KeyTest");
            var settingsDTO = new SettingsDTO(encryptionDTO, keyDTO);
            var passwordHistoryDTO1 = new PasswordHistoryDTO("EncryptedText1", 1, "SourceText1", DateTime.Now, settingsDTO);
            var passwordHistoryDTO2 = new PasswordHistoryDTO("EncryptedText2", 2, "SourceText2", DateTime.Now);
            passwordHistoryDTO1 = _service.Add(passwordHistoryDTO1);
            passwordHistoryDTO2 = _service.Add(passwordHistoryDTO2);

            // Act
            var passwordHistories = _service.Get().ToList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(passwordHistories, Has.Count.EqualTo(2));
                Assert.That(passwordHistories, Contains.Item(passwordHistoryDTO1));
                Assert.That(passwordHistories, Contains.Item(passwordHistoryDTO2));
                Assert.That(passwordHistoryDTO2!.SettingsDTO, Is.Null);
            });
        }

        [Test]
        public void Get_ValidPasswordHistoryDTO_ReturnsPasswordHistoryDTO()
        {
            // Arrange
            var encryptionDTO1 = new EncryptionDTO("EncryptionTest1");
            var keyDTO1 = new KeyDTO("KeyTest1");
            var settingsDTO1 = new SettingsDTO(encryptionDTO1, keyDTO1);
            var encryptionDTO2 = new EncryptionDTO("EncryptionTest2");
            var keyDTO2 = new KeyDTO("KeyTest2");
            var settingsDTO2 = new SettingsDTO(encryptionDTO2, keyDTO2);
            var passwordHistoryDTO1 = new PasswordHistoryDTO("EncryptedText1", 1, "SourceText1", DateTime.Now, settingsDTO1);
            var passwordHistoryDTO2 = new PasswordHistoryDTO("EncryptedText2", 2, "SourceText2", DateTime.Now, settingsDTO2);
            passwordHistoryDTO1 = _service.Add(passwordHistoryDTO1);
            passwordHistoryDTO2 = _service.Add(passwordHistoryDTO2);

            // Act
            var resultPasswordHistoryDTO1 = _service.Get(passwordHistoryDTO1!.Id);
            var resultPasswordHistoryDTO2 = _service.Get(p => p.Id == passwordHistoryDTO2!.Id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultPasswordHistoryDTO1, Is.Not.Null);
                Assert.That(resultPasswordHistoryDTO2, Is.Not.Empty);
                Assert.That(resultPasswordHistoryDTO1, Is.EqualTo(passwordHistoryDTO1));
                Assert.That(resultPasswordHistoryDTO2.First(), Is.EqualTo(passwordHistoryDTO2));
            });
        }

        [Test]
        public void Get_InvalidPasswordHistoryDTO_ReturnsNullOrEmpty()
        {
            // Arrange
            var encryptionDTO1 = new EncryptionDTO("EncryptionTest1");
            var keyDTO1 = new KeyDTO("KeyTest1");
            var settingsDTO1 = new SettingsDTO(encryptionDTO1, keyDTO1);
            var encryptionDTO2 = new EncryptionDTO("EncryptionTest2");
            var keyDTO2 = new KeyDTO("KeyTest2");
            var settingsDTO2 = new SettingsDTO(encryptionDTO2, keyDTO2);
            var passwordHistoryDTO1 = new PasswordHistoryDTO("EncryptedText1", 1, "SourceText1", DateTime.Now, settingsDTO1);
            var passwordHistoryDTO2 = new PasswordHistoryDTO("EncryptedText2", 2, "SourceText2", DateTime.Now, settingsDTO2);
            passwordHistoryDTO1 = _service.Add(passwordHistoryDTO1);
            passwordHistoryDTO2 = _service.Add(passwordHistoryDTO2);

            // Act
            var result1 = _service.Get(passwordHistoryDTO1!.Id + 2);
            var result2 = _service.Get(p => p.Id == (passwordHistoryDTO2!.Id + 1));

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result1, Is.Null);
                Assert.That(result2, Is.Empty);
            });
        }

        [Test]
        public void Update_ValidPasswordHistoryDTO()
        {
            // Arrange
            var encryptionDTO = new EncryptionDTO("EncryptionTest");
            var keyDTO = new KeyDTO("KeyTest");
            var settingsDTO = new SettingsDTO(encryptionDTO, keyDTO);
            var passwordHistoryDTO = new PasswordHistoryDTO("EncryptedText", 1, "SourceText", DateTime.Now, settingsDTO);
            passwordHistoryDTO = _service.Add(passwordHistoryDTO);

            // Act
            passwordHistoryDTO!.SourceText = "UpdatedSourceText";
            passwordHistoryDTO.EncryptedText = "UpdatedEncryptedText";
            passwordHistoryDTO.Date = DateTime.Now.AddDays(1);
            _service.Update(passwordHistoryDTO);

            // Assert
            var updatedPasswordHistoryDTO = _service.Get(passwordHistoryDTO.Id);
            Assert.Multiple(() =>
            {
                Assert.That(updatedPasswordHistoryDTO, Is.Not.Null);
                Assert.That(updatedPasswordHistoryDTO?.SourceText, Is.EqualTo("UpdatedSourceText"));
                Assert.That(updatedPasswordHistoryDTO?.EncryptedText, Is.EqualTo("UpdatedEncryptedText"));
                Assert.That(updatedPasswordHistoryDTO?.Date, Is.EqualTo(passwordHistoryDTO.Date));
            });
        }

        [Test]
        public void Update_InvalidPasswordHistoryDTO_ThrowsException()
        {
            // Arrange
            var encryptionDTO = new EncryptionDTO("EncryptionTest");
            var keyDTO = new KeyDTO("KeyTest");
            var settingsDTO = new SettingsDTO(encryptionDTO, keyDTO);
            var passwordHistoryDTO = new PasswordHistoryDTO("EncryptedText", 1, "SourceText", DateTime.Now, settingsDTO);
            passwordHistoryDTO = _service.Add(passwordHistoryDTO);

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _service.Update(new PasswordHistoryDTO("EncryptedText", 2)), Throws.Exception);
                passwordHistoryDTO!.SourceText = "ThisIsAPasswordHistoryDTOWithVeryLongEncryptedTextValueThatExceedsTheMaximumLengthAllowedByTheDatabaseModelHaveAGoodDayNowSir";
                Assert.That(() => _service.Update(passwordHistoryDTO), Throws.Exception);
            });
        }

        [Test]
        public void Delete_ValidPasswordHistoryDTO()
        {
            // Arrange
            var encryptionDTO = new EncryptionDTO("EncryptionTest");
            var keyDTO = new KeyDTO("KeyTest");
            var settingsDTO = new SettingsDTO(encryptionDTO, keyDTO);
            var passwordHistoryDTO1 = new PasswordHistoryDTO("EncryptedText1", 1, "SourceText1", DateTime.Now, settingsDTO);
            passwordHistoryDTO1 = _service.Add(passwordHistoryDTO1);

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _service.Delete(passwordHistoryDTO1!.Id), Throws.Nothing);
                var savedPasswordHistories = _dbContext.PasswordHistories.ToList();
                Assert.That(savedPasswordHistories, Is.Empty);
            });
        }

        [Test]
        public void Delete_InvalidPasswordHistoryDTO_ThrowsException()
        {
            // Act and Assert
            Assert.That(() => _service.Delete(1), Throws.Exception);
        }
    }
}
