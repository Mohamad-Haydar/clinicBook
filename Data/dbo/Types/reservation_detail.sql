CREATE TYPE reservation_detail AS (
    id int,
    availabledate date,
    start_time time,
    end_time time,
    doctor_availability_id int,
    service_names character varying[], 
    doctor_id character varying
);