using TuanZi.Reflection;


namespace TuanZi.Dependency
{
    public class ServiceScanOptions
    {
        public ServiceScanOptions()
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