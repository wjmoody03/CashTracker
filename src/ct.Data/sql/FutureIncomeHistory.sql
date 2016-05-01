--declare @startdate smalldatetime
--declare @enddate smalldatetime
--set @startdate = '5/28/15'
--set @enddate = '8/28/15'


SELECT
	d.AsOfDate,
	IsNull(SUM(t.Amount * MonthlyCashflowMultiplier),0) as IncomeForNextMonth
FROM
	TableOfDates(@StartDate, @EndDate) d 
	LEFT JOIN Transactions t 		
		ON Year(t.TransactionDate) = YEAR(d.AsOfDate) --it happened during this month
		AND Month(t.TransactionDate) = Month(d.AsOfDate)
		AND t.TransactionDate <= d.AsofDate
		AND t.FlagForFollowUp !=1 --Don't double count questionable expenses/income
		AND t.ReimbursableSource Is Null --Don't double count gifts/transfers
	LEFT JOIN TransactionTypes tt 
		ON  t.TransactionType = tt.ID  
		AND tt.CountAsIncome !=0
	--exclude splits
	left join transactions ts
		on t.id = ts.ParentTransactionID
WHERE
	ts.Id Is Null
GROUP BY
	d.AsOfDate
ORDER BY 
	d.AsOfDate