CREATE TABLE users (
	Id integer CONSTRAINT firstkey PRIMARY KEY,
	Email varchar(255) NOT NULL,
	Password varchar(255) NOT NULL,
	Salt varchar(255) NOT NULL);