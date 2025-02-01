use real_state_db;

-- Customer table create query
create table customer_details(
cust_id int auto_increment,
cust_name varchar(50),
cust_address varchar(255),
cust_contact int(10),
cust_birth_date nvarchar(30),
cust_email varchar(50),
primary key(cust_id));


Alter Table customer_details modify cust_id nvarchar(50);

-- User table details select query
select * from user;

SELECT @@hostname AS Hostname, @@port AS Port;

-- User table create query
create table user(
user_id int auto_increment,
user_name varchar(50),
user_email varchar(50),
user_contact varchar(50),
user_password varchar(50),
primary key(user_id));

-- Building table create query

create table building_details(
building_id int auto_increment,
building_or_project_name varchar(125),
building_location_pincode varchar(125),
building_state varchar(125),
building_district varchar(125),
building_village varchar(125),
primary key(building_id));

Alter Table building_details modify building_id nvarchar(50);

select * from building_details;
drop table building_details;

-- Flat table create query
create table flat_details (
flat_id int auto_increment,
building_id int,
total_floor int ,
rate double,
available_flats_count int,
primary key(flat_id));

Alter Table flat_details modify building_id nvarchar(50);

Alter Table flat_details modify flat_id nvarchar(50);

-- Flat table details select query
select * from flat_details;

Alter Table flat_details add column flat_type_id long;

create table flat_type_details (
flat_type_id long not null,
flat_type_name varchar(50));

Alter Table flat_type_details modify flat_type_id nvarchar(50);

select * from flat_type_details;

-- Cancellation table details select query
create table cancellation_details ( 
cancel_id int auto_increment,
booking_cust_contact long,
cancel_date varchar(10),
total_amount double,
total_paid double,
refund double,
primary key(cancel_id));

-- Cancellation table details select query
select * from cancellation_details;

drop table cancellation_details;

-- Payment table details select query
create table payment_details (
payment_id int auto_increment,
transaction_date varchar(10),
booking_id int,
amount_paid double,
total_amount double,
primary key(payment_id));

-- Payment table details select query
select * from payment_details;

-- Enquiry table details select query
create table enquiry_details (
enquir_id int auto_increment,
enquir_name varchar(125),
enquiry_date varchar(10),
enquir_email varchar(50),
enquir_contact varchar(50),
primary key(enquir_id));

-- Enquiry table details select query
select * from enquiry_details;

create table quatation_details (
quatation_id nvarchar(50),
quatation_date varchar(10),
discription varchar(125),
customer_contact varchar(50),
building_name varchar(125),
flat_type_name varchar(50),
price_per_sq_ft double,
base_price double,
additionl_charges double,
discount double,
total_amount double,
down_payment double,
Payment_mode varchar(15),
primary key(quatation_id));

--  quatation table details select query
select * from quatation_details;

truncate table quatation_details;
alter table quatation_details modify column quatation_id nvarchar(50);

SELECT flat_type_name, COUNT(*) as count FROM quatation_details GROUP BY flat_type_name;

create VIEW vw_flat_details AS
SELECT 
    f.flat_id,
    ft.flat_type_name,
    b.building_or_project_name,
    b.building_location_pincode,
    b.building_state,
    b.building_district,
    b.building_village,
    f.total_floor,
    f.rate,
    f.available_flats_count
FROM flat_details f
JOIN flat_type_details ft ON f.flat_type_id = ft.flat_type_id
JOIN building_details b ON f.building_id = b.building_id;
















