CREATE OR REPLACE PROCEDURE sp_insert_doctor_availability(
	IN available_date date,
	IN day_name character varying,
	IN start_hour timetz,
	IN end_hour timetz,
	IN max_client int,
	IN doctor_id int
	)
LANGUAGE plpgsql
AS $$
BEGIN
	INSERT INTO doctor_availability(available_date, day_name, start_hour, end_hour, max_client, doctor_id)
	VALUES($1, $2, $3, $4, $5, $6);
END;
$$;
