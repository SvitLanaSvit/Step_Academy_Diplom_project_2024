

INSERT INTO Categories
Values('House'),
('Apartment');

Select * from Categories;
Select * from Houses;
Select * from AspNetUsers;
Select * from Addresses;
Select * from Rents;
Select * from Tags;
Select * from HouseTag;
Select * from Images;

INSERT INTO Addresses (Latitude, Longitude, Country, City, FormattedAddress, AddressLabel) VALUES
('40.712776', '-74.005974', 'USA', 'New York', 'New York, NY, USA', 'NYC Label'),
('48.856614', '2.352222', 'France', 'Paris', 'Paris, France', 'Paris Label'),
('51.507351', '-0.127758', 'UK', 'London', 'London, UK', 'London Label'),
('35.689487', '139.691711', 'Japan', 'Tokyo', 'Tokyo, Japan', 'Tokyo Label');

INSERT INTO Addresses (Latitude, Longitude, Country, City, FormattedAddress, AddressLabel)
VALUES ('34.052235', '-118.243683', 'USA', 'Los Angeles', 'Los Angeles, CA, USA', 'Downtown LA');

INSERT INTO Addresses (Latitude, Longitude, Country, City, FormattedAddress, AddressLabel)
VALUES
('35.689487', '139.691706', 'Japan', 'Tokyo', 'Tokyo, Japan', 'Tokyo Tower');

INSERT INTO AspNetUsers(Id, Email, EmailConfirmed, DisplayName, UserName, AccessFailedCount, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled)
Values('1','dmytro.vychkin@gmail.com', 1, 'dmytro', 'dmytro', 1, 1, 0, 0),
('2','svitlana.kizilpinar@gmail.com', 1, 'sviti', 'svitlana', 1, 1, 0, 0);

INSERT INTO Houses (Description, Price, SquareMeter, Rooms, AddressId, CategoryId, Username, IsModerated)
VALUES
('Lovely two-story house in the countryside', 250.00, 200, 5, 1, 1, 'user1', 0),
('Modern apartment in the city center', 300.00, 120, 4, 2, 2, 'user2', 0),
('Cozy cottage near the lake', 180.00, 150, 3, 3, 6, 'user3', 0),
('Spacious villa with a private pool', 500.00, 350, 6, 4, 7, 'user4', 0),
('Charming bungalow on the beach', 450.00, 250, 4, 6, 8, 'user5', 0);

Update Houses
Set Price = 450
Where Id = 14;

INSERT INTO Rents (Username, HouseId, CountOfDay, Price, "From", "To")
VALUES
('user1', 10, 7, 700.00, '2023-01-01', '2023-01-08'),
('user2', 11, 3, 450.00, '2023-02-15', '2023-02-18');

UPDATE Rents
SET Price = 900
Where Id = 2;

Update Houses
Set UserId = 1
Where Id = 14;

INSERT INTO Tags (Name, ImagePath) VALUES
('Nature', '/images/tags/nature.png'),
('City Life', '/images/tags/citylife.png'),
('Mountains', '/images/tags/mountains.png'),
('Beaches', '/images/tags/beaches.png'),
('Historical', '/images/tags/historical.png');
