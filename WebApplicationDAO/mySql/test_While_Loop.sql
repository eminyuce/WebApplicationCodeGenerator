CREATE DEFINER=`remonty`@`%` PROCEDURE `test_While_Loop`()
LANGUAGE SQL
NOT DETERMINISTIC
CONTAINS SQL
SQL SECURITY DEFINER
COMMENT ''
BEGIN
 SELECT MONTH(CURDATE()) INTO @curmonth;
   SELECT MONTHNAME(CURDATE()) INTO @curmonthname;
   SELECT DAY(LAST_DAY(CURDATE())) INTO @totaldays;
   SELECT FIRST_DAY(CURDATE()) INTO @checkweekday;
   SELECT DAY(@checkweekday) INTO @checkday;
   SET @daycount = 0;
   SET @workdays = 0;

     WHILE(@daycount < @totaldays) DO
       IF (WEEKDAY(@checkweekday) < 5) THEN
         SET @workdays = @workdays+1;
       END IF;
       SET @daycount = @daycount+1;
       SELECT ADDDATE(@checkweekday, INTERVAL 1 DAY) INTO @checkweekday;
     END WHILE;
END