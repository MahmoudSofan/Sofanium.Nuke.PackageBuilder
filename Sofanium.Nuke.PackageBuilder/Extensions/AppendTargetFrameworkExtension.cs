using Nuke.Common.IO;
using Nuke.Common.Utilities.Collections;

namespace Sofanium.Nuke.PackageBuilder.Extensions;

/// <summary>
/// AppendTargetFrameworkExtension
/// </summary>
public static class AppendTargetFrameworkExtension
{
    /// <summary>
    /// RemoveAppendTargetFrameworkDirectory
    /// </summary>
    /// <param name="contentsDirectory"></param>
    public static void RemoveAppendTargetFrameworkDirectory(AbsolutePath contentsDirectory)
    {
        contentsDirectory.GlobDirectories("**/net*")
            .ForEach(targetFrameworkDirectory =>
            {
                var directoryName = targetFrameworkDirectory.Name;
                Serilog.Log.Information($"RemoveAppendTargetFrameworkDirectory: {directoryName} - {targetFrameworkDirectory}");
                if (!targetFrameworkDirectory.Exists()) return;
                if (targetFrameworkDirectory.Parent.ContainsFile("*")) return;
                if (targetFrameworkDirectory.Parent != null)
                {
                    Serilog.Log.Information(
                        $"CopyDirectoryRecursively: {directoryName} to {targetFrameworkDirectory.Parent.Name}");
                    Serilog.Log.Information(
                        $"RemoveTargetFrameworkDirectory: {directoryName} move to {targetFrameworkDirectory.Parent.Name}");
                    targetFrameworkDirectory.Copy(targetFrameworkDirectory.Parent,
                        ExistsPolicy.DirectoryMerge);
                }

                targetFrameworkDirectory.DeleteDirectory();
            });
    }
}