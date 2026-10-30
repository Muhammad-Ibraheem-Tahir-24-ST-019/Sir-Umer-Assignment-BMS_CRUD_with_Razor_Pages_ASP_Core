IF DB_ID(N'BooksManagement') IS NULL
    CREATE DATABASE BooksManagement;
GO
USE BooksManagement;
GO
IF OBJECT_ID(N'dbo.books', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.books
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        title NVARCHAR(50) NOT NULL,
        author NVARCHAR(50) NOT NULL,
        description NVARCHAR(50) NULL,
        publisher NVARCHAR(50) NULL,
        status NVARCHAR(50) NOT NULL,
        category NVARCHAR(50) NULL
    );
END;
GO
