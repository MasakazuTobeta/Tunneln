@echo off
REM https://qiita.com/kurukurupapa@github/items/f4439fd5c33f6ebfec62

set basedir=%~dp0
set basename=%~n0
set batname=%~n0%~x0
set datestr=%DATE:/=%
set timestrtmp=%TIME: =0%
set timestr=%timestrtmp:~0,2%%timestrtmp:~3,2%%timestrtmp:~6,2%
set timestamp=%datestr%-%timestr%

:INIT
if "%1"=="/?" (
  echo 使い方：%batname% 入力ファイル 出力ファイル 置換前文字列 置換後文字列
  exit /b 0
)
if "%4"=="" (
  echo 引数の数が不正です。
  goto ERROR
)
set inpath=%1
set outpath=%2
set before=%3
set after=%4

:MAIN
call :LOG 処理開始します。


type nul > %outpath%
setlocal enabledelayedexpansion
for /f "tokens=1* delims=: eol=" %%a in ('findstr /n "^" %inpath%') do (
  set line=%%b
  if not "!line!" == "" (
    set line=!line:%before%=%after%!
  )
  echo.!line!>> %outpath%
)
endlocal


:END
call :LOG 正常終了です。
exit /b 0

:ERROR
call :LOG 異常終了です。
exit /b 1

:LOG
echo %DATE% %TIME% %basename% %1
exit /b 0

:EOF