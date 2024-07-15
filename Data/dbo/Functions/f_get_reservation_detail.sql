CREATE OR REPLACE FUNCTION f_get_reservation_detail(IN _id int) -- this id is the client reservation id
    RETURNS reservation_detail
    LANGUAGE plpgsql
AS $$
DECLARE
    res reservation_detail;
BEGIN
    SELECT cr.id, cr.starttime, cr.endtime, cr.doctoravailabilityid, ds.doctorid, array_agg(ds.servicename) 
    INTO res.id, res.start_time, res.end_time, res.doctor_availability_id, res.doctor_id, res.service_names
    FROM clientreservation as cr
    JOIN reservationdetail rd ON cr.id = rd.clientreservationid
    JOIN doctorservice as ds ON ds.id = rd.doctorserviceid
    WHERE cr.id = _id
	GROUP BY cr.id, ds.doctorid;

	RETURN res;
    
END;
$$;