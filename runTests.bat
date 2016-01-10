@ECHO OFF

REM Find MSBuild.
REM by yoyo: http://stackoverflow.com/a/13752506/1700174
FOR /D %%D in (%SYSTEMROOT%\Microsoft.NET\Framework\v4*) DO SET msbuild.exe=%%D\MSBuild.exe

IF NOT EXIST "%msbuild.exe%" (
	ECHO Error: MSBuild not found. Could not compile tests.
	EXIT /B 1
)

REM Compile tests.
CALL %msbuild.exe% Tests\Tests.csproj /property:Configuration=Debug /verbosity:minimal

IF NOT %ERRORLEVEL% == 0 (
	ECHO Error %ERRORLEVEL%: Could not compile tests.
	EXIT /B %ERRORLEVEL%
)

REM Run tests.
Tests\Libraries\NSpec\NSpecRunner.exe Tests\bin\Debug\Tests.dll	
