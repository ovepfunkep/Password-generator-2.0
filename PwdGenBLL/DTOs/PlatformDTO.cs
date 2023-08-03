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
    public class PlatformDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public PasswordHistoryDTO PasswordHistoryDTO { get; set; }
        public string? IconPath { get; set; }

        public PlatformDTO(string name, PasswordHistoryDTO passwordHistoryDTO, int id = 0, string? iconPath = null)
        {
            Id = id;
            Name = name;
            PasswordHistoryDTO = passwordHistoryDTO;
            IconPath = iconPath;
        }
    }
}
