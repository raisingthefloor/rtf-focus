
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Data.Models
{
    public class Session : BaseModel
    {
        public bool TurnONDND { get; set; }
        public bool ProvideBreak { get; set; }
        public int BreakDuration { get; set; }
        public int BreakGap { get; set; }
        public string BlockListName { get; set; }
        public virtual BlockList BlockList { get; set; }
        public DateTime ActualStartTime { get; set; }
        public DateTime ActualEndTime { get; set; }
        public Schedule Schedule { get; set; }
        public DateTime LastStartTime { get; set; }
        public DateTime LastBreakStartTime { get; set; }
        public String FocusType { get; set; }
        public int SessionDuration { get; set; }
    }
}
