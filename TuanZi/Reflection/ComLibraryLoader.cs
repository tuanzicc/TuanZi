using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;


namespace TuanZi.Reflection
{
    public class ComLibraryLoader : IDisposable
    {
        private delegate int DllGetClassObjectInvoker([MarshalAs(UnmanagedType.LPStruct)] Guid clsid,
            [MarshalAs(UnmanagedType.LPStruct)] Guid iid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppv);
        private static readonly Guid UnknownId = new Guid("00000000-0000-0000-C000-000000000046");
        private IntPtr _lib = IntPtr.Zero;
        private bool _preferURObjects = true;

        public object CreateObjectFromPath(string dllPath, Guid clsid, bool comFallback)
        {
            return CreateObjectFromPath(dllPath, clsid, false, comFallback);
        }

        public object CreateObjectFromPath(string dllPath, Guid clsid, bool setSearchPath, bool comFallback)
        {
            if (File.Exists(dllPath) && (_preferURObjects || !comFallback))
            {
                if (setSearchPath)
                {
                    NativeMethods.SetDllDirectory(Path.GetDirectoryName(dllPath));
                }
                _lib = NativeMethods.LoadLibrary(dllPath);
                if (setSearchPath)
                {
                    NativeMethods.SetDllDirectory(null);
                }
                if (_lib != IntPtr.Zero)
                {
                    IntPtr ptr = NativeMethods.GetProcAddress(_lib, "DllGetClassObject");
                    if (ptr != IntPtr.Zero)
                    {
                        if (Marshal.GetDelegateForFunctionPointer(ptr, typeof(DllGetClassObjectInvoker)) is DllGetClassObjectInvoker invoker)
                        {
                            int hr = invoker(clsid, UnknownId, out object unknow);
                            if (hr >= 0)
                            {
                                if (unknow is IComClassFactory factory)
                                {
                                    factory.CreateInstance(null, UnknownId, out object createdObject);
                                    return createdObject;
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Win32Exception();
                    }
                }
            }
            if (!comFallback)
            {
                throw new Win32Exception();
            }

            Type type = Type.GetTypeFromCLSID(clsid);
            return Activator.CreateInstance(type);
        }
        
        public void Dispose()
        {
            NativeMethods.FreeLibrary(_lib);
            GC.SuppressFinalize(this);
        }
    }
}