NaceHashPrices 0.2 (05/26/2014)
by Zelante
Donation:
BTC 1AiT1j185rZ7cokrGrszZJLWAktaFszCUR
DRK XgtrNxB4rK6pB7Y5HmqUFYAaBw9ZaNNQwm
------------------------------------------
Check prices for Scrypt, ScryptN and x11 from NiceHash site and return errorlevel.

errorlevel == 3 Price is good
errorlevel == 1 Failed to get prices from NiceHash
errorlevel == 0 No command line options was set for NiceHashPrices.exe or price is not good.

command line options:
	--scryptPrice 		x.xxxx Setup lowerest price for Scrypt
	--scryptNprice 		x.xxxx Setup lowerest price for ScryptN
	--x11price	 		x.xxxx Setup lowerest price for x11
	--wallet			BTC wallet for payouts from nicehash

example batch file for Scrypt:

	@echo off
	NiceHashPrices.exe --scryptPrice 3.1 --wallet 1AiT1j185rZ7cokrGrszZJLWAktaFszCUR
	if "%ERRORLEVEL%"=="3" (
		echo Price for Scrypt is good!
	) else if "%ERRORLEVEL%"=="1" (
		echo Filed to get prices from NiceHashes!
	) else if "%ERRORLEVEL%"=="0" (
		echo Need to set command line options for NiceHashPrices.exe or price not good!
	)

example batch file for x11:
	@echo off
	NiceHashPrices.exe --x11price 1.2 --wallet 1AiT1j185rZ7cokrGrszZJLWAktaFszCUR
	if "%ERRORLEVEL%"=="3" (
		echo Price for x11 is good!
	) else if "%ERRORLEVEL%"=="1" (
		echo Filed to get prices from NiceHashes!
	) else if "%ERRORLEVEL%"=="0" (
		echo Need to set command line options for NiceHashPrices.exe or price not good!
	)	
