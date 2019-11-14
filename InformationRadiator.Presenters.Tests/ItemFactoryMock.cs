using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationRadiatorPresentersTests
{
    class ItemFactoryMock : PresenterCommon.ItemFactory.IItemFactory
    {
        public List<string> requestedItemType = new List<string>();
        public List<object[]> requestedSpecificParameters = new List<object[]>();
        public List<object> returnObjects = new List<object>();

        public object CreateObject(string itemType, params object[] specificParameters)
        {
            requestedItemType.Add((string)itemType.Clone());
            requestedSpecificParameters.Add(specificParameters);

            object result = returnObjects[0];
            returnObjects.RemoveAt(0);
            return result;
        }
    }
}
