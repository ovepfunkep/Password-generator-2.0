using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwdGenDLL.Models
{
    public class SettingsDTO
    {
        public int Id { get; set; }
        public DateTime? DateModified { get; set; }
        public EncryptionDTO EncryptionDTO { get; set; } = null!;
        public KeyDTO KeyDTO { get; set; } = null!;
    }
}
