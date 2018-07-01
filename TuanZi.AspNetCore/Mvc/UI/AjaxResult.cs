namespace TuanZi.AspNetCore.UI
{
    public class AjaxResult
    {
        public AjaxResult()
            : this(null)
        { }

        public AjaxResult(string content, AjaxResultType type = AjaxResultType.Success, object data = null)
            : this(content, data, type)
        { }

        public AjaxResult(string content, object data, AjaxResultType type = AjaxResultType.Success)
        {
            Type = type;
            Content = content;
            Data = data;
        }

        public AjaxResultType Type { get; set; }

        public string Content { get; set; }

        public object Data { get; set; }

        public bool Successed()
        {
            return Type == AjaxResultType.Success;
        }

        public bool Error()
        {
            return Type == AjaxResultType.Error;
        }

        public static AjaxResult Success(object data)
        {
            return new AjaxResult("Success", AjaxResultType.Success, data);
        }
    }
}