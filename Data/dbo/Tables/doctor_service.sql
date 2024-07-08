Create Table doctor_service(
  id int primary key GENERATED ALWAYS AS IDENTITY,
  service_name varchar(50) NOT NULL,
  price money NOT NULL,
  duration int NOT NULL, -- duration in minutes and represent the duration of this service for the specified doctor
  doctor_id int NOT NULL,
  CONSTRAINT fk_doctor FOREIGN KEY(doctor_id) REFERENCES doctor(id) ON DELETE CASCADE
)