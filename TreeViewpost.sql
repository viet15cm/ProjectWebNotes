DECLARE @Id nvarchar(450) = 'ads';
WITH #results
AS (SELECT p.Id,
           p.Slug,
           p.PostParentId
    FROM dbo.Posts p
    WHERE p.Id = @Id
    UNION ALL
    SELECT t.Id,
           t.Slug,
           t.PostParentId
    FROM dbo.Posts t
        INNER JOIN #results r
            ON r.id = t.PostParentId)
SELECT *
FROM #results
ORDER BY Id;