Create Table client(
  id varchar(128) primary key,
  first_name varchar(20) NOT NULL,
  last_name varchar(20) NOT NULL,
  email varchar(50) NOT NULL,
  phone_number varchar(15) NOT NULL
)