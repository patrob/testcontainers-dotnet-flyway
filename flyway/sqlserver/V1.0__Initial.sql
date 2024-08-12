CREATE TABLE Testing (
    Id INT IDENTITY(1, 1) NOT NULL,
    Name varchar(max) NOT NULL
    CONSTRAINT PK_Testing_Id PRIMARY KEY CLUSTERED (Id)
);

INSERT INTO Testing (Name) VALUES ('test');