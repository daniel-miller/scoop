-- Disable the trigger before we start.

DISABLE TRIGGER a.Trigger_ShipmentAfterChange ON a.Orders_Shipments;
GO

-- Force an analysis of the entire inventory, and create clones of the Movement and Inventory 
-- snapshot tables.

DECLARE @Start DATE = 'Jan 1, 2012'
EXEC dbo.AnalyzeInventory @At = @Start;
GO

CREATE TABLE dbo.TMovementClone
    (
        MovementIdentifier     UNIQUEIDENTIFIER  NOT NULL,
        MovementNumber         INT               NOT NULL,
        MovementCode           VARCHAR(7)        NULL,
        MovementType           VARCHAR(30)       NULL,
        MovementPurpose        VARCHAR(30)       NULL,
        MovementDate           DATE              NOT NULL,
        MovementPrice          NUMERIC(9, 2)     NOT NULL,
        MovementPriceChange    BIT               NOT NULL,
        MovementUnit           VARCHAR(30)       NOT NULL,
        MovementQuantity       INT               NOT NULL,
        ProductIdentifier      UNIQUEIDENTIFIER  NOT NULL,
        ProductCategory        VARCHAR(30)       NOT NULL,
        ProductCategorySort1   INT               NOT NULL,
        ProductCategorySort2   INT               NOT NULL,
        ProductName            VARCHAR(100)      NOT NULL,
        FromSiteIdentifier     UNIQUEIDENTIFIER  NOT NULL,
        FromSiteType           VARCHAR(20)       NOT NULL,
        FromSiteName           VARCHAR(100)      NOT NULL,
        ToSiteIdentifier       UNIQUEIDENTIFIER  NOT NULL,
        ToSiteType             VARCHAR(20)       NOT NULL,
        ToSiteName             VARCHAR(100)      NOT NULL,
        FromLocationIdentifier UNIQUEIDENTIFIER  NOT NULL,
        FromLocationType       VARCHAR(20)       NOT NULL,
        FromLocationName       VARCHAR(100)      NOT NULL,
        FromLocationCompany    VARCHAR(100)      NULL,
        FromAccounting         BIT               NOT NULL,
        ToLocationIdentifier   UNIQUEIDENTIFIER  NOT NULL,
        ToLocationType         VARCHAR(20)       NOT NULL,
        ToLocationName         VARCHAR(100)      NOT NULL,
        ToLocationCompany      VARCHAR(100)      NULL,
        ToAccounting           BIT               NOT NULL,
        ModifiedWhen           DATETIMEOFFSET(7) NOT NULL,
        PRIMARY KEY CLUSTERED (MovementIdentifier ASC)
    );

GO

INSERT INTO dbo.TMovementClone
    (
        MovementIdentifier,
        MovementNumber,
        MovementCode,
        MovementType,
        MovementPurpose,
        MovementDate,
        MovementPrice,
        MovementPriceChange,
        MovementUnit,
        MovementQuantity,
        ProductIdentifier,
        ProductCategory,
        ProductCategorySort1,
        ProductCategorySort2,
        ProductName,
        FromSiteIdentifier,
        FromSiteType,
        FromSiteName,
        ToSiteIdentifier,
        ToSiteType,
        ToSiteName,
        FromLocationIdentifier,
        FromLocationType,
        FromLocationName,
        FromLocationCompany,
        FromAccounting,
        ToLocationIdentifier,
        ToLocationType,
        ToLocationName,
        ToLocationCompany,
        ToAccounting,
        ModifiedWhen
    )
            SELECT
                MovementIdentifier,
                MovementNumber,
                MovementCode,
                MovementType,
                MovementPurpose,
                MovementDate,
                MovementPrice,
                MovementPriceChange,
                MovementUnit,
                MovementQuantity,
                ProductIdentifier,
                ProductCategory,
                ProductCategorySort1,
                ProductCategorySort2,
                ProductName,
                FromSiteIdentifier,
                FromSiteType,
                FromSiteName,
                ToSiteIdentifier,
                ToSiteType,
                ToSiteName,
                FromLocationIdentifier,
                FromLocationType,
                FromLocationName,
                FromLocationCompany,
                FromAccounting,
                ToLocationIdentifier,
                ToLocationType,
                ToLocationName,
                ToLocationCompany,
                ToAccounting,
                ModifiedWhen
            FROM
                dbo.TMovement;

