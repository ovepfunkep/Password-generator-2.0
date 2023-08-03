using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PwdGenDLL.Models;

namespace PwdGenDLL.Models
{
    public class PasswordHistoryDTO
    {
        public int Id { get; set; }
        public string? SourceText { get; set; }
        public string EncryptedText { get; set; }
        public DateTime? Date { get; set; }
        public SettingsDTO? SettingsDTO { get; set; }

        public PasswordHistoryDTO(string encryptedText, int id = 0, string? sourceText = null, DateTime? date = null, SettingsDTO? settingsDTO = null)
        {
            Id = id;
            SourceText = sourceText;
            EncryptedText = encryptedText;
            Date = date;
            SettingsDTO = settingsDTO;
        }
    }
}
