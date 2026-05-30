USE MedicalCenterDB;
GO

-- 1. SŁOWNIKI (Tabele z INT ID)
-- Roles
SET IDENTITY_INSERT dbo.Roles ON;
INSERT INTO dbo.Roles (Id, Name) VALUES 
(1, N'Admin'),
(2, N'Doctor'),
(3, N'Patient'),
(4, N'Courier');
SET IDENTITY_INSERT dbo.Roles OFF;

-- AppointmentStatuses
SET IDENTITY_INSERT dbo.AppointmentStatuses ON;
INSERT INTO dbo.AppointmentStatuses (Id, Name) VALUES 
(1, N'Zaplanowana'),
(2, N'Zakończona'),
(3, N'Anulowana');
SET IDENTITY_INSERT dbo.AppointmentStatuses OFF;

-- OrderStatuses
SET IDENTITY_INSERT dbo.OrderStatuses ON;
INSERT INTO dbo.OrderStatuses (Id, Name) VALUES 
(1, N'Nowe'),
(2, N'W realizacji'),
(3, N'Wysłane'),
(4, N'Zakończone'),
(5, N'Anulowane');
SET IDENTITY_INSERT dbo.OrderStatuses OFF;

-- DeliveryStatuses
SET IDENTITY_INSERT dbo.DeliveryStatuses ON;
INSERT INTO dbo.DeliveryStatuses (Id, Name) VALUES 
(1, N'Oczekuje na kuriera'),
(2, N'W drodze'),
(3, N'Dostarczono');
SET IDENTITY_INSERT dbo.DeliveryStatuses OFF;
GO

-- 2. TABELE Z GUID (Niezależne)
-- Specializations
INSERT INTO dbo.Specializations (Id, Name) VALUES 
('11111111-1111-1111-1111-111111111111', N'Kardiolog'),
('22222222-2222-2222-2222-222222222222', N'Neurolog'),
('33333333-3333-3333-3333-333333333333', N'Pediatra');

-- Departments
INSERT INTO dbo.Departments (Id, Name) VALUES 
('44444444-1111-1111-1111-111111111111', N'Oddział Kardiologii'),
('44444444-2222-2222-2222-222222222222', N'Oddział Neurologii'),
('44444444-3333-3333-3333-333333333333', N'Oddział Pediatrii');

-- Addresses
INSERT INTO dbo.Addresses (Id, Street, HouseNumber, ApartmentNumber, PostalCode, City) VALUES 
('55555555-1111-1111-1111-111111111111', N'Lipowa', N'12', N'5', N'15-424', N'Białystok');

-- 3. USERS (Zależne od Roles)
INSERT INTO dbo.Users (Id, Email, PasswordHash, FirstName, LastName, Phone, RoleId, ProfilePicturePath) VALUES 
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', N'admin@medical.pl', N'$2a$11$wHXCchTbS3pO/OujL1VHQebwwG.cPIncjS2w7JHidEZqzLT05tg7e', N'Adam', N'Nowak', N'111222333', 1, NULL),
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', N'lekarz@medical.pl', N'$2a$11$wHXCchTbS3pO/OujL1VHQebwwG.cPIncjS2w7JHidEZqzLT05tg7e', N'Jan', N'Kowalski', N'222333444', 2, NULL),
('cccccccc-cccc-cccc-cccc-cccccccccccc', N'pacjent@medical.pl', N'$2a$11$wHXCchTbS3pO/OujL1VHQebwwG.cPIncjS2w7JHidEZqzLT05tg7e', N'Anna', N'Wiśniewska', N'333444555', 3, NULL),
('dddddddd-dddd-dddd-dddd-dddddddddddd', N'kurier@medical.pl', N'$2a$11$wHXCchTbS3pO/OujL1VHQebwwG.cPIncjS2w7JHidEZqzLT05tg7e', N'Wiesław', N'Szybki', N'999888777', 4, NULL);
GO

-- 4. POZOSTAŁE TABELE (Zależne od Users, Addresses, Specializations itp.)
-- Doctors
INSERT INTO dbo.Doctors (Id, UserId, LicenseNumber, SpecializationId) VALUES 
('66666666-1111-1111-1111-111111111111', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', N'LEK123456', '11111111-1111-1111-1111-111111111111');

-- DoctorDepartments
INSERT INTO dbo.DoctorDepartments (Id, DoctorId, DepartmentId) VALUES 
('77777777-1111-1111-1111-111111111111', '66666666-1111-1111-1111-111111111111', '44444444-1111-1111-1111-111111111111'),
('77777777-2222-2222-2222-222222222222', '66666666-1111-1111-1111-111111111111', '44444444-2222-2222-2222-222222222222');

-- Patients
INSERT INTO dbo.Patients (Id, UserId, Pesel, BirthDate, AddressId) VALUES 
('88888888-1111-1111-1111-111111111111', 'cccccccc-cccc-cccc-cccc-cccccccccccc', N'99010112345', '1999-01-01', '55555555-1111-1111-1111-111111111111');

-- Couriers
INSERT INTO dbo.Couriers (Id, UserId, VehicleRegistration) VALUES 
('99999999-1111-1111-1111-111111111111', 'dddddddd-dddd-dddd-dddd-dddddddddddd', N'BI1234K');
