Create Table Service(
  Id int primary key GENERATED ALWAYS AS IDENTITY,
  ServiceName varchar(50) NOT NULL
);

CREATE Table Category(
    Id int primary key GENERATED ALWAYS AS IDENTITY,
    CategoryName varchar(50) NOT NULL
);

Create Table Client(
  Id varchar(128) primary key,
  FirstName varchar(20) NOT NULL,
  LastName varchar(20) NOT NULL,
  Email varchar(50) NOT NULL,
  PhoneNumber varchar(15) NOT NULL
);


Create Table Doctor(
  Id varchar(128) primary key,
  FirstName varchar(20)  NOT NULL,
  LastName varchar(20) NOT NULL,
  Email varchar(50) NOT NULL,
  PhoneNumber varchar(15) NOT NULL,
  Description text NOT NULL,
  CategoryId int NOT NULL,
  CONSTRAINT fk_category FOREIGN KEY(CategoryId) REFERENCES Category(Id) ON DELETE CASCADE
);

Create Table DoctorService(
  Id int primary key GENERATED ALWAYS AS IDENTITY,
  ServiceName varchar(50) NOT NULL,
  Duration int NOT NULL, -- Duration in minutes and represent the Duration of this service for the specified doctor
  DoctorId varchar(128) NOT NULL,
  ServiceId int NOT NULL,
  CONSTRAINT fk_doctor FOREIGN KEY(DoctorId) REFERENCES Doctor(Id) ON DELETE CASCADE,
  CONSTRAINT fk_service FOREIGN KEY(ServiceId) REFERENCES Service(Id) ON DELETE CASCADE
);

Create Table DoctorAvailability(
  Id int primary key GENERATED ALWAYS AS IDENTITY,
  AvailableDate date NOT NULL,
  DayName varchar(10) NOT NULL,
  StartHour time NOT NULL,
  EndHour time NOT NULL,
  MaxClient int NOT NULL,
  CurrentReservations int NOT NULL DEFAULT 0,
  DoctorId varchar(128) NOT NULL,
  CONSTRAINT fk_doctor FOREIGN KEY(DoctorId) REFERENCES Doctor(Id) ON DELETE CASCADE,
  CONSTRAINT check_start_end_hours CHECK (starthour < endhour),
  CONSTRAINT check_available_date CHECK (availabledate >= current_date)
);

Create Table ClientReservation(
  Id int primary key GENERATED ALWAYS AS IDENTITY,
  StartTime time NOT NULL,
  endtime time NOT NULL, -- Duration in minutes and calculated based on the services reserved
  ClientId varchar(128) NOT NULL,
  DoctorAvailabilityId int NOT NULL,
  isdone boolean NOT NULL DEFAULT false,
  CONSTRAINT fk_client FOREIGN KEY(ClientId) REFERENCES Client(Id) ON DELETE CASCADE,
  CONSTRAINT fk_doctor_availability FOREIGN KEY(DoctorAvailabilityId) REFERENCES DoctorAvailability(Id) ON DELETE CASCADE,
  CONSTRAINT unique_client_doctor_availability UNIQUE (clientid, doctoravailabilityid)
);

Create Table ReservationDetail(
  Id int primary key GENERATED ALWAYS AS IDENTITY,
  DoctorServiceId int NOT NULL,
  ClientReservationId int NOT NULL,
  CONSTRAINT fk_doctor_service FOREIGN KEY(DoctorServiceId) REFERENCES DoctorService(Id) ON DELETE CASCADE,
  CONSTRAINT fk_client_reservation FOREIGN KEY(ClientReservationId) REFERENCES ClientReservation(Id) ON DELETE CASCADE
);

Create Table Secretary(
  Id varchar(128) primary key,
  FirstName varchar(20) NOT NULL,
  LastName varchar(20) NOT NULL,
  Email varchar(50) NOT NULL,
  PhoneNumber varchar(15) NOT NULL
);

