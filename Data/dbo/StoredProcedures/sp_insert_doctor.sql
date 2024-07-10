-- ≈, category_id
CREATE OR REPLACE PROCEDURE sp_insert_doctor(
	IN first_name character varying,
	IN last_name character varying,
	IN email character varying,
	IN phone_number character varying,
	IN description text,
	IN category_id int)
LANGUAGE plpgsql
AS $$
BEGIN
	INSERT INTO doctor(first_name, last_name, email, phone_number, description, category_id)
	VALUES($1, $2, $3, $4, $5, $6);
END;
$$;
