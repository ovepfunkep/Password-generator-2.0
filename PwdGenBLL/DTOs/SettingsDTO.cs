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
        public EncryptionDTO EncryptionDTO { get; set; }
        public KeyDTO KeyDTO { get; set; }

        public SettingsDTO(EncryptionDTO encryptionDTO, KeyDTO keyDTO, DateTime? dateModified = null, int id = 0)
        {
            Id = id;
            DateModified = dateModified;
            EncryptionDTO = encryptionDTO;
            KeyDTO = keyDTO;
        }
    }
}
