using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PresenterCommon.Configuration
{
    public class InformationRadiatorConfiguration
    {
        public List<InformationRadiatorItem> Items { get; set; }

        public InformationRadiatorConfiguration()
        {
            Items = new List<InformationRadiatorItem>();
        }

        public void Save(string fileName)
        {
            var serializer = new XmlSerializer(this.GetType());
            using(var stream = new System.IO.FileStream(fileName, System.IO.FileMode.Create))
            {
                serializer.Serialize(stream, this);
            }
        }

        public static InformationRadiatorConfiguration Load(string fileName)
        {
            var serializer = new XmlSerializer(typeof(InformationRadiatorConfiguration));
            using(var stream = new System.IO.StreamReader(fileName))
            {
                return (InformationRadiatorConfiguration)serializer.Deserialize(stream);
            }
        }
    }
}
