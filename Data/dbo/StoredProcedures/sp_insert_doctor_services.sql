CREATE OR REPLACE PROCEDURE sp_insert_doctor_services(
	IN duration int,
	IN doctor_id character varying,
	IN service_id int
	)
LANGUAGE plpgsql
AS $$
BEGIN
	INSERT INTO doctor_service(duration, doctor_id, service_id)
	VALUES($1, $2, $3);
END;
$$;
