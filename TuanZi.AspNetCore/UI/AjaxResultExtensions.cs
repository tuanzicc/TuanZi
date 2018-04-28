using TuanZi.Data;


namespace TuanZi.AspNetCore.UI
{
    public static class AjaxResultExtensions
    {
        public static AjaxResult ToAjaxResult<T>(this OperationResult<T> result)
        {
            string content = result.Message ?? result.ResultType.ToDescription();
            AjaxResultType type = result.ResultType.ToAjaxResultType();
            return new AjaxResult(content, type, result.Data);
        }

        public static AjaxResult ToAjaxResult(this OperationResult result)
        {
            string content = result.Message ?? result.ResultType.ToDescription();
            AjaxResultType type = result.ResultType.ToAjaxResultType();
            return new AjaxResult(content, type);
        }

        public static AjaxResultType ToAjaxResultType(this OperationResultType resultType)
        {
            switch (resultType)
            {
                case OperationResultType.Success:
                    return AjaxResultType.Success;
                case OperationResultType.NoChanges:
                    return AjaxResultType.Info;
                default:
                    return AjaxResultType.Error;
            }
        }

        public static bool IsError(this OperationResultType resultType)
        {
            return resultType == OperationResultType.QueryNull
                || resultType == OperationResultType.ValidError
                || resultType == OperationResultType.Error;
        }
    }
}