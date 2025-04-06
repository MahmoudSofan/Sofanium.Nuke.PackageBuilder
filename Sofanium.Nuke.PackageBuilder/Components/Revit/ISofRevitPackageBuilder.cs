using Nuke.Common;
using ricaun.Nuke.Extensions;

namespace Sofanium.Nuke.PackageBuilder.Components.Revit
{
    /// <summary>
    /// IHazRevitPackageBuilder
    /// </summary>
    public interface ISofRevitPackageBuilder : ISofPackageBuilderProject, INukeBuild
    {
        /// <summary>
        /// IExternalApplication Class (default: "App")
        /// </summary>
        [Parameter]
        string Application => TryGetValue(() => Application) ?? "App";

        /// <summary>
        /// Application Type (default: "Application")
        /// </summary>
        [Parameter]
        string ApplicationType => TryGetValue(() => ApplicationType) ?? "Application";

        /// <summary>
        /// GetApplication
        /// </summary>
        /// <returns></returns>
        public string GetApplication() => Application;

        /// <summary>
        /// Add Middle Versions (default: true)
        /// </summary>
        [Parameter]
        bool MiddleVersions => TryGetValue<bool?>(() => MiddleVersions) ?? true;

        /// <summary>
        /// Add New Versions (default: true)
        /// </summary>
        [Parameter]
        bool NewVersions => TryGetValue<bool?>(() => NewVersions) ?? true;

        /// <summary>
        /// VendorId
        /// </summary>
        [Parameter]
        string VendorId => TryGetValue(() => VendorId) ?? GetPackageBuilderProject().GetCompany();

        /// <summary>
        /// VendorDescription
        /// </summary>
        [Parameter]
        string VendorDescription => TryGetValue(() => VendorDescription) ?? VendorId;
    }
}
