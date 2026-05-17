$version=$args[0] -replace 'refs/tags/v', '';

[xml]$installerCsproj= Get-Content .\JaebeMusicStudio3.Installer\JaebeMusicStudio3.Installer.csproj;
$installerCsprojVersion=$installerCsproj.CreateElement('AssemblyVersion');
$installerCsprojVersion.AppendChild($installerCsproj.CreateTextNode($version));
$installerCsproj.GetElementsByTagName('PropertyGroup')[0].AppendChild($installerCsprojVersion);
$installerCsproj.Save('.\JaebeMusicStudio3.Installer\JaebeMusicStudio3.Installer.csproj');

[xml]$frontendCsproj= Get-Content .\JaebeMusicStudio3.Front\JaebeMusicStudio3.Front.csproj;
$frontendCsprojVersion=$frontendCsproj.CreateElement('AssemblyVersion');
$frontendCsprojVersion.AppendChild($frontendCsproj.CreateTextNode($version));
$frontendCsproj.GetElementsByTagName('PropertyGroup')[0].AppendChild($frontendCsprojVersion);
$frontendCsproj.Save('.\JaebeMusicStudio3.Front\JaebeMusicStudio3.Front.csproj');



$versionCs=Get-Content .\JaebeMusicStudio3.Core\Version.cs ;
$versionCs=$versionCs -replace 'public static readonly string VERSION = ".*";', "public static readonly string VERSION = `"$version`";";
Set-Content -Path .\JaebeMusicStudio3.Core\Version.cs -Value $versionCs