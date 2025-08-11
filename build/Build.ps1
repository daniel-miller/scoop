$Now            = (Get-Date)
$NowUtc         = $Now.ToUniversalTime()

$MajorNumber    = 1  # increment for incompatible API changes or breaking changes
$MinorNumber    = 0  # increment for new functionality that is backward-compatible
$PatchNumber    = 0  # increment for backward-compatible hotfixes

$PackageVersion = "$MajorNumber.$MinorNumber.$PatchNumber"
$PackageName    = "Scoop"


Write-Host "`nStep 1: Starting build for $PackageName version $PackageVersion`n" -ForegroundColor White

  $PublishFolder = ".\releases\$PackageName"
  $ReleasePath   = ".\releases\$PackageName.$PackageVersion.zip"

  if (Test-Path -Path $PublishFolder) { Remove-Item -Path $PublishFolder\* -Recurse }

  MSBuild Scoop.proj /target:BuildApp /p:Platform=AnyCpu /p:AssemblyVersion=$PackageVersion

  Compress-Archive -Path $PublishFolder\* -DestinationPath $ReleasePath -Force

  
# exit


Write-Host "`nBuild Stage 2: Uploading packages to Octopus...`n" -ForegroundColor Green

  $OctoServer = Get-Content -Path d:\temp\secret\miller-octo-url.txt -TotalCount 1
  $OctoKey    = Get-Content -Path d:\temp\secret\miller-octo-key.txt -TotalCount 1

  Octo push --server=$OctoServer --apiKey=$OctoKey --replace-existing --package=$ReleasePath
 
  $elapsedTime = $(get-date) - $Now
  Write-Host "`nStep 2: Completed upload. Elapsed time = $($ElapsedTime.ToString("mm\:ss"))`n" -ForegroundColor Green