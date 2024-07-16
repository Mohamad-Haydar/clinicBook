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
    SELECT extract(epoch from ( endtime - starttime )) as "minutes", doctoravailabilityid
	INTO old_duration, doctor_availability_id
	FROM clientreservation AS cr
	WHERE cr.id = client_reservation_id;
	IF NOT FOUND THEN
	    RAISE EXCEPTION 'You do not have a reservation, please go to book a reservation';
	END IF;
    -- Output the removed_end_time for testing
    RAISE NOTICE 'old_duration: %', old_duration;

     -- to calculate the estimated duration for the services that the user wants
	FOREACH _id IN ARRAY doctor_service_ids LOOP 
        SELECT doctorservice.duration
        INTO _service_duration
        FROM doctorservice
        WHERE doctorservice.id = _id;

        IF NOT FOUND THEN
            RAISE EXCEPTION 'You Selected Service That is Not Found Please check your selection';
        END IF;

        _duration := _duration + _service_duration;
    END LOOP;

    IF _duration = 0 THEN
        RAISE EXCEPTION 'You Need to select at least one service';
    END IF;

    -- Delete the row from client reservation table
    UPDATE clientreservation
    SET endtime = starttime + (_duration * INTERVAL '1 minute')
    WHERE id = client_reservation_id
    RETURNING starttime + old_duration * INTERVAL '1 minute'
    INTO removed_end_time;

    -- Output the removed_end_time for testing
    RAISE NOTICE 'Removed End Time: %', removed_end_time;

    -- reorder all the other reservations in the doctor reservation table
    UPDATE clientreservation
    SET starttime = starttime + (_duration - old_duration) * INTERVAL '1 minute', 
    endtime = endtime + (_duration - old_duration) * INTERVAL '1 minute'
    WHERE doctoravailabilityid = doctor_availability_id AND starttime >= removed_end_time;

    -- Output the result for testing
    RAISE NOTICE 'Reordered reservations for doctor availability ID: %', doctor_availability_id;


END;
$$;