GO

CREATE TABLE dbo.TInventoryClone
    (
        InventoryIdentifier  UNIQUEIDENTIFIER  NOT NULL,
        MovementIdentifier   UNIQUEIDENTIFIER  NOT NULL,
        MovementNumber       INT               NOT NULL,
        MovementCode         VARCHAR(7)        NULL,
        MovementType         VARCHAR(30)       NULL,
        MovementPurpose      VARCHAR(30)       NULL,
        MovementDate         DATE              NOT NULL,
        MovementPrice        NUMERIC(9, 2)     NOT NULL,
        MovementPriceChange  BIT               NOT NULL,
        MovementUnit         VARCHAR(30)       NOT NULL,
        MovementDirection    VARCHAR(3)        NOT NULL,
        MovementQuantity     INT               NOT NULL,
        ProductIdentifier    UNIQUEIDENTIFIER  NOT NULL,
        ProductCategory      VARCHAR(30)       NOT NULL,
        ProductCategorySort1 INT               NOT NULL,
        ProductCategorySort2 INT               NOT NULL,
        ProductName          VARCHAR(100)      NOT NULL,
        SiteIdentifier       UNIQUEIDENTIFIER  NOT NULL,
        SiteType             VARCHAR(20)       NOT NULL,
        SiteName             VARCHAR(100)      NOT NULL,
        LocationIdentifier   UNIQUEIDENTIFIER  NOT NULL,
        LocationType         VARCHAR(20)       NOT NULL,
        LocationName         VARCHAR(100)      NOT NULL,
        LocationCompany      VARCHAR(100)      NULL,
        Accounting           BIT               NOT NULL,
        QuantityReported     INT               NOT NULL,
        QuantityRented       INT               NOT NULL,
        QuantityReturned     INT               NOT NULL,
        QuantitySold         INT               NOT NULL,
        QuantityRacked       INT               NOT NULL,
        QuantityTransferred  INT               NOT NULL,
        QuantityOpened       INT               NOT NULL,
        QuantityProduced     INT               NOT NULL,
        QuantityAdjusted     INT               NOT NULL,
        QuantityDestroyed    INT               NOT NULL,
        ModifiedWhen         DATETIMEOFFSET(7) NOT NULL,
        PRIMARY KEY CLUSTERED (InventoryIdentifier ASC)
    );
GO

INSERT INTO dbo.TInventoryClone
    (
        InventoryIdentifier,
        MovementIdentifier,
        MovementNumber,
        MovementCode,
        MovementType,
        MovementPurpose,
        MovementDate,
        MovementPrice,
        MovementPriceChange,
        MovementUnit,
        MovementDirection,
        MovementQuantity,
        ProductIdentifier,
        ProductCategory,
        ProductCategorySort1,
        ProductCategorySort2,
        ProductName,
        SiteIdentifier,
        SiteType,
        SiteName,
        LocationIdentifier,
        LocationType,
        LocationName,
        LocationCompany,
        Accounting,
        QuantityReported,
        QuantityRented,
        QuantityReturned,
        QuantitySold,
        QuantityRacked,
        QuantityTransferred,
        QuantityOpened,
        QuantityProduced,
        QuantityAdjusted,
        QuantityDestroyed,
        ModifiedWhen
    )
            SELECT
                InventoryIdentifier,
                MovementIdentifier,
                MovementNumber,
                MovementCode,
                MovementType,
                MovementPurpose,
                MovementDate,
                MovementPrice,
                MovementPriceChange,
                MovementUnit,
                MovementDirection,
                MovementQuantity,
                ProductIdentifier,
                ProductCategory,
                ProductCategorySort1,
                ProductCategorySort2,
                ProductName,
                SiteIdentifier,
                SiteType,
                SiteName,
                LocationIdentifier,
                LocationType,
                LocationName,
                LocationCompany,
                Accounting,
                QuantityReported,
                QuantityRented,
                QuantityReturned,
                QuantitySold,
                QuantityRacked,
                QuantityTransferred,
                QuantityOpened,
                QuantityProduced,
                QuantityAdjusted,
                QuantityDestroyed,
                ModifiedWhen
            FROM
                dbo.TInventory;

