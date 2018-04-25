namespace TuanZi.Entity
{
    public interface IInputDto<TKey>
    {
        TKey Id { get; set; }
    }


    public interface IOutputDto
    { }
}