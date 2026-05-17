$version=$args[0] -replace 'refs/tags/v', '';
$versionClear= $version  -replace '[^0-9\.].*$', '.0';

[xml]$installerCsproj= Get-Content .\JaebeMusicStudio3.Installer\JaebeMusicStudio3.Installer.csproj;
$installerCsprojVersion=$installerCsproj.CreateElement('AssemblyVersion');
$installerCsprojVersion.AppendChild($installerCsproj.CreateTextNode($versionClear));
$installerCsproj.GetElementsByTagName('PropertyGroup')[0].AppendChild($installerCsprojVersion);
$installerCsprojVersion2=$installerCsproj.CreateElement('AssemblyInformationalVersion');
$installerCsprojVersion2.AppendChild($installerCsproj.CreateTextNode($version));
$installerCsproj.GetElementsByTagName('PropertyGroup')[0].AppendChild($installerCsprojVersion2);
$installerCsproj.Save('.\JaebeMusicStudio3.Installer\JaebeMusicStudio3.Installer.csproj');

[xml]$frontendCsproj= Get-Content .\JaebeMusicStudio3.Front\JaebeMusicStudio3.Front.csproj;
$frontendCsprojVersion=$frontendCsproj.CreateElement('AssemblyVersion');
$frontendCsprojVersion.AppendChild($frontendCsproj.CreateTextNode($versionClear));
$frontendCsproj.GetElementsByTagName('PropertyGroup')[0].AppendChild($frontendCsprojVersion);
$frontendCsprojVersion2=$frontendCsproj.CreateElement('AssemblyInformationalVersion');
$frontendCsprojVersion2.AppendChild($frontendCsproj.CreateTextNode($version));
$frontendCsproj.GetElementsByTagName('PropertyGroup')[0].AppendChild($frontendCsprojVersion2);
$frontendCsproj.Save('.\JaebeMusicStudio3.Front\JaebeMusicStudio3.Front.csproj');



$versionCs=Get-Content .\JaebeMusicStudio3.Core\Version.cs ;
$versionCs=$versionCs -replace 'public static readonly string VERSION = ".*";', "public static readonly string VERSION = `"$version`";";
Set-Content -Path .\JaebeMusicStudio3.Core\Version.cs -Value $versionCs