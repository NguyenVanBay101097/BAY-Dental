using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ApplicationCore.Models.ContentEditing
{
    class ProductCategoryBasic
    {
    }

    [XmlRoot(ElementName = "mxCell")]
    public class MxCell
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "parent")]
        public string Parent { get; set; }
        [XmlElement(ElementName = "mxGeometry")]
        public MxGeometry2 MxGeometry2 { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
        [XmlAttribute(AttributeName = "edge")]
        public string Edge { get; set; }
        [XmlAttribute(AttributeName = "source")]
        public string Source { get; set; }
        [XmlAttribute(AttributeName = "target")]
        public string Target { get; set; }
    }

    [XmlRoot(ElementName = "condition", Namespace = "http://www.w3.org/1999/xhtml")]
    public class Condition
    {
        [XmlAttribute(AttributeName = "namecondition")]
        public string Namecondition { get; set; }

        [XmlAttribute(AttributeName = "typecondition")]
        public string Typecondition { get; set; }

        [XmlAttribute(AttributeName = "flagcondition")]
        public string Flagcondition { get; set; }

        [XmlAttribute(AttributeName = "valuecondition")]
        public string Valuecondition { get; set; }
    }

    [XmlRoot(ElementName = "mxGeometry", Namespace = "http://www.w3.org/1999/xhtml")]
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
    }

    [XmlRoot(ElementName = "mxCell", Namespace = "http://www.w3.org/1999/xhtml")]
    public class MxCell2
    {
        [XmlElement(ElementName = "mxGeometry", Namespace = "http://www.w3.org/1999/xhtml")]
        public MxGeometry MxGeometry { get; set; }
        [XmlAttribute(AttributeName = "style")]
        public string Style { get; set; }
        [XmlAttribute(AttributeName = "vertex")]
        public string Vertex { get; set; }
        [XmlAttribute(AttributeName = "parent")]
        public string Parent { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "isRoot")]
        public bool isRoot { get; set; }
    }

    [XmlRoot(ElementName = "rule", Namespace = "http://www.w3.org/1999/xhtml")]
    public class Rule
    {
        [XmlElement(ElementName = "condition", Namespace = "http://www.w3.org/1999/xhtml")]
        public List<Condition> Condition { get; set; }

        [XmlAttribute(AttributeName = "logic")]
        public string Logic { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "mxCell", Namespace = "http://www.w3.org/1999/xhtml")]
        public MxCell2 MxCell2 { get; set; }

        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "sequences", Namespace = "http://www.w3.org/1999/xhtml")]
    public class Sequences
    {
        [XmlElement(ElementName = "mxCell", Namespace = "http://www.w3.org/1999/xhtml")]
        public MxCell2 MxCell2 { get; set; }
        [XmlAttribute(AttributeName = "tcarecampaignid")]
        public string Tcarecampaignid { get; set; }
        [XmlAttribute(AttributeName = "parentid")]
        public string Parentid { get; set; }
        [XmlAttribute(AttributeName = "messagereadid")]
        public string Messagereadid { get; set; }
        [XmlAttribute(AttributeName = "messageunreadid")]
        public string Messageunreadid { get; set; }
        [XmlAttribute(AttributeName = "channelsocialid")]
        public string Channelsocialid { get; set; }
        [XmlAttribute(AttributeName = "channeltype")]
        public string Channeltype { get; set; }
        [XmlAttribute(AttributeName = "content")]
        public string Content { get; set; }
        [XmlAttribute(AttributeName = "intervalnumber")]
        public string Intervalnumber { get; set; }
        [XmlAttribute(AttributeName = "intervaltype")]
        public string Intervaltype { get; set; }
        [XmlAttribute(AttributeName = "methodtype")]
        public string Methodtype { get; set; }
        [XmlAttribute(AttributeName = "sheduledate")]
        public string Sheduledate { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "mxGeometry")]
    public class MxGeometry2
    {
        [XmlAttribute(AttributeName = "relative")]
        public string Relative { get; set; }
        [XmlAttribute(AttributeName = "as")]
        public string As { get; set; }
    }

    [XmlRoot(ElementName = "message_read", Namespace = "http://www.w3.org/1999/xhtml")]
    public class Message_read
    {
        [XmlElement(ElementName = "mxCell", Namespace = "http://www.w3.org/1999/xhtml")]
        public MxCell2 MxCell2 { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "parent")]
        public string Parent { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "message_unread", Namespace = "http://www.w3.org/1999/xhtml")]
    public class Message_unread
    {
        [XmlElement(ElementName = "mxCell", Namespace = "http://www.w3.org/1999/xhtml")]
        public MxCell2 MxCell2 { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "parent")]
        public string Parent { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "root")]
    public class Root
    {
        [XmlElement(ElementName = "mxCell")]
        public List<MxCell> MxCell { get; set; }

        [XmlElement(ElementName = "rule", Namespace = "http://www.w3.org/1999/xhtml")]
        public Rule Rule { get; set; }

        [XmlElement(ElementName = "sequences", Namespace = "http://www.w3.org/1999/xhtml")]
        public Sequences Sequences { get; set; }

        [XmlElement(ElementName = "message_read", Namespace = "http://www.w3.org/1999/xhtml")]
        public Message_read Message_read { get; set; }

        [XmlElement(ElementName = "message_unread", Namespace = "http://www.w3.org/1999/xhtml")]
        public Message_unread Message_unread { get; set; }
    }

    [XmlRoot(ElementName = "mxGraphModel")]
    public class MxGraphModel
    {
        [XmlElement(ElementName = "root")]
        public Root Root { get; set; }
    }
}
