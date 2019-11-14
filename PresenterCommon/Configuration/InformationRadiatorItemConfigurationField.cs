using System.Xml.Serialization;

namespace PresenterCommon.Configuration
{
    public class InformationRadiatorItemConfigurationField
    {
        [XmlAttribute]
        public string ID { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}
