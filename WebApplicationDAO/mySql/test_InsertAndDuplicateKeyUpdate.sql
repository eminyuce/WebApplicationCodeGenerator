CREATE DEFINER=`remonty`@`%` PROCEDURE `test_InsertAndDuplicateKeyUpdate`(
	IN `p_id` int,
	IN `p_title` varchar(45),
	IN `p_author` varchar(45),
	IN `p_year_published` int

)
LANGUAGE SQL
NOT DETERMINISTIC
CONTAINS SQL
SQL SECURITY DEFINER
COMMENT ''
BEGIN
  DECLARE MyId INT DEFAULT NULL;
  DECLARE EXIT HANDLER FOR SQLEXCEPTION, SQLWARNING
  BEGIN
  ROLLBACK;
  RESIGNAL;
  END;

START TRANSACTION;
  SET SQL_MODE = '';
INSERT INTO oedigit_cms.test_books(
`id`,
`title`,
`author`,
`year_published`
) VALUES (
COALESCE(p_id,0),
COALESCE(p_title,''),
COALESCE(p_author,''),
COALESCE(p_year_published,0)
)
ON DUPLICATE KEY UPDATE
`id` = LAST_INSERT_ID(p_id),
`title` = COALESCE(p_title,''),
`author` = COALESCE(p_author,''),
`year_published` = COALESCE(p_year_published,0)
;
 SET MyId = LAST_INSERT_ID();
COMMIT;
 SELECT MyId;
END