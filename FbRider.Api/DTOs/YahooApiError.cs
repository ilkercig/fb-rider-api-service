using System.Xml.Serialization;

namespace FbRider.Api.DTOs
{
    [XmlRoot("error", Namespace = "http://www.yahooapis.com/v1/base.rng")]
    public class YahooApiError
    {
        [XmlAttribute("lang", Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string? Language { get; set; }

        [XmlAttribute("uri", Namespace = "http://www.yahooapis.com/v1/base.rng")]
        public string? Uri { get; set; }

        [XmlElement("description")]
        public string? Description { get; set; }

        [XmlElement("detail")]
        public string? Detail { get; set; }
    }

}
