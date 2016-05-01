----this will give stated balance for an account as of a certain date
--declare @startdate smalldatetime
--declare @enddate smalldatetime
--set @startdate = '1/1/15'
--set @enddate = '10/28/15'

declare @startEffectiveMonth smalldatetime, @endEffectiveMonth smalldatetime

--this is tricky since we offset income/expenses by 1 month
SET @startEffectiveMonth = CAST(CAST(DatePart(yyyy,@startdate) as varchar(4)) + '-' + CAST(DatePart(m,@startdate) as varchar(4)) + '-01' as smalldatetime)
SET @endEffectiveMonth = CAST(CAST(DatePart(yyyy,@enddate) as varchar(4)) + '-' + CAST(DatePart(m,@enddate) as varchar(4)) + '-01' as smalldatetime)

SELECT
	EffectiveMonth,
	SUM(
		CASE
			WHEN CountAsIncome=1 THEN Amount * MonthlyCashFlowMultiplier 
			ELSE 0 
		END
	) as Income,
	ABS(SUM(
		CASE
			WHEN CountAsIncome=0 THEN Amount * MonthlyCashFlowMultiplier 
			ELSE 0 
		END
	)) as Expenses
FROM (
			SELECT
				CASE 
					WHEN tt.CountAsIncome = 1 THEN 
						--add 1 month to the transaction date, then convert it to the first day of the month
						CAST(CAST(DatePart(yyyy,DateAdd(m,1,t.TransactionDate)) as varchar(4)) + '-' + CAST(DatePart(m,DateAdd(m,1,t.TransactionDate)) as varchar(4)) + '-01' as smalldatetime)
					ELSE 
						CAST(CAST(DatePart(yyyy,t.TransactionDate) as varchar(4)) + '-' + CAST(DatePart(m,t.TransactionDate) as varchar(4)) + '-01' as smalldatetime)
				END as EffectiveMonth,
				t.Amount, 
				tt.MonthlyCashFlowMultiplier,
				tt.CountAsIncome
			FROM
				Transactions t 				
				LEFT JOIN TransactionTypes tt 
					ON  t.TransactionType = tt.ID 
				--exclude splits
				left join transactions ts
					on t.id = ts.ParentTransactionID 
			WHERE
				t.FlagForFollowUp !=1 --Don't double count questionable expenses/income
				AND t.ReimbursableSource Is Null --Don't double count gifts/transfers
				and ts.Id Is Null --exclude splits
	) sub
WHERE
	sub.EffectiveMonth BETWEEN @startEffectiveMonth AND @endEffectiveMonth
GROUP BY 
	sub.EffectiveMonth
ORDER BY 
	sub.EffectiveMonth