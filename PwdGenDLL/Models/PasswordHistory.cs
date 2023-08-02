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
    public class PasswordHistory
    {
        public int Id { get; set; }
        public string? SourceText { get; set; }
        public string EncryptedText { get; set; } = null!;
        public DateTime? Date { get; set; }
        public int? SettingsId { get; set; }

        // Relationships
        public Settings? Settings { get; set; }
    }
}
