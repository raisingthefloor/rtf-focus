using Morphic.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Focus
{
    internal class Helper
    {
        public static bool IsActiveToday(Schedule schedule)
        {
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return schedule.IsActiveSunday;
                case DayOfWeek.Monday:
                    return schedule.IsActiveMonday;
                case DayOfWeek.Tuesday:
                    return schedule.IsActiveTuesday;
                case DayOfWeek.Wednesday:
                    return schedule.IsActiveWednesday;
                case DayOfWeek.Thursday:
                    return schedule.IsActiveThursday;
                case DayOfWeek.Friday:
                    return schedule.IsActiveFriday;
                case DayOfWeek.Saturday:
                    return schedule.IsActiveSaturday;
            }
            return false;
        }
    }
}
