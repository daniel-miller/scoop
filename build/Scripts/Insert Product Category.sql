declare @id uniqueidentifier = '019480e0-c314-7fc4-a0f6-c384f42807d1';

delete from m.Lookups_Values where LookupIdentifier = @id;

insert into m.Lookups_Values
(
    ParentLookupNumber
  , ParentLookupIdentifier
  , Abbreviation
  , Context
  , Sequence
  , Status
  , Text
  , LookupIdentifier
)
values
(115, 'DA9329E5-1A3B-EF11-9139-9AD27CB82623', null, null, 12, 'Active', 'Composite Mats', @id);