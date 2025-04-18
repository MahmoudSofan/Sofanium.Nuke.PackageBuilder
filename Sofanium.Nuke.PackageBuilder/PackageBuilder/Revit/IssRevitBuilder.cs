﻿using InnoSetup.ScriptBuilder;
using Nuke.Common.IO;
using Sofanium.Nuke.Extensions;

namespace Sofanium.Nuke.PackageBuilder.PackageBuilder.Revit
{
    /// <summary>
    /// IssRevitBuilder
    /// </summary>
    public class IssRevitBuilder : IssPackageBuilder
    {
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="packageBuilderDirectory"></param>
        /// <param name="issConfiguration"></param>
        /// <returns></returns>
        public override IssPackageBuilder CreatePackage(AbsolutePath packageBuilderDirectory, IssConfiguration issConfiguration)
        {
            string title = Project.GetTitle();

            if (string.IsNullOrWhiteSpace(issConfiguration.Title) == false)
                title = issConfiguration.Title;

            string appCopyright = Project.GetCopyright();
            string appId = Project.GetAppId();

            string app = $"{title}";
            string appPath = $"{Project.Name}";
            string appVersion = Project.GetInformationalVersion();

            string bundle = $"{appPath}.bundle";
            string appPublisher = Project.GetCompany();
            string appComments = Project.GetDescription();

            string sourceFiles = $@"{bundle}\*";

            var setup =
                Setup.Create(app)
                    .AppVerName(app)
                    .AppId(appId)
                    .AppVersion(appVersion)
                    .AppPublisher(appPublisher)
                    .AppComments(appComments)
                    .AppCopyright(appCopyright)
                    .DefaultDirName($@"{InnoConstants.Shell.CommonAppData}\Autodesk\ApplicationPlugins\{bundle}")
                    .OutputBaseFilename($"{title} {appVersion}")
                    .UninstallDisplayIcon($@"{InnoConstants.Directories.App}\unins000.exe")
                    .DisableWelcomePage(YesNo.No)
                    .DisableDirPage(YesNo.Yes)
                    .ShowLanguageDialog(YesNo.No);

            if ((packageBuilderDirectory / issConfiguration.Icon).FileExists())
                setup.SetupIconFile(issConfiguration.Icon);

            if ((packageBuilderDirectory / issConfiguration.Image).FileExists())
                setup.WizardImageFile(issConfiguration.Image);

            if ((packageBuilderDirectory / issConfiguration.ImageSmall).FileExists())
                setup.WizardSmallImageFile(issConfiguration.ImageSmall);

            if ((packageBuilderDirectory / issConfiguration.Licence).FileExists())
                setup.LicenseFile(issConfiguration.Licence);

            Files.CreateEntry(source: sourceFiles, destDir: $@"\\?\{InnoConstants.Directories.App}")
                .Flags(FileFlags.IgnoreVersion | FileFlags.RecurseSubdirs);

            Languages.CreateEntry(name: issConfiguration.Language.Name,
                messagesFile: issConfiguration.Language.MessagesFile);

            if (issConfiguration.IssLanguageLicences != null)
            {
                foreach (var issLanguageLicence in issConfiguration.IssLanguageLicences)
                {
                    var language = Languages.CreateEntry(name: issLanguageLicence.Name,
                        messagesFile: issLanguageLicence.MessagesFile);

                    if ((packageBuilderDirectory / issLanguageLicence.Licence).FileExists())
                        language.LicenseFile(issLanguageLicence.Licence);
                }
            }
            Sections.CreateParameterSection("UninstallDelete")
                .CreateEntry()
                .Parameter("Type", "filesandordirs")
                .Parameter("Name", $@"{InnoConstants.Directories.App}\*");

            Sections.CreateParameterSection("InstallDelete")
                .CreateEntry()
                .Parameter("Type", "files")
                .Parameter("Name", $@"{InnoConstants.Directories.App}\Contents\*");

            Sections.CreateParameterSection("Dirs")
                .CreateEntry()
                .Parameter("Name", $"{InnoConstants.Directories.App}")
                .Parameter("Permissions", $"users-full");

            return this;
        }
    }
}