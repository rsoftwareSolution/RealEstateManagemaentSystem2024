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
building_address varchar(125),
primary key(build_id));

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

-- Flat table details select query
select * from flat_details;


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

















