CREATE TYPE reservation_detail AS (
    id int,
    start_time timetz,
    end_time timetz,
    doctor_availability_id int,
    service_names character varying[], 
    doctor_id character varying
);