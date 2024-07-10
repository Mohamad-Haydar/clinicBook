Create Table doctor_service(
  id int primary key GENERATED ALWAYS AS IDENTITY,
  duration int NOT NULL, -- duration in minutes and represent the duration of this service for the specified doctor
  doctor_id varchar(128) NOT NULL,
  service_id int NOT NULL,
  CONSTRAINT fk_doctor FOREIGN KEY(doctor_id) REFERENCES doctor(id) ON DELETE CASCADE,
  CONSTRAINT fk_service FOREIGN KEY(service_id) REFERENCES service(id) ON DELETE CASCADE
)