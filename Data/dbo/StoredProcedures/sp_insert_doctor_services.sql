CREATE OR REPLACE PROCEDURE sp_insert_doctor_service(
	IN duration int,
	IN doctorid character varying,
	IN serviceid int
	)
LANGUAGE plpgsql
AS $$
DECLARE
	service_name VARCHAR(50);
BEGIN  
	Select servicename INTO service_name  FROM service WHERE service.id = serviceid;
  
	IF service_name IS NULL THEN
		RAISE EXCEPTION 'Invalid service ID';
	END IF;
  
  -- Insert data into doctorservice table
  INSERT INTO doctorservice(servicename, duration, doctorid, serviceid)
  VALUES (service_name, duration, doctorid, serviceid);
END;
$$;
