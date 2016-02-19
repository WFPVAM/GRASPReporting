 /* Adds Filters fields to Reports table */
 alter table [GRASP].[dbo].[Reports] add Filters nvarchar(2000), FiltersSummary nvarchar(2000), FiltersCount int;
