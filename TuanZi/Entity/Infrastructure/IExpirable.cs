using System;


namespace TuanZi.Entity
{
    public interface IExpirable
    {
        DateTime? BeginTime { get; set; }

        DateTime? EndTime { get; set; }
    }
}