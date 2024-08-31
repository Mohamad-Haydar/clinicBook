CREATE OR REPLACE PROCEDURE sp_update_specific_reservation(
    IN client_reservation_id int,
    IN doctor_service_ids int[]
)
LANGUAGE plpgsql
AS $$
DECLARE
    _id int;
    _duration int := 0;
    old_duration int;
    doctor_availability_id int;
    removed_end_time time;
    _service_duration int;
    _gap time;
BEGIN 
    --check if the reservation exists or the user should create new one
    SELECT extract(epoch from ( endtime - starttime )) / 60 AS minutes_difference, doctoravailabilityid
	INTO old_duration, doctor_availability_id
	FROM clientreservation AS cr
	WHERE cr.id = client_reservation_id;
	IF NOT FOUND THEN
	    RAISE EXCEPTION 'ليس لديك حجز, الرجاء حجز موعد.' USING ERRCODE = 'P0001';
	END IF;

     -- to calculate the estimated duration for the services that the user wants
	FOREACH _id IN ARRAY doctor_service_ids LOOP 
        SELECT doctorservice.duration
        INTO _service_duration
        FROM doctorservice
        WHERE doctorservice.id = _id;

        IF NOT FOUND THEN
            RAISE EXCEPTION 'لقد اخترت خدمة غير موجودة, الرجاء التأكد من الاختيار.' USING ERRCODE = 'P0001';
        END IF;

        _duration := _duration + _service_duration;
    END LOOP;

    IF _duration = 0 THEN
        RAISE EXCEPTION 'يجب ان تختار خدمة واحدة على الاقل.' USING ERRCODE = 'P0001';
    END IF;

    -- Delete the row from client reservation table
    UPDATE clientreservation
    SET endtime = starttime + (_duration * INTERVAL '1 minute')
    WHERE id = client_reservation_id
    RETURNING starttime + old_duration * INTERVAL '1 minute'
    INTO removed_end_time;

    -- reorder all the other reservations in the doctor reservation table
    UPDATE clientreservation
    SET starttime = starttime + (_duration - old_duration) * INTERVAL '1 minute', 
    endtime = endtime + (_duration - old_duration) * INTERVAL '1 minute'
    WHERE doctoravailabilityid = doctor_availability_id AND starttime >= removed_end_time;

    -- Update the join table to remove services not in the new list
    DELETE FROM reservationdetail
    WHERE clientreservationid = client_reservation_id;


    -- Insert new services into the join table
    FOREACH _id IN ARRAY doctor_service_ids LOOP 
        INSERT INTO reservationdetail(doctorserviceid, clientreservationid)
        VALUES(_id, client_reservation_id);
    END LOOP;

END;
$$;