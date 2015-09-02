﻿
SELECT  
	a.ID AS AccountID, 
	a.Account, 
	IsNull(SUM(t.Amount * tt.ReconciliationMultiplier),0) + IsNull(a.StartingBalance,0) AS CalculatedBalance, 
    a.StatedBalanceAtInstitution as StatedBalance,
	a.StatedBalanceAtInstitution - IsNull(SUM(t.Amount * tt.ReconciliationMultiplier),0) + IsNull(a.StartingBalance,0) AS UnreconciledAmount
FROM         
	dbo.Accounts AS a 
	LEFT JOIN Transactions t ON a.ID = t.AccountID 
	LEFT JOIN TransactionTypes tt ON t.TransactionType = tt.ID
GROUP BY 
	a.Account, 
	a.StartingBalance, 
	a.StatedBalanceAtInstitution, 
	a.ID
ORDER BY
	a.Account