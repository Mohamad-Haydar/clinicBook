CREATE OR REPLACE FUNCTION f_get_all_reservation_for_the_day(IN doctor_availability_id int) 
    RETURNS SETOF reservation_detail
    LANGUAGE plpgsql
AS $$
DECLARE
    res reservation_detail;
BEGIN
   RETURN QUERY
  SELECT 
        cr.id, 
        da.availabledate,
        cr.starttime, 
        cr.endtime, 
        cr.doctoravailabilityid, 
        array_agg(ds.servicename) AS service_names,
        ds.doctorid
   FROM clientreservation AS cr
   JOIN reservationdetail AS rd ON cr.id = rd.clientreservationid
   JOIN doctorservice AS ds ON ds.id = rd.doctorserviceid
   JOIN client ON client.id = cr.clientid
   JOIN doctoravailability AS da ON da.id = cr.doctoravailabilityid
   WHERE cr.doctoravailabilityid = doctor_availability_id
   GROUP BY cr.id, cr.starttime, cr.endtime, cr.doctoravailabilityid, ds.doctorid, da.availabledate;
END;
$$;
