using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using TuanZi.Collections;
using TuanZi.Entity;
using TuanZi.Extensions;

namespace TuanZi.Security
{
    public abstract class ModuleBase<TModuleKey> : EntityBase<TModuleKey>
        where TModuleKey : struct, IEquatable<TModuleKey>
    {
        [Required, DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Remark")]
        public string Remark { get; set; }

        [Required]
        public string Code { get; set; }

        [DisplayName("Order Code")]
        public double OrderCode { get; set; }

        [DisplayName("Tree Path String")]
        public string TreePathString { get; set; }

        [NotMapped]
        public TModuleKey[] TreePathIds
        {
            get
            {
                return TreePathString?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(m => m.Trim('$').CastTo<TModuleKey>()).ToArray() ?? new TModuleKey[0];
            }
        }

        [DisplayName("Parent ID")]
        public TModuleKey? ParentId { get; set; }
    }
}