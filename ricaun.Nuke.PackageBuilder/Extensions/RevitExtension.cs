﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ricaun.Nuke.Extensions
{
    /// <summary>
    /// RevitExtension
    /// </summary>
    public static class RevitExtension
    {
        /// <summary>
        /// GetRevitVersion using the RevitAPI ReferencedAssemblies
        /// </summary>
        /// <param name="dll"></param>
        /// <returns></returns>
        public static int GetRevitVersion(string dll)
        {
            var assemblyTest = Assembly.Load(File.ReadAllBytes(dll));

            var revit = assemblyTest.GetReferencedAssemblies()
                .FirstOrDefault(e => e.Name.StartsWith("RevitAPI"));

            var version = revit.Version.Major;
            if (version < 2000) version += 2000;

            return version;
        }
    }
}
