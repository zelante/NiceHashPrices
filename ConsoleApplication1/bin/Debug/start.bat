@echo off
NiceHashPrices.exe --x11price 1,1
if "%ERRORLEVEL%"=="3" (
	echo Price for Scrypt is good!
) else if "%ERRORLEVEL%"=="1" (
	echo Filed to get prices from NiceHashes!
) else if "%ERRORLEVEL%"=="0" (
	echo Need to set parametrs for NiceHashPrices.exe!
)