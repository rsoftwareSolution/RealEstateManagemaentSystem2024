--- All data Insert Query
use real_state_db;

INSERT INTO customer_details (cust_id, cust_name, cust_address, cust_contact, cust_birth_date, cust_email) 
VALUES
('CUST001', 'Aarav Sharma', '123 MG Road, Bangalore, Karnataka', 9876543210, '1990-05-15', 'aarav.sharma@example.com'),
('CUST002', 'Ishita Gupta', '456 Lajpat Nagar, Delhi', 9876543211, '1985-07-22', 'ishita.gupta@example.com'),
('CUST003', 'Rohit Kumar', '789 Janakpuri, Mumbai, Maharashtra', 9876543212, '1980-03-10', 'rohit.kumar@example.com'),
('CUST004', 'Neha Reddy', '321 Banjara Hills, Hyderabad, Telangana', 9876543213, '1992-11-30', 'neha.reddy@example.com'),
('CUST005', 'Vikram Verma', '654 Indira Nagar, Lucknow, Uttar Pradesh', 9876543214, '1988-12-05', 'vikram.verma@example.com'),
('CUST006', 'Priya Patel', '987 Navrangpura, Ahmedabad, Gujarat', 9876543215, '1994-02-20', 'priya.patel@example.com'),
('CUST007', 'Arjun Mehta', '654 Anna Nagar, Chennai, Tamil Nadu', 9876543216, '1982-09-14', 'arjun.mehta@example.com'),
('CUST008', 'Sanya Singh', '123 HSR Layout, Bangalore, Karnataka', 9876543217, '1996-04-25', 'sanya.singh@example.com'),
('CUST009', 'Amit Jain', '432 Bandra West, Mumbai, Maharashtra', 9876543218, '1987-06-10', 'amit.jain@example.com'),
('CUST010', 'Pooja Agarwal', '876 Nehru Place, Delhi', 9876543219, '1991-01-12', 'pooja.agarwal@example.com'),
('CUST011', 'Siddharth Nair', '321 Vasant Vihar, Delhi', 9876543220, '1993-08-17', 'siddharth.nair@example.com'),
('CUST012', 'Kritika Joshi', '987 Dadar West, Mumbai, Maharashtra', 9876543221, '1989-11-25', 'kritika.joshi@example.com'),
('CUST013', 'Rajesh Bansal', '654 Kukatpally, Hyderabad, Telangana', 9876543222, '1995-01-03', 'rajesh.bansal@example.com'),
('CUST014', 'Ritika Kapoor', '876 Kothrud, Pune, Maharashtra', 9876543223, '1983-10-09', 'ritika.kapoor@example.com'),
('CUST015', 'Karan Singh', '123 Malad East, Mumbai, Maharashtra', 9876543224, '1990-04-17', 'karan.singh@example.com'),
('CUST016', 'Divya Rao', '654 Koramangala, Bangalore, Karnataka', 9876543225, '1986-02-14', 'divya.rao@example.com'),
('CUST017', 'Manish Sharma', '987 Pusa Road, Delhi', 9876543226, '1992-07-29', 'manish.sharma@example.com'),
('CUST018', 'Simran Kaur', '321 Kharar, Punjab', 9876543227, '1994-10-22', 'simran.kaur@example.com'),
('CUST019', 'Vishal Yadav', '432 Karol Bagh, Delhi', 9876543228, '1984-12-01', 'vishal.yadav@example.com'),
('CUST020', 'Ananya Verma', '876 Salt Lake City, Kolkata, West Bengal', 9876543229, '1981-05-13', 'ananya.verma@example.com');

-- Insert records into building_details table
INSERT INTO building_details (building_id, building_or_project_name, building_location_pincode, building_state, building_district, building_village)
VALUES
('BUG0001', 'Skyline Residency', '560001', 'Karnataka', 'Bangalore', 'Indiranagar'),
('BUG0002', 'Sunset Towers', '400001', 'Maharashtra', 'Mumbai', 'Andheri'),
('BUG0003', 'Emerald Heights', '110001', 'Delhi', 'New Delhi', 'Connaught Place'),
('BUG0004', 'Green Valley', '600001', 'Tamil Nadu', 'Chennai', 'T. Nagar'),
('BUG0005', 'Lakeview Apartments', '700001', 'West Bengal', 'Kolkata', 'Salt Lake'),
('BUG0006', 'Palm Grove', '500001', 'Telangana', 'Hyderabad', 'Banjara Hills'),
('BUG0007', 'Blue Horizon', '380001', 'Gujarat', 'Ahmedabad', 'Navrangpura'),
('BUG0008', 'Golden Residency', '452001', 'Madhya Pradesh', 'Indore', 'Vijay Nagar'),
('BUG0009', 'The Grand Estates', '560002', 'Karnataka', 'Bangalore', 'Koramangala'),
('BUG0010', 'Ocean Breeze', '400002', 'Maharashtra', 'Mumbai', 'Bandra');

