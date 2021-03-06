﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyModel;

using TuanZi.Collections;
using TuanZi.Finders;

namespace TuanZi.Reflection
{
    public class AppDomainAllAssemblyFinder : FinderBase<Assembly>, IAllAssemblyFinder
    {
        private readonly bool _filterNetAssembly;

        public AppDomainAllAssemblyFinder(bool filterNetAssembly = true)
        {
            _filterNetAssembly = filterNetAssembly;
        }

        protected override Assembly[] FindAllItems()
        {
            string[] filters =
            {
                "System",
                "Microsoft",
                "netstandard",
                "dotnet",
                "Window",
                "mscorlib"
            };
            DependencyContext context = DependencyContext.Default;
            if (context != null)
            {
                List<string> names = new List<string>();
                string[] dllNames = context.CompileLibraries.SelectMany(m => m.Assemblies).Distinct().Select(m => m.Replace(".dll", "")).ToArray();
                if (dllNames.Length > 0)
                {
                    names = (from name in dllNames
                             let i = name.LastIndexOf('/') + 1
                             select name.Substring(i, name.Length - i)).Distinct()
                        .WhereIf(name => !filters.Any(name.StartsWith), _filterNetAssembly)
                        .ToList();
                }
                else
                {
                    foreach (CompilationLibrary library in context.CompileLibraries)
                    {
                        string name = library.Name;
                        if (_filterNetAssembly && filters.Any(name.StartsWith))
                        {
                            continue;
                        }
                        if (name == "TuanZiNS")
                        {
                            continue;
                        }
                        if (name == "TuanZiNS.Core")
                        {
                            name = "TuanZi";
                        }
                        else if (name.StartsWith("TuanZiNS."))
                        {
                            name = name.Replace("TuanZiNS.", "TuanZi.");
                        }
                        if (!names.Contains(name))
                        {
                            names.Add(name);
                        }
                    }
                }
                return LoadFiles(names);
            }

            string path = AppDomain.CurrentDomain.BaseDirectory;
            string[] files = Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly)
                .Concat(Directory.GetFiles(path, "*.exe", SearchOption.TopDirectoryOnly))
                .ToArray();
            if (_filterNetAssembly)
            {
                string[] files1 = files;
                files = files.WhereIf(m => files1.Any(n => m.StartsWith(n, StringComparison.OrdinalIgnoreCase)), _filterNetAssembly).ToArray();
            }
            return files.Select(Assembly.LoadFrom).ToArray();
        }

        private static Assembly[] LoadFiles(IEnumerable<string> files)
        {
            List<Assembly> assemblies = new List<Assembly>();
            foreach (string file in files)
            {
                AssemblyName name = new AssemblyName(file);
                try
                {
                    assemblies.Add(Assembly.Load(name));
                }
                catch (FileNotFoundException)
                { }
            }
            return assemblies.ToArray();
        }
    }

}