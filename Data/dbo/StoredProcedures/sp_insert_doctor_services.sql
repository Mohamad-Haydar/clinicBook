CREATE OR REPLACE PROCEDURE sp_insert_doctor_services(
	IN service_name character varying,
	IN price money,
	IN duration int,
	IN doctor_id int
	)
AS $$
	INSERT INTO doctor_service(service_name, price, duration, doctor_id)
	VALUES($1, $2, $3, $4);
END;
$$ LANGUAGE plpgsql;
