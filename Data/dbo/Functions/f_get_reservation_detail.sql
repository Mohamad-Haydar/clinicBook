CREATE OR REPLACE FUNCTION f_get_reservation_detail(IN _id int) -- this id is the client reservation id
    RETURNS reservation_detail
    LANGUAGE plpgsql
AS $$
DECLARE
    res reservation_detail;
BEGIN
    SELECT cr.id, da.availabledate, cr.starttime, cr.endtime, cr.doctoravailabilityid,cr.isdone, ds.doctorid, array_agg(ds.servicename) 
    INTO res.id, res.availabledate, res.start_time, res.end_time, res.doctor_availability_id,res.is_done, res.doctor_id, res.service_names
    FROM clientreservation as cr
    JOIN reservationdetail rd ON cr.id = rd.clientreservationid
    JOIN doctorservice as ds ON ds.id = rd.doctorserviceid
    JOIN doctoravailability AS da ON da.id = cr.doctoravailabilityid
    WHERE cr.id = _id
	GROUP BY cr.id, ds.doctorid, da.availabledate;

	RETURN res;
    
END;
$$;