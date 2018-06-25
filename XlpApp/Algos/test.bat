@echo off
CD %HOMEPATH%
CD j64-806
start jconsole.cmd  
ECHO load '%~dp0\hello.ijs' | jconsole.cmd
taskkill /im cmd.exe