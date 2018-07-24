namespace TuanZi.Entity
{
    public interface IInputDto<TKey>
    {
        TKey Id { get; set; }
    }


    public interface IOutputDto
    { }


    public interface IDataAuthEnabled
    {
        bool Updatable { get; set; }

        bool Deletable { get; set; }
    }
}