using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Focus.Models
{
    public class BaseModel
    {
        public string CreatedBy { get; set; } = Environment.UserName;
        public string? UpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; }
        public DateTime DateDeleted { get; set; }

        public int Id { get; set; }
    }
}
