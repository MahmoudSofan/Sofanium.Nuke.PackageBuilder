using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;

namespace Sofanium.Nuke.PackageBuilder.Components
{
    /// <summary>
    /// ISofPackageBuilder
    /// </summary>
    public interface ISofPackageBuilder : ISofPackageBuilderProject
    {
        /// <summary>
        /// Folder PackageBuilder 
        /// </summary>
        [Parameter]
        string Folder => TryGetValue(() => Folder) ?? "PackageBuilder";

        /// <summary>
        /// PackageBuilderDirectory
        /// </summary>
        AbsolutePath PackageBuilderDirectory => GetPackageBuilderDirectory(GetPackageBuilderProject());

        /// <summary>
        /// GetPackageBuilderDirectory
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public AbsolutePath GetPackageBuilderDirectory(Project project) => project.Directory / "bin" / Folder;
    }
}
