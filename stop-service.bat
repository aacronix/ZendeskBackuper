cd %~dp0

sc stop ZendeskBackuperService
timeout /T 2 /NOBREAK
sc delete ZendeskBackuperService
timeout /T 2 /NOBREAK
rmdir /s /q %cd%\zendeskbackupperservice

set /p DUMMY=Hit ENTER to continue...
