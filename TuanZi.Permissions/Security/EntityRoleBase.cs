

using System;
using System.ComponentModel.DataAnnotations.Schema;

using TuanZi.Entity;
using TuanZi.Filter;


namespace TuanZi.Security
{
    public abstract class EntityRoleBase<TRoleKey> : EntityBase<Guid>, ILockable, ICreatedTime
    {
        public TRoleKey RoleId { get; set; }

        public Guid EntityId { get; set; }

        public string FilterGroupJson { get; set; }

        [NotMapped]
        public FilterGroup FilgerGroup
        {
            get
            {
                if (FilterGroupJson.IsNullOrEmpty())
                {
                    return null;
                }
                return FilterGroupJson.FromJsonString<FilterGroup>();
            }
        }

        public bool IsLocked { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}