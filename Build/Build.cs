using Nuke.Common;
using Nuke.Common.Execution;
using ricaun.Nuke;
using ricaun.Nuke.Components;

[CheckBuildProjectConfigurations]
class Build : NukeBuild, IPublishPack, ICompileExample, IRevitPackageBuilder
{
    string IHazInput.Folder => "Content";
    string IHazPackageBuilderProject.Name => "RevitAddin.PackageBuilder.Example";

    string IHazExample.Name => "RevitAddin.PackageBuilder.Example";
    string IHazExample.Folder => "Content";

    string IHazContent.Folder => "Release";
    string IHazRelease.Folder => "ReleasePack";
    public static int Main() => Execute<Build>(x => x.From<IPublishPack>().Build);
}

/*
[CheckBuildProjectConfigurations]
class Build : NukeBuild, IPublishRevit
{
    string IHazMainProject.MainName => "RevitAddin.PackageBuilder.Example";
    string IHazInput.Folder => "Content";
    string IHazContent.Folder => "Content";
    string IHazPackageBuilder.Application => "Revit.App";
    public static int Main() => Execute<Build>(x => x.From<IPublishRevit>().Build);
}
*/