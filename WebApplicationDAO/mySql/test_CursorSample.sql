CREATE DEFINER=`remonty`@`%` PROCEDURE `test_CursorSample`()
LANGUAGE SQL
NOT DETERMINISTIC
CONTAINS SQL
SQL SECURITY DEFINER
COMMENT ''
BEGIN
    DECLARE userid BIGINT;  
    DECLARE done INT DEFAULT 0;
    DECLARE cur CURSOR FOR SELECT id FROM oedigi_users LIMIT 100;
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;
    OPEN cur;
    read_loop: LOOP 
    FETCH cur INTO userid;
        IF done THEN
            LEAVE read_loop;
        END IF;
        select userid; -- INSERT INTO points (iduser, points, pointcat) VALUES (uid, -1, 1), (userid, -1, 2), (userid, -1, 3), (userid, -1, 4), (userid, -1, 5), (userid, -1, 6);
    END LOOP;
    CLOSE cur;
END