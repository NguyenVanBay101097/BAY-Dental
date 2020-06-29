using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Umbraco.Web.Models.ContentEditing
{
    [XmlRoot(ElementName = "field")]
    public class Field
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlText]
        public string Text { get; set; }
        [XmlAttribute(AttributeName = "ref")]
        public string Ref { get; set; }
    }

    [XmlRoot(ElementName = "record")]
    public class Record
    {
        [XmlElement(ElementName = "field")]
        public List<Field> Field { get; set; } = new List<Field>();
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "model")]
        public string Model { get; set; }
    }

    [XmlRoot(ElementName = "data")]
    public class Data
    {
        [XmlElement(ElementName = "record")]
        public List<Record> Record { get; set; } = new List<Record>();
    }

    [XmlRoot(ElementName = "tdental")]
    public class Tdental
    {
        [XmlElement(ElementName = "data")]
        public Data Data { get; set; }
    }
}

