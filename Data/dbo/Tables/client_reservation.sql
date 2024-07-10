Create Table client_reservation(
  id int primary key GENERATED ALWAYS AS IDENTITY,
  start_time timetz NOT NULL,
  duration int NOT NULL, -- duration in minutes and calculated based on the services reserved
  client_id varchar(128) NOT NULL,
  doctor_availabilty_id int NOT NULL,
  CONSTRAINT fk_client FOREIGN KEY(client_id) REFERENCES client(id) ON DELETE CASCADE,
  CONSTRAINT fk_doctor_availability FOREIGN KEY(doctor_availabilty_id) REFERENCES doctor_availability(id) ON DELETE CASCADE
)