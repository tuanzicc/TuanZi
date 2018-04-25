namespace TuanZi.Audits
{
    public class AuditEntityProperty
    {
        public string Name { get; set; }

        public string FieldName { get; set; }

        public string OriginalValue { get; set; }

        public string NewValue { get; set; }

        public string DataType { get; set; }
    }
}