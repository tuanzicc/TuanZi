using System;


namespace TuanZi.Entity
{
    public abstract class ExpirableBase<TKey> : EntityBase<TKey>, IExpirable
        where TKey : IEquatable<TKey>
    {
        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }

        public bool IsTimeValid()
        {
            return !BeginTime.HasValue || !EndTime.HasValue || BeginTime.Value <= EndTime.Value;
        }

        public void ThrowIfTimeInvalid()
        {
            if (IsTimeValid())
            {
                return;
            }
            throw new IndexOutOfRangeException("BeginTime cannot be greater than EndTime");
        }
    }
}