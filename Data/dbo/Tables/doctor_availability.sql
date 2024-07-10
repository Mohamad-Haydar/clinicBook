Create Table doctor_availability(
  id int primary key GENERATED ALWAYS AS IDENTITY,
  available_date date NOT NULL,
  day_name varchar(10) NOT NULL,
  start_hour timetz NOT NULL,
  end_hour timetz NOT NULL,
  max_client int NOT NULL,
  doctor_id varchar(128) NOT NULL,
  CONSTRAINT fk_doctor FOREIGN KEY(doctor_id) REFERENCES doctor(id) ON DELETE CASCADE
)