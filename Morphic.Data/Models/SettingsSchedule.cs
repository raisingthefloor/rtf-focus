using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Data.Models
{
    public class Schedules
    {
        public Schedule schedule1 { get; set; } = new Schedule();
        public Schedule schedule2 { get; set; } = new Schedule();
        public Schedule schedule3 { get; set; } = new Schedule();
        public Schedule schedule4 { get; set; } = new Schedule();
        public Schedule schedule5 { get; set; } = new Schedule();
        public Schedulebreak scheduleBreak { get; set; } = new Schedulebreak();
    }

    public class Schedule
    {
        public string blockListName { get; set; } = string.Empty;
        public string startAt { get; set; } = string.Empty;
        public string endAt { get; set; } = string.Empty;
        public List<int> days { get; set; } = new List<int>();
        public bool isActive { get; set; }
    }

    public class Schedulebreak
    {
        public bool isActive { get; set; }
        public int breakDuration { get; set; } = 1;
        public int breakGap { get; set; } = 15;
    }
}
