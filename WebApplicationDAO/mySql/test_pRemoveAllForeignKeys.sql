CREATE DEFINER=`remonty`@`%` PROCEDURE `test_pRemoveAllForeignKeys`()
LANGUAGE SQL
NOT DETERMINISTIC
CONTAINS SQL
SQL SECURITY DEFINER
COMMENT ''
BEGIN

DECLARE sName TEXT;
DECLARE cName TEXT;
DECLARE tName TEXT;
DECLARE done INT DEFAULT 0;
DECLARE cur CURSOR FOR 
    SELECT TABLE_SCHEMA, CONSTRAINT_NAME, TABLE_NAME
        FROM information_schema.key_column_usage
       WHERE
       -- REFERENCED_TABLE_SCHEMA IS NOT NULL AND
          TABLE_SCHEMA =   'aogdigit_cms' -- use this line to limit the results to one schema
        LIMIT 10  -- use this the first time because it might make you nervous to run it all at once.
        ;
DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;

OPEN cur;
read_loop: LOOP 
FETCH cur INTO sName, cName, tName;
    IF done THEN
        LEAVE read_loop;
    END IF;
    SET @s = CONCAT('ALTER TABLE ', COALESCE(sName, ''), '.', COALESCE(tName, '') , ' DROP FOREIGN KEY ',COALESCE(cName, '')  );
       SELECT @s; -- uncomment this if you want to see the command being sent
  --  PREPARE stmt FROM @s;
  --  EXECUTE stmt;
END LOOP;
CLOSE cur;
   SELECT @s; 
-- deallocate prepare stmt;
END