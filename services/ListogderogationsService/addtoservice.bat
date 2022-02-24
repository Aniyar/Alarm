Remove-Service -Name ALARmGapService
New-Service -Name ALARmGapService -BinaryPathName C:\sntfi\ALARm5\services\GapService\bin\Release\netcoreapp3.1\publish\GapService.exe -Description "Listogderogations processing" -DisplayName "ALARm Listogderogations Service" -StartupType Automatic
pause