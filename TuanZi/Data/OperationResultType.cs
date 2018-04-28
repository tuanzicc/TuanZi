
using System.ComponentModel;


namespace TuanZi.Data
{
    public enum OperationResultType
    {
        [Description("Verification error")]
        ValidError,

        [Description("Query null")]
        QueryNull,

        [Description("No changes")]
        NoChanges,

        [Description("Success")]
        Success,

        [Description("Error")]
        Error
    }
}