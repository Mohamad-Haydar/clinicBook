CREATE OR REPLACE FUNCTION f_get_concurrent_reservations(IN client_reservation_id int) 
    RETURNS SETOF clientreservation
    LANGUAGE plpgsql
AS $$
DECLARE
    res reservation_detail;
BEGIN
    RETURN QUERY
    WITH target_doctoravailability AS (
        SELECT doctoravailabilityid
        FROM clientreservation
        WHERE id = _id
    )
    SELECT cr.*
    FROM clientreservation cr
    JOIN target_doctoravailability tda ON cr.doctoravailabilityid = tda.doctoravailabilityid;
END;
$$;