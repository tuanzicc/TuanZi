using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using TuanZi.Entity;


namespace TuanZi.Security
{
    public abstract class ModuleInputDtoBase<TModuleKey> : IInputDto<TModuleKey>
        where TModuleKey : struct, IEquatable<TModuleKey>
    {
        public TModuleKey Id { get; set; }

        [Required, DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Remark")]
        public string Remark { get; set; }

        [Required]
        public string Code { get; set; }

        [DisplayName("Order Code")]
        public double OrderCode { get; set; }

        public TModuleKey? ParentId { get; set; }
    }
}