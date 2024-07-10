CREATE OR REPLACE PROCEDURE sp_insert_doctor_services(
	IN duration int,
	IN doctorid character varying,
	IN serviceid int
	)
LANGUAGE plpgsql
AS $$
BEGIN
	INSERT INTO doctorservice(duration, doctorid, serviceid)
	VALUES($1, $2, $3);
END;
$$;
