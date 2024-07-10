Create Table doctor(
  id varchar(128) primary key,
  first_name varchar(20)  NOT NULL,
  last_name varchar(20) NOT NULL,
  email varchar(50) NOT NULL,
  phone_number varchar(15) NOT NULL,
  description text NOT NULL,
  category_id int NOT NULL,
  CONSTRAINT fk_category FOREIGN KEY(category_id) REFERENCES category(id) ON DELETE CASCADE
)