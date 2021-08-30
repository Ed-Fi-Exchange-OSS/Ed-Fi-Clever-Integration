--CREATE OR ALTER VIEW [onerosterv11].[demographics] AS

-- TEACHERS // STAFF
SELECT  DISTINCT 
	-- a = staff, t = teacher
	CONCAT('t',CAST(sta.StaffUSI AS VARCHAR)) 																					AS "sourcedId"
	, CASE WHEN (SELECT TOP 2 COUNT(SEOAAT.StaffUSI) FROM edfi.StaffEducationOrganizationAssignmentAssociation SEOAAT 
		WHERE SEOAAT.StaffUSI = SEOAA.StaffUSI GROUP BY SEOAAT.StaffUSI /*LIMIT 2*/) = 1 AND SEOAA.EndDate IS NULL 
	  THEN 'active' ELSE 'tobedeleted' END 																						AS "status"
    , STA.LastModifiedDate						                                                        						AS "dateLastModified"
	, STA.BirthDate																												AS "birthDate"
	, CASE WHEN SED.CodeValue = 'Female' THEN 'female' ELSE 'male' END															AS "sex"
	, CAST(CASE WHEN srdIndian.CodeValue IS NOT NULL THEN 'true' ELSE 'false' END AS VARCHAR)					AS "americanIndianOrAlaskaNative"
	, CAST(CASE WHEN RAD.CodeValue = 'Asian' THEN 'true' ELSE 'false' END AS VARCHAR)											AS "asian"
    , CAST(CASE WHEN RAD.CodeValue = 'Black - African American' THEN 'true' ELSE 'false' END AS VARCHAR)						AS "blackOrAfricanAmerican"
    , CAST(CASE WHEN RAD.CodeValue = 'Native Hawaiian - Pacific Islander' THEN 'true' ELSE 'false' END AS VARCHAR)				AS "nativeHawaiianOrOtherPacificIslander"
	, CAST(CASE WHEN RAD.CodeValue = 'White' THEN 'true' ELSE 'false' END AS VARCHAR)											AS "white"
    , CAST(CASE WHEN RAD.CodeValue = 'Other'  or RAD.CodeValue = 'Two or More Races' THEN 'true' ELSE 'false' END AS VARCHAR)	AS "demographicRaceTwoOrMoreRaces"
	, CAST(CASE WHEN STA.HispanicLatinoEthnicity = 1 THEN 'true' ELSE 'false' END AS VARCHAR)									AS "hispanicOrLatinoEthnicity"
FROM edfi.Staff STA 
LEFT JOIN edfi.StaffSectionAssociation SSA	ON STA.StaffUSI = SSA.StaffUSI
LEFT JOIN edfi.StaffEducationOrganizationAssignmentAssociation SEOAA	ON STA.StaffUSI = SEOAA.StaffUSI
LEFT JOIN edfi.Descriptor SED ON STA.SexDescriptorId = SED.DescriptorId
LEFT JOIN (select SRASingle.StaffUSI, SRASingle.RaceDescriptorId  from edfi.StaffRace SRASingle) SRA ON STA.StaffUSI = SRA.StaffUSI
LEFT JOIN edfi.Descriptor RAD ON SRA.RaceDescriptorId = RAD.DescriptorId
/*Race Joins:*/
-- 'American Indian - Alaska Native'
LEFT JOIN edfi.StaffRace srIndian ON STA.StaffUSI = srIndian.StaffUSI
LEFT JOIN edfi.Descriptor srdIndian ON srIndian.RaceDescriptorId = srdIndian.DescriptorId and srdIndian.CodeValue = 'American Indian - Alaska Native'
-- 'Asian'
LEFT JOIN edfi.StaffRace srAsian ON STA.StaffUSI = srAsian.StaffUSI
LEFT JOIN edfi.Descriptor srdAsian ON srAsian.RaceDescriptorId = srdAsian.DescriptorId and srdAsian.CodeValue = 'Asian'
-- 'Black - African American'
LEFT JOIN edfi.StaffRace srBlack ON STA.StaffUSI = srBlack.StaffUSI
LEFT JOIN edfi.Descriptor srdBlack ON srAsian.RaceDescriptorId = srdBlack.DescriptorId and srdBlack.CodeValue = 'Black - African American'
-- TODO: Add more races.


