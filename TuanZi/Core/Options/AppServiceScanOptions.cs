using TuanZi.Dependency;
using TuanZi.Reflection;


namespace TuanZi.Core.Options
{
    public class AppServiceScanOptions
    {
        public AppServiceScanOptions()
        {
            TransientTypeFinder = new TransientDependencyTypeFinder();
            ScopedTypeFinder = new ScopedDependencyTypeFinder();
            SingletonTypeFinder = new SingletonDependencyTypeFinder();
        }

        public ITypeFinder TransientTypeFinder { get; set; }

        public ITypeFinder ScopedTypeFinder { get; set; }

        public ITypeFinder SingletonTypeFinder { get; set; }
    }
}