using System;


namespace TuanZi.Entity
{
    public interface IEntity<out TKey>
    {
        TKey Id { get; }
    }
}