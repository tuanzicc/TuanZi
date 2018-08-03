namespace TuanZi.Audits
{
    public class AuditPropertyEntry
    {
        public string DisplayName { get; set; }

        public string FieldName { get; set; }

        public string OriginalValue { get; set; }

        public string NewValue { get; set; }

        public string DataType { get; set; }
    }
}