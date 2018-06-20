load 'csv'
IBMPATH =: 'C:\Users\ntrncic\source\repos\XlpApp\XlpApp\bin\Debug\Algos\tsla.csv'
IBMPATH2 =: 'C:\Users\ntrncic\source\repos\XlpApp\XlpApp\bin\Debug\Algos\tsla_out.csv'
dat =: readcsv IBMPATH
stock=:>".&.>}.4{|:dat
pred =: stock , 140+?10#20
out =: stock ,: pred
out writecsv IBMPATH2


