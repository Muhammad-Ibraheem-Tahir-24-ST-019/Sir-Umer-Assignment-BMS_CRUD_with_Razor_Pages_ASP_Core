CREATE DATABASE IF NOT EXISTS sir_umer_books
    CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE sir_umer_books;

CREATE TABLE IF NOT EXISTS books (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(50) NOT NULL,
    author VARCHAR(50) NOT NULL,
    description VARCHAR(50) NULL,
    publisher VARCHAR(50) NULL,
    status VARCHAR(50) NOT NULL,
    category VARCHAR(50) NULL
);