GO

-- Create buffer tables to store the list of archived jobs and sites, and the current quantity for 
-- each product in each location.

DROP TABLE IF EXISTS inventory.BArchiveJob;
DROP TABLE IF EXISTS inventory.BCurrentQuantity;
GO

CREATE TABLE inventory.BArchiveJob
(
 LocationIdentifier UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
 LocationType VARCHAR(10) NOT NULL
);

CREATE TABLE inventory.BCurrentQuantity
(
 CurrentIdentifier UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT(NEWSEQUENTIALID()),
 LocationIdentifier UNIQUEIDENTIFIER NOT NULL,
 ProductIdentifier UNIQUEIDENTIFIER NOT NULL,
 QuantityBeforeArchive INT NOT NULL,
 QuantityAfterArchive INT NULL,
 QuantityDelta INT NULL,
 QuantityAfterAdjustment INT NULL
);

-- Guarantee no duplicate entries for location and product.

CREATE UNIQUE NONCLUSTERED INDEX IX_CurrentState ON inventory.BCurrentQuantity (LocationIdentifier,ProductIdentifier);

-- Archive jobs (and sites) completed before January 1, 2023 with NO shipments after that date.

TRUNCATE TABLE inventory.BArchiveJob;

INSERT INTO inventory.BArchiveJob (LocationIdentifier, LocationType)
SELECT LocationIdentifier, SubType FROM t.Tasks_Locations AS L
WHERE SubType = 'Jobs' 
  AND DateCompleted IS NOT NULL 
  AND DateCompleted < 'Jan 1, 2023'
  AND NOT EXISTS 
  (
   SELECT * FROM dbo.TMovement AS M 
   WHERE M.MovementDate >= 'Jan 1, 2023'
   AND L.LocationIdentifier IN (M.FromLocationIdentifier, M.ToLocationIdentifier)
  )
  ;

 GO

-- If an archived job has any direct or indirect shipment activity after the cut-off date then it 
-- must be removed from the list of jobs that are candidates for archiving.

DECLARE @RowsAffected INT = 1;

WHILE @RowsAffected > 0
BEGIN
    
    DELETE FROM inventory.BArchiveJob
    WHERE LocationIdentifier IN
    (
        SELECT M.FromLocationIdentifier
        FROM dbo.TMovement AS M
        WHERE M.FromLocationIdentifier IN (SELECT A.LocationIdentifier FROM inventory.BArchiveJob AS A) AND M.FromLocationType = 'Jobs'
        AND M.ToLocationIdentifier NOT IN (SELECT A.LocationIdentifier FROM inventory.BArchiveJob AS A) AND M.ToLocationType = 'Jobs'

        UNION
        
        SELECT M.ToLocationIdentifier
        FROM dbo.TMovement AS M
        WHERE M.FromLocationIdentifier IN (SELECT A.LocationIdentifier FROM inventory.BArchiveJob AS A) AND M.FromLocationType = 'Jobs'
        AND M.ToLocationIdentifier NOT IN (SELECT A.LocationIdentifier FROM inventory.BArchiveJob AS A) AND M.ToLocationType = 'Jobs'

        UNION

        SELECT M.FromLocationIdentifier
        FROM dbo.TMovement AS M
        WHERE M.ToLocationIdentifier IN (SELECT A.LocationIdentifier FROM inventory.BArchiveJob AS A) AND M.ToLocationType = 'Jobs'
        AND M.FromLocationIdentifier NOT IN (SELECT A.LocationIdentifier FROM inventory.BArchiveJob AS A) AND M.FromLocationType = 'Jobs'

        UNION
        
        SELECT M.ToLocationIdentifier
        FROM dbo.TMovement AS M
        WHERE M.ToLocationIdentifier IN (SELECT A.LocationIdentifier FROM inventory.BArchiveJob AS A) AND M.ToLocationType = 'Jobs'
        AND M.FromLocationIdentifier NOT IN (SELECT A.LocationIdentifier FROM inventory.BArchiveJob AS A) AND M.FromLocationType = 'Jobs'
    );

    SET @RowsAffected = @@ROWCOUNT;
