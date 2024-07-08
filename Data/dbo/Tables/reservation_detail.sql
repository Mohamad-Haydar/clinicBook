Create Table reservation_detail(
  id int primary key GENERATED ALWAYS AS IDENTITY,
  doctor_service_id int NOT NULL,
  client_reservation_id int NOT NULL,
  CONSTRAINT fk_doctor_service FOREIGN KEY(doctor_service_id) REFERENCES doctor_service(id) ON DELETE CASCADE,
  CONSTRAINT fk_client_reservation FOREIGN KEY(client_reservation_id) REFERENCES client_reservation(id) ON DELETE CASCADE
)