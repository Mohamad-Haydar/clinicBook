-- ≈, category_id
CREATE OR REPLACE PROCEDURE sp_insert_doctor(
	IN id character varying,
	IN firstname character varying,
	IN lastname character varying,
	IN email character varying,
	IN phonenumber character varying,
	IN description text,
	IN categoryid int)
LANGUAGE plpgsql
AS $$
BEGIN
	INSERT INTO doctor(firstname, lastname, email, phonenumber, description, categoryid)
	VALUES($1, $2, $3, $4, $5, $6);
END;
$$;
