CREATE DEFINER=`remonty`@`%` PROCEDURE `test_PagingProcedure_Sample_Temp_Table`(
	IN `paramFrom` INT,
	IN `paramTo` INT,
	IN `Search` nvarchar(50)
  



)
LANGUAGE SQL
NOT DETERMINISTIC
CONTAINS SQL
SQL SECURITY DEFINER
COMMENT ''
BEGIN
       DECLARE valFrom INT;
       DECLARE valTo   INT;

       SET valFrom = paramFrom;
       SET valTo = paramTo;
       
         SET @vardomain := CONCAT('%',LOWER(TRIM(Search)),'%');
         
 
 
 
	 DROP TEMPORARY TABLE IF EXISTS temp_table4;
       CREATE TEMPORARY TABLE IF NOT EXISTS 
			temp_table4 ( INDEX(id) ) 
			ENGINE=MyISAM 
			AS (
			  SELECT id, title, id*25 NewColumn, id*100 NewColumn2
			  FROM oedigi_k2_items
			  Where title LIKE @vardomain 
              order by id desc
			);
	-- select * from temp_table4;
	select * from oedigi_k2_items a
    INNER JOIN temp_table4 t ON a.id=t.id
    LIMIT valFrom, valTo;
    
    
    -- Skip 20 rows and return only the next 10 rows from the sorted result set
SELECT *
FROM oedigi_k2_items
ORDER BY id desc
LIMIT 10
OFFSET 20;



 END