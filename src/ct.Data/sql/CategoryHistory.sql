--declare @startdate smalldatetime
--declare @enddate smalldatetime

--set @startdate = '1/10/15'
--set @enddate = '8/15/15'

--now come up with a date range that is beginning of one month to the end of another
declare @tempStartDate smalldatetime = CAST(CAST(DatePart(yyyy,@startDate) as varchar(4)) + '-' + CAST(DatePart(m,@startDate) as varchar(2)) + '-01' as smalldatetime)
--we want temp end date to be the last day of the month in which then enddate occurs
declare @tempEndDate smallDateTime = @endDate
set @tempEndDate = DateAdd(m,1,@endDate) --add a month
set @tempEndDate = CAST(CAST(DatePart(yyyy,@tempEndDate) as varchar(4)) + '-' + CAST(DatePart(m,@tempEndDate) as varchar(2)) + '-01' as smalldatetime) --get the beginning of the month
set @tempEndDate = DateAdd(d,-1,@tempEndDate) --subtract a day

--we need to ensure that any category found in the transactions table has a value for every month in scope, even if it's a zero
--so first we need a list of all months in scope
declare @months table (monthdate smalldatetime)
declare @totalMonths int = DateDiff(m,@startdate, @enddate)
insert into @months 
select DateAdd(m,i.id,@tempStartDate)
from integers i
where i.id <= @totalMonths

--this is for optimization only... not the simplest solution, but very fast
declare @transactions table (monthdate smalldatetime, category varchar(150), amount money, primary key (monthdate,category))
insert into @transactions
select 
	CAST(CAST(DatePart(yyyy,t.TransactionDate) as varchar(4)) + '-' + CAST(DatePart(m,t.TransactionDate) as varchar(2)) + '-01' as smalldatetime) as monthdate,
	IsNull(NullIf(LTRIM(RTRIM(t.Category)),''),'[Unknown]') as category,
	sum(t.Amount * tt.MonthlyCashflowMultiplier) as amount
from
	transactions t
	join transactiontypes tt
		on t.transactiontype = tt.ID
	--exclude splits
	left join transactions ts
		on t.id = ts.ParentTransactionID
where 
	t.transactiondate between @tempStartDate and @tempEndDate
	and t.FlagForFollowUp = 0
	and nullif(ltrim(rtrim(t.reimbursablesource)),'') is null
	and ts.id is null --exclude split transactions
group by 
	CAST(CAST(DatePart(yyyy,t.TransactionDate) as varchar(4)) + '-' + CAST(DatePart(m,t.TransactionDate) as varchar(2)) + '-01' as smalldatetime),
	IsNull(NullIf(LTRIM(RTRIM(t.Category)),''),'[Unknown]')

--now we need all potential categories in scope
declare @categories table (category varchar(150) primary key)
insert into @categories
select DISTINCT category
from @transactions	

--now create the return dataset
select
	m.monthdate as MonthDate,
	c.category,
	Abs(IsNull(sum(t.Amount),0)) as Total
from
	@months m
	cross join @categories c
	left join @transactions t
		on m.monthdate = t.monthdate
		and c.category = t.category
group by
	m.monthdate,
	c.category
