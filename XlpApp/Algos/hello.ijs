load 'csv'

filepath =: 'C:\Users\ntrncic\source\public_repos\XlpApp\XlpApp\bin\Debug\Algos\data.csv'
outpath =: 'C:\Users\ntrncic\source\public_repos\XlpApp\XlpApp\bin\Debug\Algos\out.csv'

dat =: readcsv filepath
algoname =: {.{.dat
name =: {.2{dat

test =: 3 : 0
if. (<'Average') = algoname do.
	timeseries =: >".&.>}.2{dat
	appendavg =: ( 3 : 'y,~ (+/%#)_5{.y') 
	prediction =: appendavg^:5 timeseries
	predline =: (_5{.prediction) ,~ 0$~ $ timeseries
	outfile =: timeseries,: predline
	out =: ((>name);'Prediction') ,. ('#AAAAAA';'#FF0000') ,. <"0 outfile
	out writecsv outpath
end.
)

test ''








