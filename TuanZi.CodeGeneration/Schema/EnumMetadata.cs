using System;

using TuanZi.Extensions;


namespace TuanZi.CodeGeneration.Schema
{
    public class EnumMetadata
    {
        public EnumMetadata()
        { }

        public EnumMetadata(Enum enumItem)
        {
            if (enumItem == null)
            {
                return;
            }
            Value = enumItem.CastTo<int>();
            Name = enumItem.ToString();
            Display = enumItem.ToDescription();
        }

        public int Value { get; set; }

        public string Name { get; set; }

        public string Display { get; set; }
    }
}