NaceHashPrices 0.3 (06/27/2014)
by Oleksandr Sukhina (zelante)
Donation:
BTC 1AiT1j185rZ7cokrGrszZJLWAktaFszCUR
DRK XgtrNxB4rK6pB7Y5HmqUFYAaBw9ZaNNQwm
------------------------------------------
	
	Get best normalized price from NiceHash.com.

	Return errorlevel code same to algo number:
    0 = Scrypt
	2 = Scrypt-A.-Nfactor
    3 = X11
	4 = X13
	5 = Keccak

	1 = error

	Normalized price algorithm:
	
	Scrypt	refference  x1   BTC/GH/Day profitability
    Scrypt-N	        x0.5 BTC/GH/Day profitability
    X11	                x4   BTC/GH/Day profitability
    X13	                x3   BTC/GH/Day profitability
    Keccak	            x500 BTC/GH/Day profitability


command line options:
	-w,--wallet			You BTC wallet for payouts from nicehash

example batch file for Scrypt:

	@echo off
	NiceHashPrices.exe --wallet %wallet%
	if "%ERRORLEVEL%"=="5" echo Best price normilized - Keccak
	if "%ERRORLEVEL%"=="4" echo Best price normilized - x13
	if "%ERRORLEVEL%"=="3" echo Best price normilized - x11
	if "%ERRORLEVEL%"=="2" echo Best price normilized - ScyptN
	if "%ERRORLEVEL%"=="0" echo Best price normilized - Scrypt
