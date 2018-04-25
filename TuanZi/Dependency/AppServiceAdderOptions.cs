using TuanZi.Reflection;


namespace TuanZi.Dependency
{
    public class AppServiceAdderOptions
    {
        public AppServiceAdderOptions()
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