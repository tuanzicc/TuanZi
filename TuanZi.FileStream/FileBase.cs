using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TuanZi.Entity;

namespace TuanZi.Entity
{
    public class FileBase : EntityBase<Guid>, ICreatedTime
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(100)]
        public string ContentType { get; set; }
        public long ContentLength { get; set; }

        [StringLength(100)]
        public string Extension { get; set; }

        public byte[] Binary { get; set; }

        public DateTime CreatedTime { get; set; }

        public virtual Guid? AppId { get; set; }
        public virtual Guid? UserId { get; set; }

    }



}