-- Insert records into flat_type_details table
INSERT INTO flat_type_details (flat_type_id, flat_type_name)
VALUES
('FTY0001', '1 BHK'),
('FTY0002', '2 BHK'),
('FTY0003', '3 BHK'),
('FTY0004', '4 BHK'),
('FTY0005', 'Penthouse');

-- Insert records into flat_details table
INSERT INTO flat_details (flat_id, flat_type_id, building_id, total_floor, rate, available_flats_count)
VALUES
('FLA0001', 'FTY0001', 'BUG0001', 10, 5000000, 5),
('FLA0002', 'FTY0002', 'BUG0001', 10, 7500000, 3),
('FLA0003', 'FTY0003', 'BUG0001', 10, 10000000, 2),
('FLA0004', 'FTY0001', 'BUG0002', 15, 5500000, 6),
('FLA0005', 'FTY0002', 'BUG0002', 15, 8000000, 4),
('FLA0006', 'FTY0003', 'BUG0002', 15, 12000000, 2),
('FLA0007', 'FTY0004', 'BUG0003', 12, 15000000, 1),
('FLA0008', 'FTY0005', 'BUG0003', 12, 20000000, 1),
('FLA0009', 'FTY0001', 'BUG0004', 8, 4500000, 7),
('FLA0010', 'FTY0002', 'BUG0004', 8, 7000000, 5),
('FLA0011', 'FTY0003', 'BUG0004', 8, 9500000, 2),
('FLA0012', 'FTY0001', 'BUG0005', 20, 6000000, 4),
('FLA0013', 'FTY0002', 'BUG0005', 20, 9000000, 3),
('FLA0014', 'FTY0003', 'BUG0005', 20, 13000000, 2),
('FLA0015', 'FTY0004', 'BUG0006', 14, 16000000, 1),
('FLA0016', 'FTY0005', 'BUG0006', 14, 22000000, 1),
('FLA0017', 'FTY0001', 'BUG0007', 9, 4800000, 5),
('FLA0018', 'FTY0002', 'BUG0007', 9, 7200000, 3),
('FLA0019', 'FTY0003', 'BUG0008', 18, 11000000, 2),
('FLA0020', 'FTY0004', 'BUG0008', 18, 14000000, 1);

INSERT INTO flat_type_description (flat_desc_id, flat_desc) VALUES
('FD0001', 'Luxury 2BHK with Lake View'),
('FD0002', 'Affordable 1BHK near Metro'),
('FD0003', 'Spacious 3BHK with Terrace'),
('FD0004', 'Budget Studio Apartment'),
('FD0005', 'Premium 4BHK Penthouse'),
('FD0006', 'Luxury 3BHK with Pool Access'),
('FD0007', 'Compact 1BHK in Gated Community'),
('FD0008', '3BHK with Garden Space'),
('FD0009', 'Affordable 2BHK near IT Park'),
('FD0010', 'Studio Apartment with Balcony'),
('FD0011', 'Modern 2BHK with Smart Home Features'),
('FD0012', 'Cozy 1BHK with Open Kitchen'),
('FD0013', 'Elegant 4BHK with Designer Interiors'),
('FD0014', 'Studio with Prime Location'),
('FD0015', '2BHK in a Peaceful Residential Area'),
('FD0016', 'Luxury 3BHK with City View'),
('FD0017', 'Compact 2BHK with Parking Space'),
('FD0018', 'High-End 1BHK with Luxury Amenities'),
('FD0019', '3BHK Duplex with Private Garden'),
('FD0020', 'Affordable Studio in Central Area');

INSERT INTO enquiry_details (enquir_name, enquiry_date, enquir_email, enquir_contact)  
VALUES  
('Amit Sharma', '2025-02-01', 'amit.sharma@example.com', '9876543201'),  
('Priya Verma', '2025-02-02', 'priya.verma@example.com', '9876543202'),  
('Rahul Gupta', '2025-02-03', 'rahul.gupta@example.com', '9876543203'),  
('Neha Reddy', '2025-02-04', 'neha.reddy@example.com', '9876543204'),  
('Vikram Singh', '2025-02-05', 'vikram.singh@example.com', '9876543205'),  
('Anjali Nair', '2025-02-06', 'anjali.nair@example.com', '9876543206'),  
('Ravi Iyer', '2025-02-07', 'ravi.iyer@example.com', '9876543207'),  
('Pooja Desai', '2025-02-08', 'pooja.desai@example.com', '9876543208'),  
('Suresh Menon', '2025-02-09', 'suresh.menon@example.com', '9876543209'),  
('Meena Choudhary', '2025-02-10', 'meena.choudhary@example.com', '9876543210'),  
('Karthik Rao', '2025-02-11', 'karthik.rao@example.com', '9876543211'),  
('Swati Kulkarni', '2025-02-12', 'swati.kulkarni@example.com', '9876543212'),  
('Arun Mishra', '2025-02-13', 'arun.mishra@example.com', '9876543213'),  
('Deepika Joshi', '2025-02-14', 'deepika.joshi@example.com', '9876543214'),  
('Sandeep Yadav', '2025-02-15', 'sandeep.yadav@example.com', '9876543215'),  
('Kiran Patil', '2025-02-16', 'kiran.patil@example.com', '9876543216'),  
('Roshni Saxena', '2025-02-17', 'roshni.saxena@example.com', '9876543217'),  
('Harish Bansal', '2025-02-18', 'harish.bansal@example.com', '9876543218'),  
('Jyoti Malhotra', '2025-02-19', 'jyoti.malhotra@example.com', '9876543219'),  
('Manoj Bhardwaj', '2025-02-20', 'manoj.bhardwaj@example.com', '9876543220');  


