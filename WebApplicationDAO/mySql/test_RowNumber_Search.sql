CREATE DEFINER=`remonty`@`%` PROCEDURE `test_RowNumber_Search`(
	IN `rowNumber` int,
	IN `searchKey` nvarchar(500)
)
LANGUAGE SQL
NOT DETERMINISTIC
CONTAINS SQL
SQL SECURITY DEFINER
COMMENT ''
BEGIN

DECLARE row_number INT;
DECLARE ltSearchKey   nvarchar(500);

 
    set searchKey=LOWER(TRIM(searchKey));
    SET ltSearchKey = CONCAT('%',searchKey,'%');
         
        SET row_number=0;
    
   

select * from (
SELECT id,title,@row_number:=@row_number+1 AS row_number  
FROM oedigi_k2_items where  title LIKE ltSearchKey

) T
where row_number=CASE WHEN rowNumber=0 THEN row_number ELSE rowNumber END 
;
 select ltSearchKey;



END