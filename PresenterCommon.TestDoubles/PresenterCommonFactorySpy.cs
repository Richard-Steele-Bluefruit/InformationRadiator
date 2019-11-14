using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresenterCommon.TestDoubles
{
    public class PresenterCommonFactorySpy : PresenterCommonFactory
    {
        public ITimer createTimerReturn;
        public int createTimerCallCount = 0;
        public double createTimerInterval;
        public override ITimer CreateTimer(double interval)
        {
            createTimerInterval = interval;
            createTimerCallCount++;
            var returnValue = createTimerReturn;
            createTimerReturn = null;
            return returnValue;
        }

        public void InstallSpy()
        {
            PresenterCommonFactory.Instance = this;
        }

        public static void RemoveSpy()
        {
            PresenterCommonFactory.Instance = null;
        }
    }
}
