CREATE DEFINER=`remonty`@`%` PROCEDURE `test_Book_SaveOrUpdateNwmBook`(
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
DECLARE MyId int;
DECLARE CheckExists int;
DECLARE EXIT HANDLER FOR SQLEXCEPTION, SQLWARNING
BEGIN
ROLLBACK;
RESIGNAL;
END;

START TRANSACTION;
SET CheckExists = 0;
SET MyId = p_id;
SELECT COUNT( * ) INTO CheckExists FROM oedigit_cms.test_books WHERE Id = MyId;
IF(CheckExists = 0) THEN
SET SQL_MODE = '';
INSERT INTO oedigit_cms.test_books(
    `title`,
    `author`,
    `year_published`
) VALUES(
    COALESCE(p_title, ''),
    COALESCE(p_author, ''),
    COALESCE(p_year_published, 0)
);

SET MyId = LAST_INSERT_ID();
ELSE
UPDATE oedigit_cms.test_books SET `title` = COALESCE(p_title, ''),
    `author` = COALESCE(p_author, ''),
    `year_published` = COALESCE(p_year_published, 0)
WHERE `id` = MyId;
END IF;
COMMIT;
SELECT MyId;
END