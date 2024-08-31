CREATE OR REPLACE FUNCTION f_get_all_personal_reservations(
    IN client_id character varying
)
RETURNS SETOF reservation_detail
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT 
        cr.id, 
        da.availabledate,
        cr.starttime, 
        cr.endtime, 
        cr.doctoravailabilityid, 
        cr.isdone,
        array_agg(ds.servicename) AS service_names,
        ds.doctorid
    FROM clientreservation AS cr
    JOIN reservationdetail rd ON cr.id = rd.clientreservationid
    JOIN doctorservice AS ds ON ds.id = rd.doctorserviceid
    JOIN doctoravailability AS da ON da.id = cr.doctoravailabilityid
    WHERE cr.id IN (
        SELECT cr.id
        FROM clientreservation AS cr
        JOIN client AS c ON c.id = cr.clientid
        WHERE c.id = client_id
    )
    GROUP BY cr.id, cr.starttime, cr.endtime, cr.doctoravailabilityid, ds.doctorid, da.availabledate
    ORDER BY da.availabledate;
END;
$$;