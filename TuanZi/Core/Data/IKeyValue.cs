namespace TuanZi.Core.Data
{
    public interface IKeyValue
    {
        string Key { get; set; }

        object Value { get; set; }
    }
}