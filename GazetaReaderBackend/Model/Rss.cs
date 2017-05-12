using System.Collections.Generic;
using System.Xml.Serialization;

namespace GazetaReaderFrontend.Model
{
    [XmlRoot(ElementName = "rss")]
    public class Rss
    {
        [XmlElement(ElementName = "channel")]
        public Channel Channel { get; set; }

    }
    public class Channel
    {
        [XmlElement(ElementName = "item")]
        public List<Item> Items { get; set; }
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "description")]
        public string Description { get; set; }

    }
    public class Item
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "link")]
        public string Link { get; set; }
        [XmlElement(ElementName = "pubDate")]
        public string PubDate { get; set; }
        [XmlElement(ElementName = "description")]
        public string Description { get; set; }

        
    }
    
}