using System;

using TuanZi.Data;
using TuanZi.Entity;
using TuanZi.Filter;
using TuanZi.Secutiry;

namespace TuanZi.Security
{
    public abstract class EntityRoleInputDtoBase<TRoleKey> : IInputDto<Guid>
    {
        protected EntityRoleInputDtoBase()
        {
            FilterGroup = new FilterGroup();
        }

        private Guid _id;
        public Guid Id
        {
            get { return _id; }
            set
            {
                if (value == Guid.Empty)
                {
                    value = CombGuid.NewGuid();
                }
                _id = value;
            }
        }

        public TRoleKey RoleId { get; set; }

        public Guid EntityId { get; set; }
        public DataAuthOperation Operation { get; set; }
        public FilterGroup FilterGroup { get; set; }

        public bool IsLocked { get; set; }
    }
}