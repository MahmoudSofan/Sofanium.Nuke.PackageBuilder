using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;

namespace Sofanium.Nuke.PackageBuilder.Components
{
    /// <summary>
    /// ISofInput
    /// </summary>
    public interface ISofInput : ISofPackageBuilderProject
    {
        /// <summary>
        /// Folder Input 
        /// </summary>
        [Parameter]
        string Folder => TryGetValue(() => Folder) ?? "Release";

        /// <summary>
        /// InputDirectory
        /// </summary>
        AbsolutePath InputDirectory => GetInputDirectory(GetPackageBuilderProject());

        /// <summary>
        /// GetInputDirectory
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public AbsolutePath GetInputDirectory(Project project) => project.Directory / "bin" / Folder;
    }
}
