CREATE DEFINER=`remonty`@`%` PROCEDURE `test_StringFunctions`()
LANGUAGE SQL
NOT DETERMINISTIC
CONTAINS SQL
SQL SECURITY DEFINER
COMMENT ''
BEGIN
		SELECT  LOWER('GEEKSFORGEEKS') FROM oedigi_k2_categories   limit 1;
			SELECT LOWER('DATABASE@456') FROM oedigi_k2_categories   limit 1;
			SELECT UPPER('geeksforgeeks') FROM oedigi_k2_categories   limit 1;
	
 
	 SELECT UPPER('dbms$508%7') FROM oedigi_k2_categories   limit 1;
 	--	 SELECT INITCAP('geeksforgeeks is a computer science portal for geeks') FROM oedigi_k2_categories   limit 1;
 	--	 SELECT INITCAP('PRACTICE_CODING_FOR_EFFICIENCY') FROM oedigi_k2_categories   limit 1;
			SELECT CONCAT('computer' ,'science') FROM oedigi_k2_categories   limit 1;
SELECT CONCAT( NULL ,'Android') FROM oedigi_k2_categories   limit 1;
SELECT CONCAT( NULL ,NULL ) FROM oedigi_k2_categories   limit 1;
SELECT LENGTH('Learning Is Fun') FROM oedigi_k2_categories   limit 1;
SELECT LENGTH('   Write an Interview  Experience ') FROM oedigi_k2_categories   limit 1;
SELECT LENGTH('') FROM oedigi_k2_categories   limit 1;
            



 SELECT LENGTH( NULL ) FROM oedigi_k2_categories   limit 1;
SELECT LENGTH('') FROM oedigi_k2_categories   limit 1; 
 SELECT LENGTH( NULL ) FROM oedigi_k2_categories   limit 1;
SELECT SUBSTR('Database Management System', 9) FROM oedigi_k2_categories   limit 1;
SELECT SUBSTR('Database Management System', 9, 7) FROM oedigi_k2_categories   limit 1;
 


SELECT LPAD('100',5,'*') FROM oedigi_k2_categories   limit 1;
SELECT LPAD('hello', 21, 'geek') FROM oedigi_k2_categories   limit 1;
SELECT RPAD('5000',7,'*') FROM oedigi_k2_categories   limit 1;


SELECT RPAD('earn', 19, 'money') FROM oedigi_k2_categories   limit 1;

SELECT TRIM('G' FROM 'GEEKS') FROM oedigi_k2_categories   limit 1;


SELECT TRIM('        geeksforgeeks   ') FROM oedigi_k2_categories   limit 1; 

SELECT REPLACE('DATA MANAGEMENT', 'DATA','DATABASE') FROM oedigi_k2_categories   limit 1;


END