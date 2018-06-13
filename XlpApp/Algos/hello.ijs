load 'csv'
IBMPATH =: 'C:\JFiles\ibm.csv'
IBMPATH2 =: 'C:\JFiles\ibmout.csv'
dat =: readcsv IBMPATH
stock=:>".&.>}.4{|:dat
pred =: stock , 140+?10#20
out =: stock ,: pred
out writecsv IBMPATH2


