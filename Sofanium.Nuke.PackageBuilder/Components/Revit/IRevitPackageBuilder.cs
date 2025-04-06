using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.InnoSetup;
using Nuke.Common.Utilities.Collections;
using Sofanium.Nuke.PackageBuilder.Extensions;
using Sofanium.Nuke.PackageBuilder.PackageBuilder;
using Sofanium.Nuke.PackageBuilder.PackageBuilder.Revit;
using System;
using System.IO;
using System.Linq;
using Sofanium.Nuke.Components;
using Sofanium.Nuke.Extensions;

namespace Sofanium.Nuke.PackageBuilder.Components.Revit
{
    /// <summary>
    /// IRevitPackageBuilder
    /// </summary>
    public interface IRevitPackageBuilder : IRevitPackageBuilder<IssRevitBuilder>
    {

    }

    /// <summary>
    /// IRevitPackageBuilder
    /// </summary>
    public interface IRevitPackageBuilder<T> :
        ISofRevitPackageBuilder, ISofInstallationFiles,
        IRelease, ISofPackageBuilder, ISofInput, ISofOutput
        where T : IssPackageBuilder, new()
    {
        /// <summary>
        /// Target PackageBuilder
        /// </summary>
        Target PackageBuilder => d => d
            .TriggeredBy(Sign)
            .Before(Release)
            .Executes(() =>
            {
                Project packageBuilderProject = GetPackageBuilderProject();
                if (GetMainProject() != packageBuilderProject)
                    Solution.BuildProject(packageBuilderProject, (project) =>
                        {
                            SignProject(project);
                            project.ShowInformation();
                        }
                    );

                CreatePackageBuilder(packageBuilderProject, ReleasePackageBuilder, ReleaseBundle);
            });

        /// <summary>
        /// CreatePackageBuilder
        /// </summary>
        /// <param name="project"></param>
        /// <param name="releasePackageBuilder"></param>
        /// <param name="releaseBundle"></param>
        public void CreatePackageBuilder(Project project, bool releasePackageBuilder = false, bool releaseBundle = false)
        {
            var projectName = project.Name;
            var projectVersion = project.GetInformationalVersion();

            if (projectVersion is null)
            {
                throw new Exception($"Project {projectName} has no version, assembly file not found. Make sure your project name is the same as the assembly file name.");
            }

            var projectNameVersion = GetReleaseFileNameVersion(projectName, projectVersion);

            var bundleName = $"{projectName}.bundle";
            var bundleDirectory = PackageBuilderDirectory / bundleName;
            var contentsDirectory = bundleDirectory / "Contents";

            if (ProjectNameFolder)
                contentsDirectory = contentsDirectory / projectName;

            if (ProjectVersionFolder)
                contentsDirectory = contentsDirectory / projectVersion;

            InputDirectory.Copy(contentsDirectory);

            if (ProjectRemoveTargetFrameworkFolder)
            {
                AppendTargetFrameworkExtension.RemoveAppendTargetFrameworkDirectory(contentsDirectory);
            }

            CreateRevitAddinOnProjectFiles(project, contentsDirectory);

            new RevitContentsBuilder(project, bundleDirectory, MiddleVersions, NewVersions)
                .Build(bundleDirectory / "PackageContents.xml");

            if (releasePackageBuilder)
            {
                // CopyInstallationFiles If Exists
                CopyInstallationFilesTo(PackageBuilderDirectory);

                // Create Iss Files
                try
                {
                    Serilog.Log.Information($"IssPackageBuilder: {typeof(T)}");
                    var issPackageBuilder = new T();
                    issPackageBuilder
                        .Initialize(project)
                        .CreatePackage(PackageBuilderDirectory, IssConfiguration)
                        .CreateFile(PackageBuilderDirectory);
                }
                catch (Exception)
                {
                    Serilog.Log.Error($"Error on IssPackageBuilder: {typeof(T)}");
                    throw;
                }

                // Deploy File
                var outputInno = OutputDirectory;
                var packageBuilderDirectory = GetMaxPathFolderOrTempFolder(PackageBuilderDirectory);
                var issFiles = packageBuilderDirectory.GlobFiles($"*{projectName}.iss");

                if (issFiles.IsEmpty())
                    Serilog.Log.Error($"Not found any .iss file in {packageBuilderDirectory}");

                issFiles.ForEach(file =>
                {
                    InnoSetupTasks.InnoSetup(config => config
                        .SetProcessToolPath(NuGetToolPathResolver.GetPackageExecutable("Tools.InnoSetup", "ISCC.exe"))
                        .SetScriptFile(file)
                        .SetOutputDir(outputInno));
                });

                // Sign outputInno
                SignFolder(outputInno);

                // Zip exe Files
                var exeFiles = outputInno.GlobFiles("**/*.exe");
                exeFiles.ForEach(file => ZipExtension.ZipFileCompact(file, projectNameVersion));

                if (exeFiles.IsEmpty())
                    Serilog.Log.Error($"Not found any .exe file in {outputInno}");

                var message = string.Join(" | ", exeFiles.Select(e => e.Name));
                ReportSummary(d => d.AddPair("File", message));

                if (outputInno != ReleaseDirectory)
                {
                    outputInno.GlobFiles("**/*.zip")
                        .ForEach(file => file.CopyToDirectory(ReleaseDirectory));
                }

                var folder = Path.GetFileName(PackageBuilderDirectory);
                var releaseFileName = CreateReleaseFromDirectory(PackageBuilderDirectory, projectName, projectVersion, $".{folder}.zip");
                Serilog.Log.Information($"Release: {releaseFileName}");
            }

            if (releaseBundle)
            {
                var releaseFileName = CreateReleaseFromDirectory(bundleDirectory, projectName, projectVersion, ".bundle.zip", true);
                Serilog.Log.Information($"Release: {releaseFileName}");
            }
        }

        /// <summary>
        /// Create AddIns on each dll with the valid name
        /// </summary>
        /// <param name="project"></param>
        /// <param name="directory"></param>
        private void CreateRevitAddinOnProjectFiles(Project project, AbsolutePath directory)
        {
            var addInFiles = directory.GlobFiles($"**/*{project.Name}*.dll")
                            .Where(e => RevitExtension.HasRevitVersion(e));

            addInFiles.ForEach(file =>
            {
                var folder = file.Parent;
                SignFolder(folder, $"*{project.Name}*");
                new RevitProjectAddInsBuilder(project, file, Application, ApplicationType, VendorId, VendorDescription)
                    .Build(file);
            });
        }

        /// <summary>
        /// Check Folder if pass max path length return a copy with a temp folder
        /// </summary>
        /// <param name="packageBuilderDirectory"></param>
        /// <returns></returns>
        private AbsolutePath GetMaxPathFolderOrTempFolder(AbsolutePath packageBuilderDirectory)
        {
            const string tempFolder = "PackageBuilder";
            const int maxPath = 260;

            var temp = (AbsolutePath)Path.Combine(Path.GetTempPath(), tempFolder);
            Serilog.Log.Information($"Path Max: {temp.ToString().Length} - {temp}");

            var file = packageBuilderDirectory;
            Serilog.Log.Information($"Path Max: {file.ToString().Length} - {Path.GetFileName(file)}");

            packageBuilderDirectory.GlobFiles("**/*")
                .ForEach(path =>
                {
                    Serilog.Log.Information($"Path Max: {path.ToString().Length} - {Path.GetFileName(path)}");
                });

            var max = packageBuilderDirectory.GlobFiles("**/*").Max(path => path.ToString().Length);

            Serilog.Log.Information($"Path Max: {max}");
            if (max >= maxPath)
            {
                if (temp.DirectoryExists()) temp.DeleteDirectory();
                packageBuilderDirectory.Copy(temp);
                var limit = max - file.ToString().Length + temp.ToString().Length;
                Serilog.Log.Information($"Path Max: {limit} - {temp}");
                return temp;
            }

            return packageBuilderDirectory;
        }
    }
}
