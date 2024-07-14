CREATE OR REPLACE FUNCTION get_all_personal_reservations(
    IN client_id character varying
)
RETURNS SETOF reservation_detail
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT 
        cr.id, 
        cr.starttime, 
        cr.endtime, 
        cr.doctoravailabiltyid, 
        array_agg(ds.servicename) AS service_names,
        ds.doctorid
    FROM clientreservation AS cr
    JOIN reservationdetail rd ON cr.id = rd.clientreservationid
    JOIN doctorservice AS ds ON ds.id = rd.doctorserviceid
    WHERE cr.id IN (
        SELECT cr.id
        FROM clientreservation AS cr
        JOIN client AS c ON c.id = cr.clientid
        WHERE c.id = client_id
    )
    GROUP BY cr.id, cr.starttime, cr.endtime, cr.doctoravailabiltyid, ds.doctorid;
END;
$$;