CREATE TABLE dbo.Basket (
    CartItemId INT PRIMARY KEY,         
    Id INT NOT NULL,              
    CofId INT NOT NULL,                
    Quantity INT NOT NULL,              
    FOREIGN KEY (Id) REFERENCES dbo.Users(Id),   
    FOREIGN KEY (CofId) REFERENCES [dbo].[Table](CofId)   
);
