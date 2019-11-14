using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Model
{
    public interface ISprintDays
    {
        DateTime StartDate { get; set; }
        DateTime CurrentDate { get; set; }
        int SprintDay { get; }
        int DaysInSprint { get; set; }
    }
}
