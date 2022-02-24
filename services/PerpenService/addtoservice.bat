Remove-Service -Name ALARmGapService
New-Service -Name ALARmGapService -BinaryPathName C:\sntfi\ALARm5\services\GapService\bin\Release\netcoreapp3.1\publish\GapService.exe -Description "Perpen processing" -DisplayName "ALARm Perpen Service" -StartupType Automatic
pause