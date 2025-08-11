DECLARE @TableName NVARCHAR(512);
DECLARE @SQL NVARCHAR(MAX);
DECLARE @TotalRows BIGINT = 0;

-- Temporary table to hold row counts
IF OBJECT_ID('tempdb..#RowCounts') IS NOT NULL DROP TABLE #RowCounts;
CREATE TABLE #RowCounts (
    TableName NVARCHAR(512),
    [RowCount] BIGINT
);

-- Cursor to go through each table
DECLARE TableCursor CURSOR FOR
SELECT QUOTENAME(s.name) + '.' + QUOTENAME(t.name)
FROM sys.tables t
JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE t.is_ms_shipped = 0;

OPEN TableCursor;
FETCH NEXT FROM TableCursor INTO @TableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @SQL = '
        INSERT INTO #RowCounts (TableName, [RowCount])
        SELECT ''' + @TableName + ''', COUNT(*) FROM ' + @TableName;

    EXEC sp_executesql @SQL;

    FETCH NEXT FROM TableCursor INTO @TableName;
END

CLOSE TableCursor;
DEALLOCATE TableCursor;

-- Show individual table row counts
SELECT REPLACE(REPLACE(TableName,'[',''),']','') AS TableName, [RowCount] FROM #RowCounts 
ORDER BY TableName;

-- Show total row count
SELECT SUM([RowCount]) AS TotalRowCount FROM #RowCounts;
