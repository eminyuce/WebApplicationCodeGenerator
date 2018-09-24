CREATE  PROCEDURE `test_TempTable`()
LANGUAGE SQL
NOT DETERMINISTIC
CONTAINS SQL
SQL SECURITY DEFINER
COMMENT ''
BEGIN

	 DROP TEMPORARY TABLE IF EXISTS TempTable4;
CREATE TEMPORARY TABLE TempTable4 (myid int, myfield varchar(100)); 
		
		INSERT INTO TempTable4 (myid,myfield) VALUES(1,'emin');
 		INSERT INTO TempTable4 (myid,myfield) VALUES(2,'yuce');
 		INSERT INTO TempTable4 (myid,myfield) VALUES(3,'emin 1');
 		INSERT INTO TempTable4 (myid,myfield) VALUES(4,'yuce 2');
 		
 		SELECT * FROM (
 		Select * from TempTable4
 		UNION ALL 
    	Select * from TempTable4
    	) a order by myid desc;
END