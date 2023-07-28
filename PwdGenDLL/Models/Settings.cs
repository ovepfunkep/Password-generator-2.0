using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace PwdGenDLL.Models
{
    public class Settings
    {
        public int Id { get; set; }
        public DateTime? DateModified { get; set; }
        public int EncryptionId { get; set; }
        public int KeyId { get; set; }

        // Relationships
        public Encryption Encryption { get; set; }
        public Key Key { get; set; }
    }
}
