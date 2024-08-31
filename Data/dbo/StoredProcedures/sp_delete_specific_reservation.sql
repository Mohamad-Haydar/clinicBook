CREATE OR REPLACE PROCEDURE sp_delete_specific_reservation(
    IN client_reservation_id int
)
LANGUAGE plpgsql
AS $$
DECLARE
    doctor_availability_id int;
    removed_end_time time;
    gap time;
BEGIN 

    -- Delete the row from client reservation table
    DELETE FROM clientreservation
    WHERE id = client_reservation_id
    RETURNING doctoravailabilityid, endtime, endtime - starttime
    INTO doctor_availability_id, removed_end_time, gap;

    -- to reduce 1 from the reservations from the doctoravailability table
    UPDATE doctoravailability
    SET currentreservations = currentreservations - 1
    WHERE id = doctor_availability_id;


    -- reorder all the other reservations in the doctor reservation table
    UPDATE clientreservation
    SET starttime = starttime - gap, endtime = endtime - gap
    WHERE doctoravailabilityid = doctor_availability_id AND starttime >= removed_end_time;

END;
$$;