using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresenterCommon.TestDoubles
{
    public class ITimerFake : ITimer
    {
        public event EventHandler Tick;

        public void OnTick(int times = 1)
        {
            var handler = Tick;
            if(handler != null)
            {
                for (int i = 0; i < times; i++)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }
    }
}