--SELECT staffusi, Count(*) FROM edfi.StaffRace group by StaffUsi

UNION ALL
-- STUDENTS
SELECT DISTINCT
	CONCAT('s',  CAST(STU.studentusi AS VARCHAR)  )																					AS "sourcedId"
	,CASE WHEN (SELECT COUNT(SSAT.StudentUSI) FROM edfi.StudentSchoolAssociation SSAT 
	WHERE SSAT.StudentUSI = SSA.StudentUSI GROUP BY SSAT.StudentUSI /*LIMIT 2*/) = 1 AND SSA.ExitWithdrawDate IS NULL 
	THEN 'active' ELSE 'active' END 																								AS status
    , STU.LastModifiedDate							                                                         						AS "dateLastModified"
	, STU.birthDate as  "birthDate"
	, CASE WHEN SED.CodeValue = 'Female' THEN 'female' ELSE 'male' END																AS sex
	, CAST(CASE WHEN SRA.CodeValues like '%American Indian - Alaska Native%' THEN 'true' ELSE 'false' END AS VARCHAR)						AS "americanIndianOrAlaskaNative"
	, CAST(CASE WHEN SRA.CodeValues like '%Asian%' THEN 'true' ELSE 'false' END AS VARCHAR)												AS asian
    , CAST(CASE WHEN SRA.CodeValues like '%Black - African American%' THEN 'true' ELSE 'false' END AS VARCHAR)							AS "blackOrAfricanAmerican"
    , CAST(CASE WHEN SRA.CodeValues like '%Native Hawaiian - Pacific Islander%' THEN 'true' ELSE 'false' END AS VARCHAR)					AS "nativeHawaiianOrOtherPacificIslander"
	, CAST(CASE WHEN SRA.CodeValues like '%White%' THEN 'true' ELSE 'false' END AS VARCHAR)												AS white
    , CAST(CASE WHEN SRA.CodeValues like '%Other%'  or   SRA.CodeValues like '%Two or More Races%' or SRA.CodeValues like '%,%' THEN 'true' ELSE 'false' END AS VARCHAR)		AS "demographicRaceTwoOrMoreRaces"
		, CAST(CASE WHEN SEOA.HispanicLatinoEthnicity = 1 or SRA.CodeValues like '%Hispanic or Latino%' THEN 'true' ELSE 'false' END AS VARCHAR)	AS "hispanicOrLatinoEthnicity"

FROM edfi.Student STU
INNER JOIN edfi.StudentSchoolAssociation SSA 	ON STU.StudentUSI = SSA.StudentUSI
LEFT JOIN edfi.Descriptor SED 	ON STU.BirthSexDescriptorId = SED.DescriptorId
INNER JOIN edfi.StudentEducationOrganizationAssociation SEOA 	ON STU.StudentUSI = SEOA.StudentUSI

left join (SELECT edfi.studenteducationorganizationassociationrace.studentusi
,  string_agg(edfi.Descriptor.codevalue/*::text*/, ',')  CodeValues
FROM edfi.studenteducationorganizationassociationrace
LEFT JOIN edfi.Descriptor  	ON edfi.studenteducationorganizationassociationrace.RaceDescriptorId = edfi.Descriptor.DescriptorId
GROUP BY edfi.studenteducationorganizationassociationrace.studentusi
) SRA ON STU.studentusi = SRA.studentusi

join edfi.StudentSchoolAssociation SSAT on SSAT.StudentUSI = stu.StudentUSI
where SSAT.exitwithdrawdate is null
GO


