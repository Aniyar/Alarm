Remove-Service -Name ALARmGapService
New-Service -Name ALARmGapService -BinaryPathName C:\sntfi\ALARm5\services\GapService\bin\Release\netcoreapp3.1\publish\GapService.exe -Description "Gaps processing" -DisplayName "ALARm Gap Service" -StartupType Automatic
pause