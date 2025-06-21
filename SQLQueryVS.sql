Select*from specialPackages

DBCC CHECKIDENT ('specialPackages', RESEED, 21);

UPDATE specialPackages
SET Id = Id + 21

DELETE FROM specialPackages

SET IDENTITY_INSERT SpecialPackages ON;

INSERT INTO SpecialPackages (Id,Name, Price, Duration)
VALUES
(21,'Relaxation Bundle', 1500.00, 180),
(22,'Rejuvenation Pack', 1800.00, 180),
(23,'Ultimate Wellness Package', 2000.00, 180),
(24,'Serenity Package', 1600.00, 180);

select*from Bookings

SELECT * 
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS 
WHERE CONSTRAINT_NAME = 'FK_Bookings_Treatments_TreatmentId';

ALTER TABLE Bookings
ALTER COLUMN TreatmentId INT NULL;
