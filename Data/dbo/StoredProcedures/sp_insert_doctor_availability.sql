CREATE OR REPLACE PROCEDURE sp_insert_doctor_availability(
	IN availabledate date,
	IN dayname character varying,
	IN starthour time,
	IN endhour time,
	IN maxclient int,
	IN doctorid character varying
	)
LANGUAGE plpgsql
AS $$
BEGIN
	INSERT INTO doctoravailability(availabledate, dayname, starthour, endhour, maxclient, doctorid)
	VALUES($1, $2, $3, $4, $5, $6);
END;
$$;
