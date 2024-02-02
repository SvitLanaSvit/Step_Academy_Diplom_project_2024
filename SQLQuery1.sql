INSERT INTO Categories
Values('House'),
('Apartment');

Select * from Categories;
Select * from Houses;
Select * from AspNetUsers;
Select * from Addresses;

INSERT INTO Addresses (Latitude, Longitude, Country, City, FormattedAddress, AddressLabel) VALUES
('40.712776', '-74.005974', 'USA', 'New York', 'New York, NY, USA', 'NYC Label'),
('48.856614', '2.352222', 'France', 'Paris', 'Paris, France', 'Paris Label'),
('51.507351', '-0.127758', 'UK', 'London', 'London, UK', 'London Label'),
('35.689487', '139.691711', 'Japan', 'Tokyo', 'Tokyo, Japan', 'Tokyo Label');