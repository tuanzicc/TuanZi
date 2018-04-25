﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;


namespace TuanZi.Reflection
{
    public class DirectoryAssemblyFinder : IAssemblyFinder
    {
        private static readonly ConcurrentDictionary<string, Assembly[]> CacheDict = new ConcurrentDictionary<string, Assembly[]>();
        private readonly string _path;
        
        public DirectoryAssemblyFinder(string path)
        {
            _path = path;
        }

        public Assembly[] Find(Func<Assembly, bool> predicate, bool fromCache = false)
        {
            return FindAll(fromCache).Where(predicate).ToArray();
        }

        public Assembly[] FindAll(bool fromCache = false)
        {
            if (fromCache && CacheDict.ContainsKey(_path))
            {
                return CacheDict[_path];
            }
            string[] files = Directory.GetFiles(_path, "*.dll", SearchOption.TopDirectoryOnly)
                .Concat(Directory.GetFiles(_path, "*.exe", SearchOption.TopDirectoryOnly))
                .ToArray();
            Assembly[] assemblies = files.Select(Assembly.LoadFrom).Distinct().ToArray();
            CacheDict[_path] = assemblies;
            return assemblies;
        }

    }
}