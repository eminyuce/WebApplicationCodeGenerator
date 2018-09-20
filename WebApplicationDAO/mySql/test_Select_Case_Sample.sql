CREATE DEFINER=`remonty`@`%` PROCEDURE `test_Select_Case_Sample`()
LANGUAGE SQL
NOT DETERMINISTIC
CONTAINS SQL
SQL SECURITY DEFINER
COMMENT ''
BEGIN
SELECT id, title,
CASE
    WHEN id % 5 = 0 THEN "Mode for 5 is 0"
    WHEN id % 5 = 1 THEN "Mode for 5 is 1"
    WHEN id % 5 = 2 THEN "Mode for 5 is 2"
    WHEN id % 5 = 3 THEN "Mode for 5 is 3"
    ELSE "ELSE CASE:Mode for 5 is 4"
END as IdDesc
,IF(id % 5 > 2, 100, id) AS newId
FROM oedigit_cms.oedigi_k2_items order by id desc limit 100;


SELECT id,CASE id % 5
WHEN 0 THEN 'Zombie'
WHEN 1 THEN 'Human'
ELSE 'Alien'
END AS race
FROM oedigit_cms.oedigi_k2_items order by id desc limit 100;


END