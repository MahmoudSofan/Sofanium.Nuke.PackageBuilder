using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;

namespace Sofanium.Nuke.PackageBuilder.Components
{
    /// <summary>
    /// IHazOutput
    /// </summary>
    public interface ISofOutput : ISofPackageBuilderProject
    {
        /// <summary>
        /// Folder Output 
        /// </summary>
        [Parameter]
        string Folder => TryGetValue(() => Folder) ?? "Output";

        /// <summary>
        /// OutputDirectory
        /// </summary>
        AbsolutePath OutputDirectory => GetOutputDirectory(GetPackageBuilderProject());

        /// <summary>
        /// GetOutputDirectory
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public AbsolutePath GetOutputDirectory(Project project) => project.Directory / "bin" / Folder;
    }
}
