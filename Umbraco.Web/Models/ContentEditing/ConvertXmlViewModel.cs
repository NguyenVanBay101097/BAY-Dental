using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Umbraco.Web.Models.ContentEditing
{
    

    [XmlRoot(ElementName = "mxCell")]
    public class MxCell
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "parent")]
        public string Parent { get; set; }
        [XmlElement(ElementName = "mxGeometry")]
        public MxGeometry MxGeometry { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
        [XmlAttribute(AttributeName = "style")]
        public string Style { get; set; }
        [XmlAttribute(AttributeName = "vertex")]
        public string Vertex { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "beforeDays")]
        public string BeforeDays { get; set; }
        [XmlElement(ElementName = "sheduleDate")]
        public DateTime SheduleDate { get; set; }
        [XmlAttribute(AttributeName = "tCareCampaignId")]
        public string TCareCampaignId { get; set; }
        [XmlAttribute(AttributeName = "methodType")]
        public string MethodType { get; set; }
        [XmlAttribute(AttributeName = "intervalType")]
        public string IntervalType { get; set; }
        [XmlAttribute(AttributeName = "intervalNumber")]
        public string IntervalNumber { get; set; }
        [XmlAttribute(AttributeName = "content")]
        public string Content { get; set; }
        [XmlAttribute(AttributeName = "channelSocialId")]
        public string ChannelSocialId { get; set; }
        [XmlAttribute(AttributeName = "channelType")]
        public string ChannelType { get; set; }
        [XmlAttribute(AttributeName = "source")]
        public string Source { get; set; }
        [XmlAttribute(AttributeName = "target")]
        public string Target { get; set; }
        [XmlAttribute(AttributeName = "edge")]
        public string Edge { get; set; }
    }

    [XmlRoot(ElementName = "mxGeometry")]
    public class MxGeometry
    {
        [XmlAttribute(AttributeName = "x")]
        public string X { get; set; }
        [XmlAttribute(AttributeName = "y")]
        public string Y { get; set; }
        [XmlAttribute(AttributeName = "width")]
        public string Width { get; set; }
        [XmlAttribute(AttributeName = "height")]
        public string Height { get; set; }
        [XmlAttribute(AttributeName = "as")]
        public string As { get; set; }
        [XmlAttribute(AttributeName = "relative")]
        public string Relative { get; set; }
    }

    [XmlRoot(ElementName = "Date")]
    public class Date
    {
        [XmlAttribute(AttributeName = "as")]
        public string As { get; set; }
    }

    [XmlRoot(ElementName = "root")]
    public class Root
    {
        [XmlElement(ElementName = "mxCell")]
        public List<MxCell> MxCell { get; set; }
    }

    [XmlRoot(ElementName = "mxGraphModel")]
    public class MxGraphModel
    {
        [XmlElement(ElementName = "root")]
        public Root Root { get; set; }
    }
}
