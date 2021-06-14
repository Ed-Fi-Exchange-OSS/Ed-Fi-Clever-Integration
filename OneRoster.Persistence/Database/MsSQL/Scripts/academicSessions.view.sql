CREATE OR ALTER VIEW onerosterv11.academicSessions
AS
-- SchoolYears
SELECT       
	SYT.Id																						AS "sourcedId"
    , CASE WHEN  currentschoolyear='1' THEN 'active'		ELSE 'tobedeleted' END				AS status
    , SYT.LastModifiedDate																		AS "dateLastModified"
	, SYT.SchoolYearDescription																	AS title
     , MIN(SES.BeginDate)																		AS "startDate"
     , MAX(SES.EndDate)																			AS "endDate"	    
	, 'schoolYear'																				AS type
	, NULL																						AS parent
    , SYT.SchoolYear																			AS "schoolYear"
FROM edfi.SchoolYearType AS SYT
INNER JOIN edfi.Session AS SES ON SYT.SchoolYear = SES.SchoolYear
Group by SYT.Id, SYT.SchoolYear, SYT.CurrentSchoolYear, SYT.LastModifiedDate, SYT.SchoolYearDescription

UNION ALL

--Terms
-- SELECT       
-- 	TDE.Id										AS "sourcedId"
--     , CASE 		
-- 	  WHEN  currentschoolyear='1' THEN 'active'		
-- 	  ELSE 'tobedeleted' END					AS status
--     , MAX(SES.LastModifiedDate)					AS "dateLastModified"
-- 	, TDE.CodeValue								AS title
--     , MIN(SES.BeginDate)						AS "startDate"
--     , MAX(SES.EndDate)							AS "endDate"
-- 	, 'term'									AS type
-- 	, CONCAT('{ "sourcedId":"', SYT.Id, '", "type": "schoolYear" , "href": "/" }')	AS parent
-- 	, '[]'										AS children
--     , SYT.SchoolYear							AS "schoolYear"
-- FROM edfi.SchoolYearType AS SYT
-- INNER JOIN edfi.Session AS SES ON SYT.SchoolYear = SES.SchoolYear
-- INNER JOIN edfi.Descriptor TDE ON ses.TermDescriptorId = TDE.DescriptorId
-- Group by SYT.Id, SYT.SchoolYear, SYT.CurrentSchoolYear, TDE.Id, TDE.CodeValue

-- UNION ALL

-- Sessions
SELECT       
	SES.Id									  																AS "sourcedId"
    , CASE 	  WHEN SYT.CurrentSchoolYear = '1' THEN 'active'	  ELSE 'tobedeleted' END					AS status	
	, SES.LastModifiedDate																					AS "dateLastModified"
	, SES.SessionName																						AS title
    , SES.BeginDate																							AS "startDate"
    , SES.EndDate																							AS "endDate"	
	, 'term'																								AS type
	, CONCAT('{ "sourcedId":"', SYT.Id, '" } ') AS parent
    , SES.SchoolYear							AS "schoolYear"
FROM edfi.Session AS SES
INNER JOIN edfi.SchoolYearType AS SYT ON SES.SchoolYear = SYT.SchoolYear
LEFT JOIN edfi.Descriptor TDE ON ses.TermDescriptorId = TDE.DescriptorId

UNION ALL
-- Grading Periods
SELECT 
	GP.Id																									AS "sourcedId"
	, CASE  WHEN SYT.CurrentSchoolYear = '1' THEN 'active'	  ELSE 'tobedeleted' END						AS status
	, GP.LastModifiedDate																					AS "dateLastModified"
	, DES.CodeValue															AS title
	, GP.BeginDate								AS "startDate"
	, GP.EndDate								AS "endDate"	
	, 'gradingPeriod'														AS type
	, CONCAT('{ "sourcedId":"', SES.Id, '"}') 								AS parent
	, GP.SchoolYear								AS "schoolYear"
FROM edfi.GradingPeriod				AS GP
INNER JOIN edfi.Descriptor AS DES ON DES.DescriptorId = GP.GradingPeriodDescriptorId
INNER JOIN edfi.SessionGradingPeriod AS SGP 
	ON SGP.GradingPeriodDescriptorId = GP.GradingPeriodDescriptorId 
	AND SGP.SchoolId = GP.SchoolId
INNER JOIN edfi.Session AS SES 
	ON SGP.SessionName = SES.SessionName 
	AND SES.SchoolId = SGP.SchoolId 
	AND SES.SchoolYear = SGP.SchoolYear
INNER JOIN edfi.SchoolYearType AS SYT ON SES.SchoolYear = SYT.SchoolYear;