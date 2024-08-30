CREATE OR REPLACE PROCEDURE sp_remove_doctor_availability(IN id int)
LANGUAGE plpgsql
AS $$
DECLARE
    nb_of_availability_after_the_deleted INT;
    availability_id INT := id;
    last_availability_date DATE;
    doctor_availability_row RECORD;
    future_availability RECORD;
    reservation_row clientreservation;
    old_reservations clientreservation[];
    all_services services_array[];
    oneservice services_array;
    doctor_service_id INT;
    reservation_services INT[];
BEGIN
    -- STEP 2: Get the doctor id and other details
    SELECT *
    INTO doctor_availability_row
    FROM doctoravailability
    WHERE doctoravailability.id = availability_id;

    --STEP 3: Find the next available slot or raise an error if none is found
    SELECT * INTO future_availability 
    FROM doctoravailability
    WHERE availabledate > doctor_availability_row.availabledate
    AND doctorid = doctor_availability_row.doctorid 
    ORDER BY availabledate LIMIT 1;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'MYERROR:لا يوجد مواعيد متاحة كافية, الرجاء اتاحة موعد جديد لنقل الحجوزات بنجاح';
        ROLLBACK;
        RETURN;
    END IF;

    UPDATE doctoravailability SET currentreservations = 0
    WHERE  doctoravailability.id = future_availability.id;

    future_availability.currentreservations := 0;

    -- STEP 4: select curent reservations into client_reservation table
    FOR reservation_row IN
        SELECT cr.id, cr.starttime, cr.endtime, cr.clientid, cr.doctoravailabilityid, cr.isdone 
        FROM doctoravailability da
        JOIN clientreservation cr ON cr.doctoravailabilityid = da.id
        WHERE availabledate >= doctor_availability_row.availabledate
        AND doctorid = doctor_availability_row.doctorid
        ORDER BY da.availabledate, cr.starttime
    LOOP
        -- Get the services of every reservation and append them in an array
        FOR doctor_service_id IN 
            SELECT doctorserviceid 
            FROM reservationdetail
            WHERE clientreservationid = reservation_row.id
        LOOP
            reservation_services := array_append(reservation_services, doctor_service_id);
        END LOOP;

        old_reservations := array_append(old_reservations, reservation_row);
        oneservice.reservation_services = reservation_services;
        all_services := array_append(all_services, oneservice);

        UPDATE doctoravailability SET currentreservations = 0
        WHERE  doctoravailability.id = reservation_row.doctoravailabilityid;

        DELETE FROM clientreservation as cr2
        WHERE cr2.id = reservation_row.id;
    END LOOP;

    DELETE FROM doctoravailability
    WHERE doctoravailability.id = availability_id;


    -- STEP 7: Loop over old client reservations and add them as needed
    FOR i IN 1 .. array_length(old_reservations, 1) LOOP
            reservation_row := old_reservations[i];
            reservation_services := all_services[i].reservation_services;
        
            IF array_length(reservation_services, 1) IS NULL THEN
                RAISE NOTICE 'thier is no services'; 
            END IF;

            IF future_availability.maxclient = future_availability.currentreservations THEN
                SELECT * INTO future_availability 
                FROM doctoravailability
                WHERE availabledate > future_availability.availabledate
                AND doctorid = future_availability.doctorid 
                ORDER BY availabledate LIMIT 1;

                IF NOT FOUND THEN
                    RAISE EXCEPTION 'MYERROR:لا يوجد مواعيد متاحة كافية, الرجاء اتاحة موعد جديد لنقل الحجوزات بنجاح';
                    ROLLBACK;
                    RETURN;
                END IF;
            END IF;

            -- Assign the reservation to the found slot
            CALL sp_create_queue_reservation(reservation_row.clientid, future_availability.id, reservation_services, true);
            future_availability.currentreservations := future_availability.currentreservations + 1;
    END LOOP;
END;
$$;
