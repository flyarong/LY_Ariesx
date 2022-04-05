echo "Start covert"
SET mypath=%~dp0
echo %mypath:~0,-1%
copy %mypath:~0,-1%\..\csv\*.csv %mypath:~0,-1%\..\..\Assets\Configures\
pause
