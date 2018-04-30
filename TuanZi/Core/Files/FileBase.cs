using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

using TuanZi.Entity;
using TuanZi.Reflection;


namespace TuanZi.Core.Files
{
    public abstract class FileBase<TKey> : EntityBase<TKey> where TKey : IEquatable<TKey>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ContentType { get; set; }
        public long ContentLength { get; set; }
        public string Hash { get; set; }
        public byte[] Binary { get; set; }
        public string Path { get; set; }
    }
}