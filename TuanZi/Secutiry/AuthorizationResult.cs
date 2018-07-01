using System.Diagnostics;
using TuanZi.Data;
using TuanZi.Extensions;

namespace TuanZi.Secutiry
{
    [DebuggerDisplay("{ResultType}-{Message}")]
    public sealed class AuthorizationResult : TuanResult<AuthorizationStatus>
    {
        public AuthorizationResult(AuthorizationStatus status)
            : this(status, null)
        { }

        public AuthorizationResult(AuthorizationStatus status, string message)
            : this(status, message, null)
        { }

        public AuthorizationResult(AuthorizationStatus status, string message, object data)
            : base(status, message, data)
        { }

        public override string Message
        {
            get { return _message ?? ResultType.ToDescription(); }
            set { _message = value; }
        }

        public bool IsOk
        {
            get { return ResultType == AuthorizationStatus.OK; }
        }

        public bool IsUnauthorized
        {
            get { return ResultType == AuthorizationStatus.Unauthorized; }
        }

        public bool IsForbidden
        {
            get { return ResultType == AuthorizationStatus.Forbidden; }
        }

        public bool IsNoFound
        {
            get { return ResultType == AuthorizationStatus.NoFound; }
        }

        public bool IsLocked
        {
            get { return ResultType == AuthorizationStatus.Locked; }
        }

        public bool IsError
        {
            get { return ResultType == AuthorizationStatus.Error; }
        }

        public static AuthorizationResult OK { get; } = new AuthorizationResult(AuthorizationStatus.OK);
    }
}