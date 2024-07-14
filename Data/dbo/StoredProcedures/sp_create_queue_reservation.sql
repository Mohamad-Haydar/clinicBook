CREATE OR REPLACE PROCEDURE sp_create_queue_reservation(
    IN client_id character varying, 
    IN doctor_availability_id int,
    IN doctor_service_ids int[]
)
LANGUAGE plpgsql
AS $$
DECLARE
	_duration int := 0;
    _reservation_id int;
    _last_end_time timetz;
    _id int;
    _max_client int;
    _current_reservations int;
    _service_duration int;
BEGIN

    -- Check to see if we can afford one more reservation
    SELECT maxclient, currentreservations 
	INTO _max_client, _current_reservations
    FROM doctoravailability
    WHERE doctoravailability.id = doctor_availability_id;

    IF _current_reservations >= _max_client THEN
        RETURN;
    END IF;



    -- to calculate the estimated duration for the services that the user wants
	FOREACH _id IN ARRAY doctor_service_ids LOOP 
        _duration := _duration + 
			(
				SELECT doctorservice.duration 
				FROM doctorservice 
				WHERE doctorservice.id = _id
			);
        -- SELECT doctorservice.duration
        -- INTO _service_duration
        -- FROM doctorservice
        -- WHERE doctorservice.id = _id;

        -- IF NOT FOUND THEN
        --     RETURN;
        -- END IF;

        -- _duration := _duration + _service_duration;
    END LOOP;

    -- to select the last end time for the specific doctor availability
    SELECT Max(endtime) INTO _last_end_time
    FROM clientreservation
    WHERE clientreservation.doctoravailabiltyid = doctor_availability_id;

    IF _last_end_time IS NULL THEN
        SELECT starthour INTO _last_end_time 
        FROM doctoravailability
        WHERe doctoravailability.id = doctor_availability_id;
    END IF;

    -- to insert data in the client reservation
    INSERT INTO clientreservation (starttime, endtime, clientid, doctoravailabiltyid)
    VALUES (_last_end_time, _last_end_time + (_duration * INTERVAL '1 minute'), client_id, doctor_availability_id)
    RETURNING id INTO _reservation_id;


    FOREACH _id IN ARRAY doctor_service_ids
    LOOP 
        INSERT INTO reservationdetail(doctorserviceid, clientreservationid)
        VALUES(_id, _reservation_id);
    END LOOP;

    UPDATE doctoravailability
    SET currentreservations = _current_reservations + 1
    WHERE doctoravailability.id = doctor_availability_id;
	
END;
$$;
