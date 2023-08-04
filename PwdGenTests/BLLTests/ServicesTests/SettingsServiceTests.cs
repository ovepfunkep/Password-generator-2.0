using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using PwdGenBLL.Converters;
using PwdGenBLL.Services;

using PwdGenDLL;
using PwdGenDLL.Models;
using PwdGenDLL.Repositories.Implementations;

namespace PwdGenTests.BLL.Services
{
    public class SettingsServiceTests
    {
        private AppDbContext _dbContext;
        private SettingsService _service;
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

            SettingsRepository repository = new(_dbContext);
            SettingsConverter converter = new(new EncryptionConverter(), new KeyConverter());
            _service = new SettingsService(repository, converter);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
            _connection.Close();
        }

        [Test]
        public void Add_ValidSettingsDTO()
        {
            // Arrange
            var encryptionDTO = new EncryptionDTO("TestEncryption");
            var keyDTO = new KeyDTO("TestKey");
            var settings = new SettingsDTO(encryptionDTO, keyDTO);

            // Act
            settings = _service.Add(settings);

            // Assert
            var result = _service.Get(settings!.Id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(settings));
        }

        [Test]
        public void Add_InvalidSettingsDTO_ThrowsException()
        {
            // Arrange
            var encryptionDTO = new EncryptionDTO("TestEncryption");
            var keyDTO = new KeyDTO("TestKey");
            var sameSettingsDTO1 = new SettingsDTO(encryptionDTO, keyDTO);
            var sameSettingsDTO2 = new SettingsDTO(encryptionDTO, keyDTO);
            sameSettingsDTO1 = _service.Add(sameSettingsDTO1);

            // Act and Assert
            Assert.That(() => _service.Add(sameSettingsDTO2), Throws.Exception);
        }

