Create Table doctor(
  id int primary key GENERATED ALWAYS AS IDENTITY,
  first_name varchar(20)  NOT NULL,
  last_name varchar(20) NOT NULL,
  speciality varchar(50) NOT NULL,
  email varchar(50) NOT NULL,
  phone_number varchar(15) NOT NULL
)