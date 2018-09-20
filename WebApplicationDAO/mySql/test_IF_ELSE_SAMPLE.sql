CREATE DEFINER=`remonty`@`%` PROCEDURE `test_IF_ELSE_SAMPLE`(
	IN `p_title` VARCHAR(50),
	IN `p_catId` INT,
	INOUT `resultado` INT

)
LANGUAGE SQL
NOT DETERMINISTIC
CONTAINS SQL
SQL SECURITY DEFINER
COMMENT ''
begin 

    if exists (select * from oedigi_k2_items where title = p_title and catid = p_catId) then
        set resultado = 0;
    elseif exists (select * from oedigi_k2_items where title = p_title) then
        set resultado = -1;
    else 
        set resultado = -2;
    end if;
end