        [Test]
        public void Get_ReturnsAllSettingsDTO()
        {
            // Arrange
            var encryptionDTO1 = new EncryptionDTO("AES1");
            var encryptionDTO2 = new EncryptionDTO("AES2");
            var keyDTO1 = new KeyDTO("TestKeyDTO1");
            var keyDTO2 = new KeyDTO("TestKeyDTO2");
            var settings1 = new SettingsDTO(encryptionDTO1, keyDTO1);
            var settings2 = new SettingsDTO(encryptionDTO2, keyDTO2);
            settings1 = _service.Add(settings1);
            settings2 = _service.Add(settings2);

            // Act
            var settings = _service.Get().ToList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(settings, Has.Count.EqualTo(2));
                Assert.That(settings, Contains.Item(settings1));
                Assert.That(settings, Contains.Item(settings2));
                Assert.That(!string.IsNullOrEmpty(settings.FirstOrDefault(s => s.Id == 1)?.KeyDTO.Value));
                Assert.That(!string.IsNullOrEmpty(settings.FirstOrDefault(s => s.Id == 2)?.KeyDTO.Value));
            });
        }

        [Test]
        public void Get_ValidSettingsDTO_ReturnsSettingsDTO()
        {
            // Arrange
            var encryptionDTO1 = new EncryptionDTO("AES1");
            var encryptionDTO2 = new EncryptionDTO("AES2");
            var keyDTO1 = new KeyDTO("TestKeyDTO1");
            var keyDTO2 = new KeyDTO("TestKeyDTO2");
            var settings1 = new SettingsDTO(encryptionDTO1, keyDTO1);
            var settings2 = new SettingsDTO(encryptionDTO2, keyDTO2);
            settings1 = _service.Add(settings1);
            settings2 = _service.Add(settings2);

            // Act
            var resultSettingsDTO = _service.Get().ToList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultSettingsDTO.FirstOrDefault(s => s.Id == 1), Is.Not.Null);
                Assert.That(resultSettingsDTO.FirstOrDefault(s => s.Id == 2), Is.Not.Null);
                Assert.That(resultSettingsDTO.FirstOrDefault(s => s.Id == 1), Is.EqualTo(settings1));
                Assert.That(resultSettingsDTO.FirstOrDefault(s => s.Id == 2), Is.EqualTo(settings2));
                Assert.That(resultSettingsDTO.FirstOrDefault(s => s.Id == 1)?.EncryptionDTO.Name, Is.Not.Null);
                Assert.That(resultSettingsDTO.FirstOrDefault(s => s.Id == 2)?.KeyDTO.Value, Is.Not.Null);
            });
        }

        [Test]
        public void Get_InvalidSettingsDTO_ReturnsNullOrEmpty()
        {
            // Arrange
            var encryptionDTO = new EncryptionDTO("AES");
            var keyDTO = new KeyDTO("TestKeyDTO");
            var settings = new SettingsDTO(encryptionDTO, keyDTO);
            settings = _service.Add(settings);

            // Act
            var result1 = _service.Get(settings!.Id + 1);
            var result2 = _service.Get(s => s.Id == settings.Id + 1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result1, Is.Null);
                Assert.That(result2, Is.Empty);
            });
        }

        [Test]
        public void Update_ValidSettingsDTO()
        {
            // Arrange
            var encryptionDTO1 = new EncryptionDTO("AES1");
            var keyDTO1 = new KeyDTO("TestKeyDTO1");
            var settings = new SettingsDTO(encryptionDTO1, keyDTO1);
            settings = _service.Add(settings);
            _dbContext.Add(new Key() { Value = "TestKey" });
            _dbContext.Add(new Encryption() { Name = "EncryptionTest" });

            // Act
            settings!.EncryptionDTO.Id = 2;
            settings!.KeyDTO.Id = 2;
            _service.Update(settings);

            // Assert
            var updatedSettingsDTO = _service.Get(settings.Id);
            Assert.Multiple(() =>
            {
                Assert.That(updatedSettingsDTO, Is.Not.Null);
                Assert.That(updatedSettingsDTO?.EncryptionDTO.Id, Is.EqualTo(2));
                Assert.That(updatedSettingsDTO?.KeyDTO.Id, Is.EqualTo(2));
            });
        }

        [Test]
        public void Update_InvalidSettingsDTO_ThrowsException()
        {
            // Arrange
            var encryptionDTO1 = new EncryptionDTO("AES1");
            var encryptionDTO2 = new EncryptionDTO("AES2");
            var keyDTO1 = new KeyDTO("TestKeyDTO1");
            var keyDTO2 = new KeyDTO("TestKeyDTO2");
            var settings1 = new SettingsDTO(encryptionDTO1, keyDTO1);
            var settings2 = new SettingsDTO(encryptionDTO2, keyDTO2);
            settings1 = _service.Add(settings1);
            settings2 = _service.Add(settings2);

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _service.Update(new SettingsDTO(encryptionDTO1, keyDTO2, id: 3)), Throws.Exception);
                settings1!.EncryptionDTO = settings2!.EncryptionDTO;
                settings1.KeyDTO = settings2.KeyDTO;
                Assert.That(() => _service.Update(settings1), Throws.Exception);
            });
        }

        [Test]
        public void Delete_ValidSettingsDTO()
        {
            // Arrange
            var encryptionDTO = new EncryptionDTO("AES");
            var keyDTO = new KeyDTO("TestKeyDTO");
            var settings = new SettingsDTO(encryptionDTO, keyDTO);
            settings = _service.Add(settings);

            // Act and Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _service.Delete(settings!.Id), Throws.Nothing);
                var savedSettingsDTO = _dbContext.Settings.ToList();
                Assert.That(savedSettingsDTO, Is.Empty);
            });
        }

        [Test]
        public void Delete_InvalidSettingsDTO_ThrowsException()
        {
            // Act and Assert
            Assert.That(() => _service.Delete(1), Throws.Exception);
        }
    }
}
