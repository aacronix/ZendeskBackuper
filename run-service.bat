dotnet publish -c Release -r win10-x64 -o %~dp0\zendeskbackupperservice /p:PublishTrimmed=true --self-contained

sc create ZendeskBackuperService binPath= %~dp0\zendeskbackupperservice\ZendeskBackuperRunner.exe
timeout /T 2 /NOBREAK
sc start ZendeskBackuperService
timeout /T 2 /NOBREAK
sc config ZendeskBackuperService start=auto

set /p DUMMY=Hit ENTER to continue...
