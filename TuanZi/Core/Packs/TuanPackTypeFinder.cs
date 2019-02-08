using System;
using System.Linq;

using TuanZi.Finders;
using TuanZi.Reflection;


namespace TuanZi.Core.Packs
{
    public class TuanPackTypeFinder : BaseTypeFinderBase<TuanPack>, ITuanPackTypeFinder
    {
        public TuanPackTypeFinder(IAllAssemblyFinder allAssemblyFinder)
            : base(allAssemblyFinder)
        { }
    }
}