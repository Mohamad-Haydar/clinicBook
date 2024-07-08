CREATE OR REPLACE PROCEDURE sp_insert_doctor(
	IN first_name character varying,
	IN last_name character varying,
	IN speciality character varying,
	IN email character varying,
	IN phone_number character varying)
AS $$
	INSERT INTO doctor(first_name, last_name, speciality, email, phone_number)
	VALUES($1, $2, $3, $4, $5);
END;
$$ LANGUAGE plpgsql;
