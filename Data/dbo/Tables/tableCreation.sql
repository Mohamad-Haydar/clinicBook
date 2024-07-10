Create Table service(
  id int primary key GENERATED ALWAYS AS IDENTITY,
  service_name varchar(50) NOT NULL
);

CREATE Table category(
    id int primary key GENERATED ALWAYS AS IDENTITY,
    category_name varchar(50) NOT NULL
);

Create Table client(
  id int primary key GENERATED ALWAYS AS IDENTITY,
  first_name varchar(20) NOT NULL,
  last_name varchar(20) NOT NULL,
  email varchar(50) NOT NULL,
  phone_number varchar(15) NOT NULL
);


Create Table doctor(
  id int primary key GENERATED ALWAYS AS IDENTITY,
  first_name varchar(20)  NOT NULL,
  last_name varchar(20) NOT NULL,
  email varchar(50) NOT NULL,
  phone_number varchar(15) NOT NULL,
  description text NOT NULL,
  category_id int NOT NULL,
  CONSTRAINT fk_category FOREIGN KEY(category_id) REFERENCES category(id) ON DELETE CASCADE
);

Create Table doctor_service(
  id int primary key GENERATED ALWAYS AS IDENTITY,
  service_name varchar(50) NOT NULL,
  duration int NOT NULL, -- duration in minutes and represent the duration of this service for the specified doctor
  doctor_id int NOT NULL,
  service_id int NOT NULL,
  CONSTRAINT fk_doctor FOREIGN KEY(doctor_id) REFERENCES doctor(id) ON DELETE CASCADE,
  CONSTRAINT fk_service FOREIGN KEY(service_id) REFERENCES service(id) ON DELETE CASCADE
);

Create Table doctor_availability(
  id int primary key GENERATED ALWAYS AS IDENTITY,
  available_date date NOT NULL,
  day_name varchar(10) NOT NULL,
  start_hour timetz NOT NULL,
  end_hour timetz NOT NULL,
  max_client int NOT NULL,
  doctor_id int NOT NULL,
  CONSTRAINT fk_doctor FOREIGN KEY(doctor_id) REFERENCES doctor(id) ON DELETE CASCADE
);

Create Table client_reservation(
  id int primary key GENERATED ALWAYS AS IDENTITY,
  start_time timetz NOT NULL,
  duration int NOT NULL, -- duration in minutes and calculated based on the services reserved
  client_id int NOT NULL,
  doctor_availabilty_id int NOT NULL,
  CONSTRAINT fk_client FOREIGN KEY(client_id) REFERENCES client(id) ON DELETE CASCADE,
  CONSTRAINT fk_doctor_availability FOREIGN KEY(doctor_availabilty_id) REFERENCES doctor_availability(id) ON DELETE CASCADE
);

Create Table reservation_detail(
  id int primary key GENERATED ALWAYS AS IDENTITY,
  doctor_service_id int NOT NULL,
  client_reservation_id int NOT NULL,
  CONSTRAINT fk_doctor_service FOREIGN KEY(doctor_service_id) REFERENCES doctor_service(id) ON DELETE CASCADE,
  CONSTRAINT fk_client_reservation FOREIGN KEY(client_reservation_id) REFERENCES client_reservation(id) ON DELETE CASCADE
);

Create Table secretary(
  id int primary key GENERATED ALWAYS AS IDENTITY,
  first_name varchar(20) NOT NULL,
  last_name varchar(20) NOT NULL,
  email varchar(50) NOT NULL,
  phone_number varchar(15) NOT NULL
);

