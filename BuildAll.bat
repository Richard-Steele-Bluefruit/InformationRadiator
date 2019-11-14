@echo off
setlocal enabledelayedexpansion

set VSTEST="C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"
set DEVENV="C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe"
set OPENCOVER="C:\Program Files (x86)\OpenCover\OpenCover.Console.exe"
set REPORTGENERATOR="%~dp0packages\ReportGenerator.2.3.5.0\tools\ReportGenerator.exe"
set SEVENZIP="C:\Program Files\7-Zip\7z.exe"

REM ===================================================

call "%~dp0BuildServer\SourceMonitor\AddCheckpointAndOutputMetrics.bat"
if %ERRORLEVEL% NEQ 0 goto exit_batch

REM ===================================================

echo ##teamcity[progressMessage 'Building Solution']
cd "%~dp0"
%DEVENV% InformationRadiator.sln /Rebuild Release
if %ERRORLEVEL% NEQ 0 goto exit_batch

cd "%~dp0"
for /f %%i in ('BuildServer\SigCheck\SigCheck.exe -q -n InformationRadiator\bin\Release\InformationRadiator.exe') do set RELEASEFILEVERSION=%%i

REM ===================================================

set RELEASEFILE=%~dp0InformationRadiator - Version %RELEASEFILEVERSION%.zip

%SEVENZIP% a "%RELEASEFILE%" "%~dp0InformationRadiator\bin\Release\*.exe"
if %ERRORLEVEL% NEQ 0 goto exit_batch

%SEVENZIP% a "%RELEASEFILE%" "%~dp0InformationRadiator\bin\Release\*.dll"
if %ERRORLEVEL% NEQ 0 goto exit_batch

echo ##teamcity[publishArtifacts '%RELEASEFILE%']

REM ===================================================

set LEANKITQUERYRELEASEFILE=%~dp0LeanKitQuery - Version %RELEASEFILEVERSION%.zip

%SEVENZIP% a "%LEANKITQUERYRELEASEFILE%" "%~dp0LeanKitQuery\bin\Release\*.exe"
if %ERRORLEVEL% NEQ 0 goto exit_batch

%SEVENZIP% a "%LEANKITQUERYRELEASEFILE%" "%~dp0LeanKitQuery\bin\Release\*.dll"
if %ERRORLEVEL% NEQ 0 goto exit_batch

%SEVENZIP% a "%LEANKITQUERYRELEASEFILE%" "%~dp0LeanKitQuery\bin\Release\*.config"
if %ERRORLEVEL% NEQ 0 goto exit_batch

echo ##teamcity[publishArtifacts '%LEANKITQUERYRELEASEFILE%']

REM ===================================================

set HOLIDAYCALENDARDOWNLOADERRELEASEFILE=%~dp0HolidayCalendarDownloader - Version %RELEASEFILEVERSION%.zip

%SEVENZIP% a "%HOLIDAYCALENDARDOWNLOADERRELEASEFILE%" "%~dp0HolidayCalendarDownloader\bin\Release\*.exe"
if %ERRORLEVEL% NEQ 0 goto exit_batch

%SEVENZIP% a "%HOLIDAYCALENDARDOWNLOADERRELEASEFILE%" "%~dp0HolidayCalendarDownloader\bin\Release\*.dll"
if %ERRORLEVEL% NEQ 0 goto exit_batch

%SEVENZIP% a "%HOLIDAYCALENDARDOWNLOADERRELEASEFILE%" "%~dp0HolidayCalendarDownloader\bin\Release\*.config"
if %ERRORLEVEL% NEQ 0 goto exit_batch

echo ##teamcity[publishArtifacts '%HOLIDAYCALENDARDOWNLOADERRELEASEFILE%']

REM ===================================================

echo ##teamcity[progressMessage 'Running Tests']
cd "%~dp0"
set test_files=
for /D %%f in (*.Tests) do set test_files=!test_files! %%f\bin\Release\%%f.dll

set VSTESTARGUMENTS="%test_files% /logger:trx"
%OPENCOVER% -register:user -target:%VSTEST% -targetargs:%VSTESTARGUMENTS% -output:CodeCoverage.xml -mergebyhash
if %ERRORLEVEL% NEQ 0 goto exit_batch

REM ===================================================

for %%f in (%~dp0TestResults\*.trx) do echo ##teamcity[importData type='mstest' path='%%f']

echo ##teamcity[importData type='dotNetCoverage' tool='partcover' path='%~dp0CodeCoverage.xml']

REM ===================================================

cd "%~dp0"
%REPORTGENERATOR% -reports:CodeCoverage.xml  "-targetdir:%~dp0CoverageReport" -reporttypes:Html
if %ERRORLEVEL% NEQ 0 goto exit_batch

%SEVENZIP% a "%~dp0CoverageReport.zip" "%~dp0CoverageReport"
if %ERRORLEVEL% NEQ 0 goto exit_batch

echo ##teamcity[publishArtifacts '%~dp0CoverageReport.zip']

cd "%~dp0"
"%~dp0BuildServer\XSLConvert.exe" "%~dp0CodeCoverage.xml" "%~dp0BuildServer\OpenCover\CoverageToTeamCity.xslt" Coverage.txt
if %ERRORLEVEL% NEQ 0 goto exit_batch
type Coverage.txt

REM ===================================================

:exit_batch
exit /b %ERRORLEVEL%
