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
        [XmlAttribute(AttributeName = "style")]
        public string Style { get; set; }
        [XmlAttribute(AttributeName = "vertex")]
        public string Vertex { get; set; }
        [XmlAttribute(AttributeName = "source")]
        public string Source { get; set; }
        [XmlAttribute(AttributeName = "target")]
        public string Target { get; set; }
        [XmlAttribute(AttributeName = "edge")]
        public string Edge { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
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

    [XmlRoot(ElementName = "sequence")]
    public class Sequence
    {
        [XmlElement(ElementName = "mxCell")]
        public MxCell MxCell { get; set; }

        [XmlAttribute(AttributeName = "channelSocialId")]
        public string ChannelSocialId { get; set; }

        [XmlAttribute(AttributeName = "content")]
        public string Content { get; set; }

        [XmlAttribute(AttributeName = "methodType")]
        public string MethodType { get; set; }

        [XmlAttribute(AttributeName = "campaignId")]
        public string CampaignId { get; set; }

        [XmlAttribute(AttributeName = "intervalNumber")]
        public string IntervalNumber { get; set; }

        [XmlAttribute(AttributeName = "intervalType")]
        public string IntervalType { get; set; }

        [XmlAttribute(AttributeName = "sheduleDate")]
        public string SheduleDate { get; set; }
        [XmlAttribute(AttributeName = "channelType")]
        public string ChannelType { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "tag")]
    public class Tag
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "addTag")]
    public class AddTag
    {
        [XmlElement(ElementName = "tag")]
        public List<Tag> Tag { get; set; }
        [XmlElement(ElementName = "mxCell")]
        public MxCell MxCell { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "messageOpen")]
    public class MessageOpen
    {
        [XmlElement(ElementName = "mxCell")]
        public MxCell MxCell { get; set; }
        [XmlAttribute(AttributeName = "label")]
        public string Label { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "messageDelivered")]
    public class MessageDelivered
    {
        [XmlElement(ElementName = "mxCell")]
        public MxCell MxCell { get; set; }
        [XmlAttribute(AttributeName = "label")]
        public string Label { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "condition")]
    public class Condition
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "op")]
        public string Op { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
        [XmlAttribute(AttributeName = "displayValue")]
        public string DisplayValue { get; set; }
    }

    [XmlRoot(ElementName = "rule")]
    public class Rule
    {
        [XmlElement(ElementName = "condition")]
        public List<Condition> Condition { get; set; }
        [XmlElement(ElementName = "mxCell")]
        public MxCell MxCell { get; set; }
        [XmlAttribute(AttributeName = "logic")]
        public string Logic { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "root")]
    public class Root
    {
        [XmlElement(ElementName = "mxCell")]
        public List<MxCell> MxCell { get; set; }

        [XmlElement(ElementName = "sequence")]
        public Sequence Sequence { get; set; }

        [XmlElement(ElementName = "addTag")]
        public List<AddTag> AddTag { get; set; }

        [XmlElement(ElementName = "messageOpen")]
        public MessageOpen MessageOpen { get; set; }

        [XmlElement(ElementName = "messageDelivered")]
        public MessageDelivered MessageDelivered { get; set; }

       

        [XmlElement(ElementName = "rule")]
        public Rule Rule { get; set; }
    }

    [XmlRoot(ElementName = "mxGraphModel")]
    public class MxGraphModel
    {
        [XmlElement(ElementName = "root")]
        public Root Root { get; set; }
    }
}
