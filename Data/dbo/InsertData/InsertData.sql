-- TO fill the secretary table
INSERT INTO secretary(first_name, last_name, email, phone_number) VALUES('Zeinab','Salloum','zeinab@gmail.com','123456');

-- To fill category
INSERT INTO category(category_name) VALUES
('Public health'),
('Endocrine and diabetes'),
('Heart diseases'),
('Gynecology'),
('Nutrition specialist'),
('Pediatrics'),
('Dentistry'),
('Family deaseses'),
('Skin desises');

-- To fill the services table
insert into service(service_name) VALUES
('Preview'),
('Session'),
('Review'),
('Cleaning');

-- To Fill the doctor table
INSERT INTO doctor(first_name, last_name, email, phone_number, description, category_id) VALUES
('Jalal','Haydar','jalal@gmail.com','123456','general treatment for 400000',1),
('Ziad','Awad','Ziad@gmail.com','123456','Endocrine and diabetes for 600000',2),
('Mohamad','Hayek','MohamadHayek@gmail.com','123456','Heart diseases for 600000',3),
('Hayat','Shoaib','Hayat@gmail.com','123456','Gynecology for 600000',4),
('Hiba','Ismail','Hiba@gmail.com','123456','Nutrition specialist for 500000',5),
('Bashar','Shmeisany','Bashar@gmail.com','123456','Pediatrics for 700000',6),
('Mohamad','Abdallah','MohamadAbdallah@gmail.com','123456','Dentistry \n removal for 400000 \n cleaning for 200000 \n fix cockroach for 700000',7),
('Zeina','Kanso','Zeina@gmail.com','123456','Famiiy desease for 500000',8),
('Yosra','Nasser','Yosra@gmail.com','123456','Gynecology for 600000',4),
('Kawthar','Halawi','Kawthar@gmail.com','123456','Dentistry \n removal for 400000 \n cleaning for 200000 \n fix cockroach for 700000',7),
('Reda','Reda','Reda@gmail.com','123456','Skin treatment for 500000', 9);

-- To fill the client
INSERT INTO client(first_name, last_name, email, phone_number) VALUES
('ali','awad','aliawad@gmail.com','123456'),
('Jomana','Kheil','JomanaKheil@gmail.com','123456'),
('ghadir','salhab','ghadirsalhab@gmail.com','123456'),
('ali','haydar','alihaydar@gmail.com','123456'),
('hussein','maatouk','husseinmaatouk@gmail.com','123456'),
('jawd','halawi','jawdhalawi@gmail.com','123456'),
('mohamad','koeik','mohamadkoeik@gmail.com','123456'),
('hassan','sbeity','hassansbeity@gmail.com','123456'),
('lina','sabra','linasabra@gmail.com','123456'),
('layal','maatouk','layalmaatouk@gmail.com','123456'),
('jinan','hamed','jinanhamed@gmail.com','123456'),
('zahraa','halawi','zahraahalawi@gmail.com','123456'),
('mouslim','mousilmany','mouslimmousilmany@gmail.com','123456'),
('rabia','hammoud','rabiahammoud@gmail.com','123456'),
('fouad','nassir','fouadnassir@gmail.com','123456'),
('ali','ankar','aliankar@gmail.com','123456'),
('joumana','mourad','joumanamourad@gmail.com','123456'),
('aline','nahle','alinenahle@gmail.com','123456'),
('alaa','kassem','alaakassem@gmail.com','123456'),
('hassan','sbeity','hassansbeity@gmail.com','123456'),
('abed','hayek','abedhayek@gmail.com','123456'),
('ali','mousilmany','alimousilmany@gmail.com','123456'),
('line','hamze','linehamze@gmail.com','123456'),
('ahmad','ayash','ahmadayash@gmail.com','123456');
