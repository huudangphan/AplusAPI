using System;

namespace Apzon.PosmanErp.Commons.Attributes
{
    public class DescriptionAttribute:Attribute
    {
        public string Value { get; }

        public DescriptionAttribute(string value)
        {
            Value = value;
        }
    }
}