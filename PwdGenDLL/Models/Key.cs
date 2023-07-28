using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace PwdGenDLL.Models
{
    public class Key
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }
}