INSERT INTO enquiry_details (enquir_name, enquiry_date, enquir_email, enquir_contact) VALUES
('Amit Sharma', '2025-02-01', 'amit.sharma@example.com', '9876543210'),
('Priya Verma', '2025-02-02', 'priya.verma@example.com', '9876543211'),
('Rahul Gupta', '2025-02-03', 'rahul.gupta@example.com', '9876543212'),
('Neha Singh', '2025-02-04', 'neha.singh@example.com', '9876543213'),
('Vikram Iyer', '2025-02-05', 'vikram.iyer@example.com', '9876543214'),
('Anjali Nair', '2025-02-06', 'anjali.nair@example.com', '9876543215'),
('Suresh Reddy', '2025-02-07', 'suresh.reddy@example.com', '9876543216'),
('Meera Menon', '2025-02-08', 'meera.menon@example.com', '9876543217'),
('Arjun Desai', '2025-02-09', 'arjun.desai@example.com', '9876543218'),
('Pooja Patil', '2025-02-10', 'pooja.patil@example.com', '9876543219'),
('Rajesh Kumar', '2025-02-11', 'rajesh.kumar@example.com', '9876543220'),
('Sneha Choudhary', '2025-02-12', 'sneha.choudhary@example.com', '9876543221'),
('Manish Tiwari', '2025-02-13', 'manish.tiwari@example.com', '9876543222'),
('Deepika Rani', '2025-02-14', 'deepika.rani@example.com', '9876543223'),
('Karthik Srinivasan', '2025-02-15', 'karthik.srinivasan@example.com', '9876543224'),
('Swati Joshi', '2025-02-16', 'swati.joshi@example.com', '9876543225'),
('Ravi Pillai', '2025-02-17', 'ravi.pillai@example.com', '9876543226'),
('Divya Mehta', '2025-02-18', 'divya.mehta@example.com', '9876543227'),
('Harish Pandey', '2025-02-19', 'harish.pandey@example.com', '9876543228'),
('Aishwarya Rao', '2025-02-20', 'aishwarya.rao@example.com', '9876543229');


INSERT INTO parking_details (available_parking, total_parking, vehicle_name) VALUES
(5, 50, 'Maruti Suzuki Swift'),
(10, 100, 'Hyundai Creta'),
(3, 30, 'Honda City'),
(7, 70, 'Tata Nexon'),
(2, 20, 'Mahindra XUV700'),
(6, 60, 'Kia Seltos'),
(8, 80, 'Toyota Fortuner'),
(4, 40, 'Renault Kwid'),
(9, 90, 'Ford EcoSport'),
(1, 10, 'Volkswagen Polo'),
(12, 120, 'Hyundai Venue'),
(15, 150, 'Honda Amaze'),
(18, 180, 'Mahindra Thar'),
(20, 200, 'Maruti Suzuki Baleno'),
(14, 140, 'Kia Carens'),
(16, 160, 'MG Hector'),
(11, 110, 'Skoda Kushaq'),
(19, 190, 'Tata Harrier'),
(13, 130, 'Toyota Innova Crysta'),
(17, 170, 'Nissan Magnite');

INSERT INTO parking_details (available_parking, total_parking, vehicle_name) VALUES
(5, 50, 'Maruti Suzuki Swift'),
(10, 100, 'Hyundai Creta'),
(3, 30, 'Honda City'),
(7, 70, 'Tata Nexon'),
(2, 20, 'Mahindra XUV700'),
(6, 60, 'Kia Seltos'),
(8, 80, 'Toyota Fortuner'),
(4, 40, 'Renault Kwid'),
(9, 90, 'Ford EcoSport'),
(1, 10, 'Volkswagen Polo'),
(12, 120, 'Hyundai Venue'),
(15, 150, 'Honda Amaze'),
(18, 180, 'Mahindra Thar'),
(20, 200, 'Maruti Suzuki Baleno'),
(14, 140, 'Kia Carens'),
(16, 160, 'MG Hector'),
(11, 110, 'Skoda Kushaq'),
(19, 190, 'Tata Harrier'),
(13, 130, 'Toyota Innova Crysta'),
(17, 170, 'Nissan Magnite');
