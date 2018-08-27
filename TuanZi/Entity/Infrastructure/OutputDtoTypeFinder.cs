using TuanZi.Reflection;


namespace TuanZi.Entity
{
    public class OutputDtoTypeFinder : BaseTypeFinderBase<IOutputDto>, IOutputDtoTypeFinder
    {
        public OutputDtoTypeFinder(IAllAssemblyFinder allAssemblyFinder)
            : base(allAssemblyFinder)
        { }
    }
}