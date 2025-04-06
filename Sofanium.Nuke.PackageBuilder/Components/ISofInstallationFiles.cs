using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Utilities.Collections;
using Sofanium.Nuke.PackageBuilder.PackageBuilder;
using System;
using System.IO;
using System.IO.Compression;
using Sofanium.Nuke.Extensions;

namespace Sofanium.Nuke.PackageBuilder.Components;

/// <summary>
/// ISofInstallationFiles
/// </summary>
public interface ISofInstallationFiles : ISofPackageBuilderProject 
{
    /// <summary>
    /// Folder/Url (default: InstallationFiles)
    /// </summary>
    [Parameter]
    string InstallationFiles => TryGetValue(() => InstallationFiles) ?? "InstallationFiles";

    /// <summary>
    /// Configuration
    /// </summary>
    [Parameter]
    IssConfiguration IssConfiguration => TryGetValue(() => IssConfiguration) ?? new IssConfiguration();

    /// <summary>
    /// InstallationFilesDirectory
    /// </summary>
    AbsolutePath InstallationFilesDirectory => GetInstallationFilesDirectory(GetPackageBuilderProject());

    /// <summary>
    /// GetInstallationFilesDirectory
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    public AbsolutePath GetInstallationFilesDirectory(Project project) => project.Directory / InstallationFiles;

    /// <summary>
    /// CopyInstallationFilesTo
    /// </summary>
    /// <param name="packageBuilderDirectory"></param>
    public void CopyInstallationFilesTo(AbsolutePath packageBuilderDirectory)
    {
        Serilog.Log.Information($"InstallationFiles: {InstallationFiles}");
        DownloadFilesAndUnzip(InstallationFiles, packageBuilderDirectory);

        InstallationFilesDirectory.GlobFiles("*")
            .ForEach(file => file.CopyToDirectory(packageBuilderDirectory));
    }

    /// <summary>
    /// Download Files from url
    /// </summary>
    /// <param name="url"></param>
    /// <param name="downloadFolder"></param>
    /// <returns></returns>
    private bool DownloadFilesAndUnzip(string url, string downloadFolder)
    {
        try
        {
            var fileName = Path.GetFileName(url);
            var file = Path.Combine(downloadFolder, fileName);

            if (!HttpClientExtension.DownloadFileRetry(url, file))
            {
                Serilog.Log.Warning($"DownloadFileRetry Fail");
                return false;
            }

            if (Path.GetExtension(file).EndsWith("zip"))
            {
                ZipFile.ExtractToDirectory(file, Path.GetDirectoryName(file) ?? throw new InvalidOperationException());
                File.Delete(file);
            }
            Serilog.Log.Information($"DownloadFiles: {fileName}");
            return true;
        }
        catch
        {
            // ignored
        }

        return false;
    }
}