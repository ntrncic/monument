CD %HOMEPATH%
CD j64-806
start jconsole.cmd  
ECHO load 'C:\Users\ntrncic\source\repos\XlpApp\XlpApp\bin\Debug\Algos\hello.ijs' | jconsole.cmd
taskkill /im jconsole.cmd