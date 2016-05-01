--DECLARE @month int
--DECLARE @year int
--SET @month = 8
--SET @year = 2015

SELECT
	IsNull(NullIF(COALESCE(t.Category,b.Category),''),'[Uncategorized]') as Category,
	ISNULL(ABS(SUM(t.Amount * tt.MonthlyCashflowMultiplier)),0) as Spent,
	IsNull(b.BudgetedAmount,0) as Budgeted
FROM
	Transactions t
	FULL JOIN Budget b ON t.Category = b.Category
	LEFT JOIN TransactionTypes tt on t.TransactionType = tt.ID
	--exclude splits
	left join transactions ts
		on t.id = ts.ParentTransactionID
WHERE
	(t.FlagForFollowUp = 0 or t.FlagForFollowUp is null)
	AND t.ReimbursableSource is null
	AND (DatePart(m,t.TransactionDate) = @month OR t.TransactionDate is null)
	AND (DatePart(yyyy,t.TransactionDate) = @year OR t.TransactionDate is null)
	AND (tt.CalcInCategoryOverview = 1 OR tt.CalcInCategoryOverview is null)
	and ts.id is null --exclude split transactions
GROUP BY
	COALESCE(t.Category,b.Category),
	b.BudgetedAmount
HAVING 
	ISNULL(ABS(SUM(t.Amount * tt.MonthlyCashflowMultiplier)),0) !=0 OR IsNull(b.BudgetedAmount,0) !=0
ORDER BY 
	IsNull(NullIF(COALESCE(t.Category,b.Category),''),'[Uncategorized]')