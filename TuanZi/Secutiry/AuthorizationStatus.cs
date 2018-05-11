using System.ComponentModel;


namespace TuanZi.Secutiry
{
    public enum AuthorizationStatus
    {
        [Description("Permission checks passed")] OK = 200,

        [Description("This operation requires logging in to continue")] Unauthorized = 401,

        [Description("The current user has insufficient privileges and cannot continue")] Forbidden = 403,

        [Description("The specified function does not exist")] NoFound = 404,

        [Description("The specified function is locked")] Locked = 423,

        [Description("Permissions detection error")] Error = 500
    }
}