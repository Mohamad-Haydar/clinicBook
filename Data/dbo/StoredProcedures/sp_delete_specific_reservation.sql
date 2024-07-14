CREATE OR REPLACE PROCEDURE sp_delete_specific_reservation(
    IN client_reservation_id int
)
LANGUAGE plpgsql
AS $$
DECLARE
    doctor_availability_id int;
BEGIN 
    DELETE FROM clientreservation
    WHERE id = client_reservation_id
    RETURNING doctoravailabilityid INTO doctor_availability_id;

    IF doctor_availability_id IS NULL THEN
        RAISE EXCEPTION 'Reservation with ID % does not exist', client_reservation_id;
    END IF;

    UPDATE doctoravailability
    SET currentreservations = currentreservations - 1
    WHERE id = doctor_availability_id;
END;
$$;