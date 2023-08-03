﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwdGenDLL.Models
{
    public class EncryptionDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Link { get; set; }

        public EncryptionDTO(string name, int id = 0, string? description = null, string? link = null)
        {
            Id = id;
            Name = name;
            Description = description;
            Link = link;
        }
    }
}
