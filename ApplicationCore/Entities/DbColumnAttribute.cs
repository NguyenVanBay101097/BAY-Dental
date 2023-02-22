using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public sealed class DbColumnAttribute : Attribute
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public DbColumnAttribute(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
