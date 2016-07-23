--declare @startdate smalldatetime
--declare @enddate smalldatetime
--set @startdate = '5/28/15'
--set @enddate = '8/28/15'

SELECT
	d.AsOfDate,
	IsNull(SUM(t.Amount * MonthlyCashflowMultiplier),0) as ReimbursableAmountTotal
FROM
	TableOfDates(@StartDate, @EndDate) d 
	LEFT JOIN Transactions t 		
		ON t.TransactionDate <= d.AsofDate
		AND t.FlagForFollowUp!=1 --Don't double count questionable expenses/income
		AND NullIf(LTRIM(RTRIM(t.ReimbursableSource)),'') Is Not Null --Don't double count gifts/transfers
	LEFT JOIN TransactionTypes tt 
		ON  t.TransactionType = tt.ID  		
	--exclude splits
	left join transactions ts
		on t.id = ts.ParentTransactionID
WHERE
	ts.Id Is null
GROUP BY
	d.AsOfDate
ORDER BY 
	d.AsOfDate