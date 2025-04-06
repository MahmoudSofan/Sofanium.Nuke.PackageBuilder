# ricaun.Nuke.PackageBuilder

This package is to simplify the build automation system using to RevitAddin Application. 
- [ricaun.Nuke](https://www.nuget.org/packages/ricaun.Nuke) 
- [Autodesk.PackageBuilder](https://www.nuget.org/packages/Autodesk.PackageBuilder/)
- [InnoSetup.ScriptBuilder](https://www.nuget.org/packages/InnoSetup.ScriptBuilder/)

[![Revit 2017](https://img.shields.io/badge/Revit-2017+-blue.svg)](https://github.com/ricaun-io/ricaun.Nuke.PackageBuilder)
[![Visual Studio 2022](https://img.shields.io/badge/Visual%20Studio-2022-blue)](https://github.com/ricaun-io/ricaun.Nuke.PackageBuilder)
[![Nuke](https://img.shields.io/badge/Nuke-Build-blue)](https://nuke.build/)
[![License MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Build](https://github.com/ricaun-io/ricaun.Nuke.PackageBuilder/actions/workflows/Build.yml/badge.svg)](https://github.com/ricaun-io/ricaun.Nuke.PackageBuilder/actions)
[![Release](https://img.shields.io/nuget/v/ricaun.Nuke.PackageBuilder?logo=nuget&label=release&color=blue)](https://www.nuget.org/packages/ricaun.Nuke.PackageBuilder)

# Example

```C#
using Nuke.Common;
using Nuke.Common.Execution;
using Sofanium.Nuke;
using Sofanium.Nuke.Components;

class Build : NukeBuild, IPublishRevit
{
    // string ISofRevitPackageBuilder.Application => "Revit.App";
    public static int Main() => Execute<Build>(x => x.From<IPublishRevit>().Build);
}
```

## Environment Variables

```yml
env:
    GitHubToken: ${{ secrets.GITHUB_TOKEN }}
    SignFile: ${{ secrets.SIGN_FILE }}
    SignPassword: ${{ secrets.SIGN_PASSWORD }}
    InstallationFiles: ${{ secrets.INSTALLATION_FILES }}
```

## IPublishRevit

### ISofPackageBuilderProject
```C#
string ISofPackageBuilderProject.Name => "Example";
bool ISofPackageBuilderProject.ReleasePackageBuilder => true;
bool ISofPackageBuilderProject.ReleaseBundle => true;
bool ISofPackageBuilderProject.ProjectNameFolder => true;
bool ISofPackageBuilderProject.ProjectVersionFolder => true;
bool ISofPackageBuilderProject.ProjectRemoveTargetFrameworkFolder => true;
```

### ISofRevitPackageBuilder

```C#
string ISofRevitPackageBuilder.Application => "Revit.App";
string ISofRevitPackageBuilder.ApplicationType => "Application";
bool ISofRevitPackageBuilder.MiddleVersions => true;
bool ISofRevitPackageBuilder.NewVersions => true;
string ISofRevitPackageBuilder.VendorId => "VendorId";
string ISofRevitPackageBuilder.VendorDescription => "VendorDescription";
```

### ISofInstallationFiles

```C#
string ISofInstallationFiles.InstallationFiles => "InstallationFiles";
IssConfiguration ISofInstallationFiles.IssConfiguration => new IssConfiguration()
{
    Image = "image.bmp",
    ImageSmall = "imageSmall.bmp",
    Icon = "icon.ico",
    Licence = "License.txt",
    Language = new IssLanguage() { Name = "en", MessagesFile = "compiler:Default.isl"},
    IssLanguageLicences
        = new[] {
            new IssLanguageLicence() { Name="br", Licence = "License-br.txt", MessagesFile = @"compiler:Languages\BrazilianPortuguese.isl"}
        }
};
```


## License

This package is [licensed](LICENSE) under the [MIT License](https://en.wikipedia.org/wiki/MIT_License).

---

Do you like this package? Please [star this project on GitHub](https://github.com/ricaun-io/ricaun.Nuke.PackageBuilder/stargazers)!

---

Copyright © 2022 ricaun

