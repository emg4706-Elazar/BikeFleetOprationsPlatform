CREATE TABLE Stations
(
	station_id VARCHAR(100) PRIMARY KEY,
	station_name VARCHAR(255) NOT NULL,
	short_Name VARCHAR(50) NOT NULL,
	longitude DECIMAL(10,7) NOT NULL,
	latitude DECIMAL(9,7) NOT NULL,
	region_id VARCHAR(50) NOT NULL,
	capacity INT NOT NULL,
	android_uri VARCHAR(500) NULL,
	ios_uri VARCHAR(500) NULL,
	web_uri VARCHAR(500) NULL
);

CREATE TABLE VehicleTypes
(
	vehicle_type_id	VARCHAR(100) PRIMARY KEY,
    form_factor VARCHAR(30) NOT NULL,
    propulsion_type VARCHAR(30) NOT NULL,
    max_range_meters DECIMAL(12,2) NULL
);
