-- To fill the doctor services 
INSERT INTO doctor_service(duration, doctor_id, service_id) VALUES
(10,1, 1),
(10,2, 1),
(5,2, 3),
(10,3, 1),
(7,3, 3),
(10,4, 1),
(5,4, 3),
(10,5, 1),
(5,5, 3),
(10,6, 1),
(5,6, 3),
(10,7, 2),
(10,7, 2),
(5,7, 4),
(15,7, 2),
(10,8, 1),
(5,8, 3),
(10,9, 1),
(5,9, 3),
(10,10, 2),
(10,10, 2),
(5,10, 4),
(15,10, 2),
(10,11, 1),
(5,11, 3);

-- Insert into doctor_availability
INSERT INTO doctor_availability(available_date, day_name, start_hour, end_hour, max_client, doctor_id) VALUES
('2024-7-8', 'Monday', '8:00:00','12:00:00',15,1),
('2024-7-9', 'Tuesday', '8:00:00','12:00:00',15,1),
('2024-7-9', 'Tuesday', '8:00:00','9:00:00',8,2),
('2024-7-10', 'Wednesday', '8:00:00','12:00:00',15,1),
('2024-7-10', 'Wednesday', '9:00:00','10:00:00',7,3),
('2024-7-10', 'Wednesday', '10:30:00','11:30:00',7,4),
('2024-7-11', 'Thursday', '10:00:00','11:00:00',5,5),
('2024-7-11', 'Thursday', '11:00:00','12:00:00',8,6),
('2024-7-12', 'Friday', '8:00:00','12:00:00',15,1),
('2024-7-12', 'Friday', '9:00:00','10:30:00',15,11),
('2024-7-12', 'Friday', '10:00:00','11:30:00',10,7),
('2024-7-13','Saturday', '9:00:00','11:10:00',13,1),
('2024-7-13','Saturday', '9:00:00','11:00:00',15,8),
('2024-7-13','Saturday', '9:30:00','11:00:00',9,9),
('2024-7-13','Saturday', '9:00:00','11:00:00',15,10),
--
('2024-7-15', 'Monday', '8:00:00','12:00:00',15,1),
('2024-7-16','Tuesday', '8:00:00','12:00:00',15,1),
('2024-7-17', 'Wednesday', '8:00:00','12:00:00',15,1),
('2024-7-17', 'Wednesday', '9:00:00','10:00:00',7,3),
('2024-7-17', 'Wednesday', '10:30:00','11:30:00',7,4),
('2024-7-18', 'Thursday', '10:00:00','11:00:00',5,5),
('2024-7-18', 'Thursday', '11:00:00','12:00:00',8,6),
('2024-7-19', 'Friday', '8:00:00','12:00:00',15,1),
('2024-7-19', 'Friday','10:00:00','11:30:00',10,7),
('2024-7-20', 'Saturday', '9:00:00','11:10:00',13,1),
('2024-7-20', 'Saturday', '9:30:00','11:00:00',9,9),
('2024-7-20', 'Saturday', '9:00:00','11:00:00',15,10);

-- Insert into client_reservation
INSERT INTO client_reservation(start_time, duration, client_id, doctor_availabilty_id) VALUES
('8:00:00', 10, 1, 1),
('8:10:00', 10, 3, 1),
('8:20:00', 10, 5, 1),
('8:30:00', 10, 6, 1),
('8:40:00', 10, 7, 1),
('8:50:00', 10, 8, 1),
('8:00:00', 10, 2, 3),
('8:00:00', 10, 4, 4),
('8:00:00', 7, 9, 5),
('10:00:00', 20, 10, 11);

-- Insert into client_reservation
INSERT INTO reservation_detail(doctor_service_id, client_reservation_id) VALUES
(10, 1),
(10, 2),
(10, 3),
(10, 4),
(10, 5),
(10, 6),
(10, 7),
(10, 8),
(7, 9),
(5, 10),
(15, 10);