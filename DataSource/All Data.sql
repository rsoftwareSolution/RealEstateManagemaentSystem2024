--- All data Insert Query
use real_state_db;


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
