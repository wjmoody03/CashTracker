--this will give stated balance for an account as of a certain date
--declare @startdate smalldatetime
--declare @enddate smalldatetime
--declare @AccountType int
--set @startdate = '8/28/15'
--set @enddate = '8/28/15'

--calc the balance
select
	t.AccountID,
	a.AccountType,
	d.AsOfDate,
	SUM(t.Amount * tt.ReconciliationMultiplier) + a.StartingBalance as Balance
from 
	TableOfDates(@StartDate,@EndDate) d
	join transactions t
		on t.TransactionDate <= d.asOfDate 
	join transactiontypes tt
		on t.TransactionType = tt.ID
	join accounts a 
		on t.AccountID = a.ID
	--exclude splits
	left join transactions ts
		on t.id = ts.ParentTransactionID
where
	a.AccountTypeID = @AccountType OR @AccountType Is Null
	and ts.id is null --exclude split transactions
group by 
	t.AccountID,
	a.AccountType,
	d.AsOfDate,
	a.StartingBalance
order by 
	t.AccountiD,
	d.AsOfDate