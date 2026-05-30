-- W master
USE master;
GO
CREATE LOGIN MedCenterAdmin 
    WITH PASSWORD = 'Admin#StrongPass123', 
         DEFAULT_DATABASE = MedicalCenterDB,
         CHECK_POLICY = ON;
GO

-- W bazie aplikacyjnej
USE MedicalCenterDB;
GO
CREATE USER MedCenterAdmin FOR LOGIN MedCenterAdmin;
ALTER ROLE db_owner ADD MEMBER MedCenterAdmin;
GO


USE master;
GO
CREATE LOGIN ApplicationIdentity
    WITH PASSWORD = 'App#StrongPass123',
         DEFAULT_DATABASE = MedicalCenterDB,
         CHECK_POLICY = ON;
GO

USE MedicalCenterDB;
GO
CREATE USER ApplicationIdentity FOR LOGIN ApplicationIdentity;

-- READ
ALTER ROLE db_datareader ADD MEMBER ApplicationIdentity;

-- WRITE
ALTER ROLE db_datawriter ADD MEMBER ApplicationIdentity;
GO


USE MedicalCenterDB;
GO

-- Tworzymy własną rolę
CREATE ROLE db_procexecutor;

-- Nadajemy jej uprawnienie EXECUTE na poziomie całej bazy
-- (czyli na wszystkie obecne i przyszłe procedury i funkcje)
GRANT EXECUTE TO db_procexecutor;

-- Dodajemy do niej ApplicationIdentity
ALTER ROLE db_procexecutor ADD MEMBER ApplicationIdentity;
GO


USE master;
GO
CREATE LOGIN Dev_Milosz
    WITH PASSWORD = 'Dev#StrongPass123',
         DEFAULT_DATABASE = MedicalCenterDB,
         CHECK_POLICY = ON;
GO

USE MedicalCenterDB;
GO
CREATE USER Dev_Milosz FOR LOGIN Dev_Milosz;
ALTER ROLE db_datareader ADD MEMBER Dev_Milosz;
GO


USE master;
GO
CREATE LOGIN Dev_Jakub
    WITH PASSWORD = 'Dev#StrongPass123',
         DEFAULT_DATABASE = MedicalCenterDB,
         CHECK_POLICY = ON;
GO

USE MedicalCenterDB;
GO
CREATE USER Dev_Jakub FOR LOGIN Dev_Jakub;
ALTER ROLE db_datareader ADD MEMBER Dev_Jakub;
GO



USE master;
GO
CREATE LOGIN MedAnalyst
    WITH PASSWORD = 'Analyst#StrongPass123',
         DEFAULT_DATABASE = MedicalCenterDB,
         CHECK_POLICY = ON;
GO

USE MedicalCenterDB;
GO
CREATE USER MedAnalyst FOR LOGIN MedAnalyst;
GO

-- Własna rola dla analityków
CREATE ROLE db_analyst;

-- Dodajemy konto do roli — uprawnienia nadajemy roli, nie kontu
ALTER ROLE db_analyst ADD MEMBER MedAnalyst;
GO