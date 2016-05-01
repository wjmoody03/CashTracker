select t.ID, t.Description, t.TransactionDate, t.amount * tt.MonthlyCashflowMultiplier as Amount, 
	splits.SplitAmount, (t.Amount*tt.MonthlyCashflowMultiplier)-splits.SplitAmount as Diff
from transactions t
	join transactiontypes tt on t.transactiontype = tt.ID
	join (
		select ParentTransactionID, sum(t2.Amount * tt2.MonthlyCashflowMultiplier) as SplitAmount
		from transactions t2
			join transactiontypes tt2 on t2.transactiontype = tt2.ID
		where parentTransactionID Is Not null
		group by ParentTransactionID
	) splits on t.ID = splits.ParentTransactionID
where (t.Amount*tt.MonthlyCashflowMultiplier)-splits.SplitAmount !=0
