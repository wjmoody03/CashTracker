--work in progress... this just shows calculated balance hist

DECLARE @startdate smalldatetime, @enddate smalldatetime
DECLARE @AccountID int 

set @accountid = 22
SET @Startdate = '12/1/15'
set @enddate = '3/7/16'


select
	d.asofdate,
	a.ID AS AccountID, 
	a.Account, 
	IsNull(SUM(t.Amount * tt.ReconciliationMultiplier),0) + IsNull(a.StartingBalance,0) AS CalculatedBalance,
	bal.AccountBalance
from 
	TableOfDates(@StartDate,@EndDate) d
	cross join dbo.Accounts AS a 
	LEFT JOIN Transactions t ON a.ID = t.AccountID AND t.TransactionDate <= d.Asofdate
	--exclude splits
	left join transactions ts
		on t.id = ts.ParentTransactionID
	LEFT JOIN TransactionTypes tt ON t.TransactionType = tt.ID
	CROSS APPLY (SELECT TOP 1 AccountBalance From AccountDownloadResults adr WHERE adr.StartTime < d.AsOfDate AND adr.AccountID=a.ID ORDER BY StartTime DESC) bal
where
	a.ID = @AccountID or @AccountID is null
	AND ts.Id Is Null
GROUP BY 
	a.iD,
	a.Account, 
	a.StartingBalance,
	d.AsOfDate,
	bal.AccountBalance
ORDER BY
	d.AsOFDate DESC,
	a.ID