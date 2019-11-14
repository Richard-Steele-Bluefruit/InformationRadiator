using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PresenterCommon.ItemFactory
{
    public interface IItemFactory
    {
        object CreateObject(string itemType, params object[] specificParameters);
    }
}
