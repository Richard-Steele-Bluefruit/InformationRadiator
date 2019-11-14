using System;
using System.Collections.Generic;
using System.Reflection;

namespace PresenterCommon.Plugin
{
    public class PluginFinder<T> where T : class
    {
        public static IEnumerable<Type> SearchAssemblies(IEnumerable<Assembly> assembliesToSearch)
        {
            var plugins = new List<Type>();

            foreach(var assembly in assembliesToSearch)
            {
                Type[] types = assembly.GetTypes();
                foreach(var type in types)
                {
                    if(!type.IsAbstract && type.IsPublic)
                    {
                        var interfaces = type.GetInterfaces();

                        if (Array.Find(interfaces, i => i == typeof(T)) != null)
                            plugins.Add(type);
                    }
                }
            }
            return plugins;
        }

        public static IEnumerable<Type> SearchPath(string path)
        {
            var files = System.IO.Directory.EnumerateFiles(path, "*Views.dll",
                System.IO.SearchOption.TopDirectoryOnly);

            var toSearch = new List<Assembly>();

            foreach(var file in files)
            {
                try
                {
                    Assembly a = Assembly.LoadFile(file);
                    toSearch.Add(a);
                }
                catch(Exception) { }
            }

            return SearchAssemblies(toSearch);
        }
    }
}
