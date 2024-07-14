CREATE OR REPLACE FUNCTION f_get_previous_reservations( IN client_reservation_id int)
RETURNS SETOF clientreservation
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    WITH target_client_reservation AS (
        SELECT *
        FROM clientreservation
        WHERE id = client_reservation_id
    )
    SELECT cr.*
    FROM clientreservation cr
    JOIN target_client_reservation tcr ON cr.doctoravailabilityid = tcr.doctoravailabilityid
    WHERE cr.endtime <= tcr.starttime;
END;
$$;