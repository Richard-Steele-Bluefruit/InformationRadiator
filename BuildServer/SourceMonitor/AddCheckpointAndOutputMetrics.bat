@echo off

IF DEFINED ProgramFiles(x86) GOTO :64Bit

SET LOCAL_PROGRAM_FILES=%ProgramFiles%
GOTO :Continue

:64Bit
SET LOCAL_PROGRAM_FILES=%ProgramFiles(x86)%

:Continue


cd "%~dp0"
"%LOCAL_PROGRAM_FILES%\SourceMonitor\SourceMonitor.exe" /s AddCheckpointAndExportMetrics.xml
if %ERRORLEVEL% NEQ 0 goto exit_batch


cd "%~dp0"
..\xslconvert "Metrics.xml" MetricsToTeamCity.xslt Metrics.txt
if %ERRORLEVEL% NEQ 0 goto exit_batch

type Metrics.txt

:exit_batch
exit /b %ERRORLEVEL%
