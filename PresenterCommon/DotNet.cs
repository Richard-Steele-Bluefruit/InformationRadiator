using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PresenterCommon
{
    public class DotNet
    {
        private static IDotNetWrapper _instance;

        public static IDotNetWrapper Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new DotNetImplementation();
                }
                return _instance;
            }
            internal set
            {
                _instance = value;
            }
        }

        private class DotNetImplementation : IDotNetWrapper
        {

            public DateTime Now
            {
                get { return DateTime.Now; }
            }
        }
    }
}
