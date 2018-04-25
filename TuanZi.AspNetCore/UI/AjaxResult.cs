namespace TuanZi.AspNetCore.UI
{
    public class AjaxResult
    {
        public AjaxResult()
            : this(null)
        { }

        public AjaxResult(string content, AjaxResultType type = AjaxResultType.Info, object data = null)
            : this(content, data, type)
        { }

        public AjaxResult(string content, object data, AjaxResultType type = AjaxResultType.Info)
        {
            Type = type.ToString();
            Content = content;
            Data = data;
        }

        public string Type { get; set; }

        public string Content { get; set; }

        public object Data { get; set; }
    }
}