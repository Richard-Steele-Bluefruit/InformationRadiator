using System.Collections.Generic;
using System.Xml.Serialization;

namespace PresenterCommon.Configuration
{
    public class InformationRadiatorItem
    {
        public InformationRadiatorItem()
        {
            Configuration = new InformationRadiatorItemConfiguration();
            Width = 300;
            Height = 300;
        }

        [XmlAttribute]
        public string ItemType { get; set; }

        [XmlAttribute]
        public int Width { get; set; }

        [XmlAttribute]
        public int Height { get; set; }

        [XmlAttribute]
        public string Left { get; set; }

        [XmlAttribute]
        public string Top { get; set; }

        [XmlAttribute]
        public string Screen { get; set; }

        public string Title { get; set; }

        [XmlArrayItem(ElementName = "Field")]
        public InformationRadiatorItemConfiguration Configuration { get; set; }
    }
}
