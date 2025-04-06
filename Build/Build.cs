using Nuke.Common;
using Sofanium.Nuke;
using Sofanium.Nuke.Components;
using Sofanium.Nuke.PackageBuilder.Components;
using Sofanium.Nuke.PackageBuilder.Components.Revit;

#if !PUBLISH_ONLY_REVIT
class Build : NukeBuild, IPublishPack, IRevitPackageBuilder, ITest, IPrePack
{
    string ISofInstallationFiles.InstallationFiles => "InstallationFiles";
    string ISofPackageBuilderProject.Name => "Example";
    string ISofRevitPackageBuilder.Application => "Revit.App";
    string ISofRevitPackageBuilder.ApplicationType => "Application";
    public static int Main() => Execute<Build>(x => x.From<IPublishPack>().Build);

}
#else
/// <summary>
/// Create IPublishRevit
/// </summary>
class Build : NukeBuild, IPublishRevit
{
    private const string RevitProjectName = "RevitAddin.PackageBuilder.Example";
    bool ISofRelease.ReleaseNameVersion => true;
    string ISofMainProject.MainName => RevitProjectName;
    string ISofRevitPackageBuilder.Application => "Revit.App";

    IssConfiguration ISofInstallationFiles.IssConfiguration => new IssConfiguration()
    {
        Title = "Example",
        IssLanguageLicences
            = new[] {
                new IssLanguageLicence() { Name="br", Licence = "License-br.txt", MessagesFile = @"compiler:Languages\BrazilianPortuguese.isl"}
            }
    };
    public static int Main() => Execute<Build>(x => x.From<IPublishRevit>().Build);
}
#endif