
SELECT 
	*,
	StatedBalance - CalculatedBalance as UnreconciledAmount
FROM (
	SELECT  
		a.ID AS AccountID, 
		a.Account, 
		IsNull(SUM(t.Amount * tt.ReconciliationMultiplier),0) + IsNull(a.StartingBalance,0) AS CalculatedBalance, 
		a.StatedBalanceAtInstitution as StatedBalance
	FROM         
		dbo.Accounts AS a 
		LEFT JOIN Transactions t ON a.ID = t.AccountID 
		LEFT JOIN TransactionTypes tt ON t.TransactionType = tt.ID
		--exclude splits
		left join transactions ts
			on t.id = ts.ParentTransactionID
	WHERE
		ts.Id Is Null
	GROUP BY 
		a.Account, 
		a.StartingBalance, 
		a.StatedBalanceAtInstitution, 
		a.ID
) sub
ORDER BY
	Account