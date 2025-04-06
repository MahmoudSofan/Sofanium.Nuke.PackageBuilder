using System.IO;
using System.Linq;
using System.Reflection;

namespace Sofanium.Nuke.PackageBuilder.Extensions;

/// <summary>
/// RevitExtension
/// </summary>
public static class RevitExtension
{
    /// <summary>
    /// HasRevitVersion RevitAPI ReferencedAssemblies
    /// </summary>
    /// <param name="dll"></param>
    /// <returns></returns>
    public static bool HasRevitVersion(string dll)
    {
        return GetRevitVersion(dll) > 0;
    }

    /// <summary>
    /// GetRevitVersion using the RevitAPI ReferencedAssemblies
    /// </summary>
    /// <param name="dll"></param>
    /// <returns></returns>
    public static int GetRevitVersion(string dll)
    {
        var assemblyTest = Assembly.Load(File.ReadAllBytes(dll));

        var revit = assemblyTest.GetReferencedAssemblies()
            .FirstOrDefault(e => e.Name != null && e.Name.StartsWith("RevitAPI"));

        if (revit == null) return 0;

        if (revit.Version != null)
        {
            var version = revit.Version.Major;
            if (version < 2000) version += 2000;

            return version;
        }

        return 0;
    }
}