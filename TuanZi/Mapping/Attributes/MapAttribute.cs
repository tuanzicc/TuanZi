using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuanZi.Mapping
{
    /// <summary>
    /// The base class for attributes that map objects to other objects using AutoMapper.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class MapAttribute : Attribute
    {
        public MapAttribute() { }

        /// <summary>
        /// If true, mapping will be configured in reverse as well.
        /// </summary>
        public bool ReverseMap { get; set; }
    }
}