END

GO

-- Calculate the current quantity for each product in each location before archiving anything.

WITH cte AS 
(SELECT SiteIdentifier, ProductIdentifier, SUM(CASE MovementDirection WHEN 'Out' THEN -1 ELSE 1 END * MovementQuantity) AS Quantity 
 FROM dbo.TInventory GROUP BY SiteIdentifier, ProductIdentifier)
INSERT INTO inventory.BCurrentQuantity
(LocationIdentifier, ProductIdentifier, QuantityBeforeArchive)
SELECT Q.SiteIdentifier, Q.ProductIdentifier, Q.Quantity
FROM cte AS Q
GO

-- Delete archived locations, shipments, orders, and rentals.

DECLARE @cutoff DATE = 'Jan 1, 2023';

DELETE FROM m.Lookups_Dates 
WHERE [Value] < @cutoff OR [Value] >= 'Jan 1, 2026';

DELETE FROM t.Tasks_Locations 
WHERE EXISTS (SELECT * FROM inventory.BArchiveJob AS j WHERE j.LocationIdentifier = Tasks_Locations.LocationIdentifier);

DELETE FROM t.Tasks_Locations 
WHERE ParentLocationIdentifier IS NOT NULL 
AND NOT EXISTS (SELECT * FROM t.Tasks_Locations AS X WHERE X.LocationIdentifier = Tasks_Locations.ParentLocationIdentifier);

DELETE FROM a.Orders_Shipments 
WHERE NOT EXISTS (SELECT * FROM t.Tasks_Locations AS x WHERE x.LocationIdentifier IN (Orders_Shipments.ShipFromLocationIdentifier,Orders_Shipments.ShipToLocationIdentifier));

DELETE FROM a.Orders_Orders 
WHERE NOT EXISTS (SELECT * FROM t.Tasks_Locations AS x WHERE x.LocationIdentifier = Orders_Orders.ShipToLocationIdentifier);

DELETE FROM a.Orders_Items 
WHERE NOT EXISTS (SELECT * FROM a.Orders_Orders AS x WHERE x.OrderIdentifier = Orders_Items.OrderIdentifier);

DELETE FROM a.Sales_Rentals 
WHERE NOT EXISTS (SELECT * FROM t.Tasks_Locations AS x WHERE x.LocationIdentifier = Sales_Rentals.LocationIdentifier);

GO

-- Recalculate the current quantity for each product in each location after archiving.

DECLARE @Start DATE = 'Jan 1, 2012'
EXEC dbo.AnalyzeInventory @At = @Start;
GO

WITH cte AS (SELECT SiteIdentifier, ProductIdentifier, SUM(CASE MovementDirection WHEN 'Out' THEN -1 ELSE 1 END * MovementQuantity) AS Quantity FROM dbo.TInventory GROUP BY SiteIdentifier, ProductIdentifier)
UPDATE inventory.BCurrentQuantity
SET QuantityAfterArchive = Q.Quantity
FROM cte AS Q
WHERE BCurrentQuantity.LocationIdentifier = Q.SiteIdentifier AND BCurrentQuantity.ProductIdentifier = Q.ProductIdentifier;
GO

-- Calculate the before-and-after change in quantity.

UPDATE inventory.BCurrentQuantity
SET QuantityDelta = ISNULL(QuantityAfterArchive,0) - QuantityBeforeArchive
GO

-- Insert a new accounting location for the Archive.

DECLARE @archive UNIQUEIDENTIFIER = '6BF78A66-F3B3-4173-8E81-5D3870D7403D';

DELETE FROM t.Tasks_Locations WHERE LocationIdentifier = @archive;

INSERT INTO t.Tasks_Locations
(ParentLocationIdentifier, SubType, Name, TenantID, IsReconciled, LocationIdentifier)
VALUES
('B7A829E5-1A3B-EF11-9139-9AD27CB82623','Yards','Archive',1,0,@archive);

