Remove-Service -Name ALARmGapService
New-Service -Name ALARmGapService -BinaryPathName C:\sntfi\ALARm5\services\GapService\bin\Release\netcoreapp3.1\publish\GapService.exe -Description "onlinephotobolts processing" -DisplayName "ALARm onlinephotobolts Service" -StartupType Automatic
pause