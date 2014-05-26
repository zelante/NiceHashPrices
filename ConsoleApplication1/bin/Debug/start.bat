@echo off
NiceHashPrices.exe --x11price 0.6
if "%ERRORLEVEL%"=="3" (
	echo Price for x11 is good!
) else if "%ERRORLEVEL%"=="1" (
	echo Filed to get prices from NiceHashes!
) else if "%ERRORLEVEL%"=="0" (
	echo Need to set parametrs for NiceHashPrices.exe!
)
pause