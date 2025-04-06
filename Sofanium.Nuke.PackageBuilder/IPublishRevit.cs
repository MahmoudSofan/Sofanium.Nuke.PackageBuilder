using Nuke.Common;
using Sofanium.Nuke.Components;
using Sofanium.Nuke.PackageBuilder.Components.Revit;

namespace Sofanium.Nuke.PackageBuilder
{
    /// <summary>
    /// IPublishRevit
    /// </summary>
    public interface IPublishRevit : IRevitPackageBuilder, IGitRelease
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
