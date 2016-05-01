--this averages the recency/frequency rank and returns the lowest avg
SELECT Description, Category
FROM 
(SELECT
	Description,
	Category,
	RANK() OVER(Partition By Description ORDER BY TotalScore) as TotalScoreRank
FROM 
	(
		--this calcs ranks based on frequency and recency
		SELECT
			Description,
			Category,
			total,
			DENSE_RANK() OVER(Partition By Description ORDER BY MostRecent DESC) --as RecencyRank
			 + DENSE_RANK() OVER (Partition BY Description ORDER BY total DESC) --FrequencyRank 
			 as TotalScore
		FROM
			(
				--This calcs frequency of each category and last time it was used
				SELECT
					t.Description,
					t.Category,
					Count(*) as Total,
					Max(t.TransactionDate) as MostRecent
				FROM 
					Transactions t
					--exclude splits
					left join transactions ts
						on t.id = ts.ParentTransactionID
				WHERE
					NullIf(LTRIM(RTRIM(t.Category)),'') is not null
					AND t.Description NOT LIKE 'check%'
					AND ts.Id Is Null
				GROUP BY 
					t.Description,
					t.Category
			) sub
	) score
) rnk
WHERE TotalScoreRank =1