using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PresenterCommon
{
    public class PresenterCommonFactory
    {
        private static PresenterCommonFactory _instance;

        public static PresenterCommonFactory Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PresenterCommonFactory();
                return _instance;
            }
            internal set
            {
                _instance = value;
            }
        }

        public virtual ITimer CreateTimer(double interval)
        {
            return new DotNetTimer(interval);
        }
    }
}
