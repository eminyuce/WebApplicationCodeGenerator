CREATE DEFINER=`remonty`@`%` PROCEDURE `test_PagingProcedure_Sample2`(
	IN `p_Take` INT,
	IN `p_Page` INT

)
LANGUAGE SQL
NOT DETERMINISTIC
CONTAINS SQL
SQL SECURITY DEFINER
COMMENT ''
BEGIN

DECLARE p_OffSet INT DEFAULT NULL;
SET p_OffSet=  p_Take * p_Page;
    -- Skip 20 rows and return only the next 10 rows from the sorted result set
SELECT *
FROM oedigi_k2_items
ORDER BY id desc
LIMIT p_Take
OFFSET p_OffSet;

END