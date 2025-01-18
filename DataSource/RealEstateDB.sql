use real_state_db;
create table customer_details(
cust_id int auto_increment,
cust_name varchar(50),
cust_address varchar(255),
cust_contact int(10),
cust_birth_date nvarchar(30),
cust_email varchar(50),
primary key(cust_id));

select * from user;

SELECT @@hostname AS Hostname, @@port AS Port;

create table user(
user_id int auto_increment,
user_name varchar(50),
user_email varchar(50),
user_contact varchar(50),
user_password varchar(50),
primary key(user_id));