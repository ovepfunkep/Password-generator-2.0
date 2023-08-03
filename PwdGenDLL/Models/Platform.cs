using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace PwdGenDLL.Models
{
    public class Platform
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int PasswordHistoryId { get; set; }
        public string? IconPath { get; set; }

        // Relationships
        public PasswordHistory PasswordHistory { get; set;} = null!;
    }
}
