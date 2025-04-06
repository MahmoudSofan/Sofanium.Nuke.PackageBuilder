using Nuke.Common;
using ricaun.Nuke.Components;
using Sofanium.Nuke.PackageBuilder.Components.Revit;

namespace Sofanium.Nuke.PackageBuilder
{
    /// <summary>
    /// IPublishRevit
    /// </summary>
    public interface IPublishRevit : ICompile, IClean, ISign, IRelease, IRevitPackageBuilder, IGitRelease, IHazSolution, INukeBuild
    {
        /// <summary>
        /// Target Build
        /// </summary>
        Target Build => d => d
            .DependsOn(Compile)
            .Executes(() =>
            {

            });
    }
}