-- Create a shipment from the location to the Archive for each location that has "gained" inventory.
-- This will restore its quantity to the expected amount.

DECLARE @next INT = 1 + (SELECT MAX(ShipmentNumber) FROM a.Orders_Shipments);

INSERT INTO a.Orders_Shipments
(
  ShipFromLocationIdentifier
, ShipToLocationIdentifier
, UnitLookupIdentifier
, ProductIdentifier
, Quantity
, UnitPrice
, DateShipped
, SubType
, SubTypeQualifier
, ShipmentIdentifier
, ShipmentNumber
)
SELECT
    LocationIdentifier,
    '6BF78A66-F3B3-4173-8E81-5D3870D7403D' ,
    'BA9329E5-1A3B-EF11-9139-9AD27CB82623' ,
    ProductIdentifier,
    ABS(SUM(QuantityDelta)),
    0 ,
    CAST('2023-01-01' AS DATE) ,
    'Transferred' ,
    'Archive' ,
    NEWID() ,
    @next + ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) 
FROM inventory.BCurrentQuantity
WHERE QuantityDelta > 0
GROUP BY LocationIdentifier, ProductIdentifier;

GO

-- Create a shipment from the Archive to the location for each location that has "lost" inventory.
-- This will restore its quantity to the expected amount.

DECLARE @next INT = 1 + (SELECT MAX(ShipmentNumber) FROM a.Orders_Shipments);

INSERT INTO a.Orders_Shipments
(
  ShipFromLocationIdentifier
, ShipToLocationIdentifier
, UnitLookupIdentifier
, ProductIdentifier
, Quantity
, UnitPrice
, DateShipped
, SubType
, SubTypeQualifier
, ShipmentIdentifier
, ShipmentNumber
)
SELECT
    '6BF78A66-F3B3-4173-8E81-5D3870D7403D' ,
    LocationIdentifier,
    'BA9329E5-1A3B-EF11-9139-9AD27CB82623' ,
    ProductIdentifier,
    ABS(SUM(QuantityDelta)),
    0 ,
    CAST('2023-01-01' AS DATE) ,
    'Transferred' ,
    'Archive' ,
    NEWID() ,
    @next + ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) 
FROM inventory.BCurrentQuantity
WHERE QuantityDelta < 0
GROUP BY LocationIdentifier, ProductIdentifier;

GO

-- Recalculate the current quantity for each product in each location one last time to confirm the 
-- adjustments produce the expected quantity for every product in every location.

DECLARE @Start DATE = 'Jan 1, 2012'
EXEC dbo.AnalyzeInventory @At = @Start;
GO

WITH cte AS (SELECT SiteIdentifier, ProductIdentifier, SUM(CASE MovementDirection WHEN 'Out' THEN -1 ELSE 1 END * MovementQuantity) AS Quantity FROM dbo.TInventory GROUP BY SiteIdentifier, ProductIdentifier)
UPDATE inventory.BCurrentQuantity
SET QuantityAfterAdjustment = Q.Quantity
FROM cte AS Q
WHERE BCurrentQuantity.LocationIdentifier = Q.SiteIdentifier AND BCurrentQuantity.ProductIdentifier = Q.ProductIdentifier;
GO

UPDATE inventory.BCurrentQuantity SET QuantityAfterArchive = 0 WHERE QuantityAfterArchive IS NULL;

UPDATE inventory.BCurrentQuantity SET QuantityAfterAdjustment = 0 WHERE QuantityAfterAdjustment IS NULL;

GO

-- Enable the trigger to restore normal operation.

ENABLE TRIGGER a.Trigger_ShipmentAfterChange ON a.Orders_Shipments;
GO

-- Confirm the quantity before archiving now matches the quantity after adjustments. We expect this 
-- query to return zero rows.

SELECT COUNT(*) AS ExpectZero FROM inventory.BCurrentQuantity AS Q
WHERE Q.LocationIdentifier IN (SELECT X.LocationIdentifier from t.Tasks_Locations AS X)
  AND QuantityBeforeArchive <> QuantityAfterAdjustment;

