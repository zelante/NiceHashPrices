@echo off
NiceHashPrices.exe --scryptPrice 3,1
if "%ERRORLEVEL%"=="3" (
	echo Price for Scrypt is good!
) else if "%ERRORLEVEL%"=="1" (
	echo Filed to get prices from NiceHashes!
) else if "%ERRORLEVEL%"=="0" (
	echo Need to set parametrs for NiceHashPrices.exe!
)