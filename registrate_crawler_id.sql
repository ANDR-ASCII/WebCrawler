CREATE PROCEDURE `webcrawler`.`registrate_crawler_id` (IN Id INT)
BEGIN
	IF
	(SELECT COUNT(*) FROM crawler_id WHERE id=Id) = NULL
	THEN
    INSERT INTO crawler_id (id) VALUES(Id);
    END IF;
END