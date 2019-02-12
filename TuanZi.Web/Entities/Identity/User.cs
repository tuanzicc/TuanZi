using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TuanZi.Entity;
using TuanZi.Identity;

namespace FuqLink.Entities
{
    public class User : UserBase<Guid>
    {

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }
        
        public Guid? Photo { get; set; }

        public UserStatus Status { get; set; }


        public virtual App App { get; set; }
    }

    public enum UserStatus
    {
        [Description("Active")] Active = 0,
        [Description("Inactive")] Inactive = 1
    }
}
