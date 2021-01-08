drop view if exists onerosterv11.demographics;

CREATE VIEW onerosterv11.demographics
AS

-- TEACHERS // STAFF
SELECT  DISTINCT 
	-- a = staff, t = teacher
	CONCAT((CASE WHEN SSA.ClassroomPositionDescriptorId IS NULL THEN 't' ELSE 't' END), 
	CAST(sta.StaffUSI AS VARCHAR)  
	) 				AS "sourcedId"
	, 
	CASE WHEN (SELECT COUNT(SEOAAT.StaffUSI) FROM edfi.StaffEducationOrganizationAssignmentAssociation SEOAAT 
	WHERE SEOAAT.StaffUSI = SEOAA.StaffUSI GROUP BY SEOAAT.StaffUSI LIMIT 2) = 1 AND SEOAA.EndDate IS NULL 
	THEN 'active' ELSE 'active' END 																							AS status
    , STA.LastModifiedDate::timestamp with time zone                                                        						AS "dateLastModified"
	, '10-10-1987'::date as "birthDate"
	, CASE WHEN SED.CodeValue = 'Female' THEN 'female' ELSE 'male' END																AS sex
	, CAST(CASE WHEN RAD.CodeValue = 'American Indian - Alaska Native' THEN 'true' ELSE 'false' END AS VARCHAR)						AS "americanIndianOrAlaskaNative"
	, CAST(CASE WHEN RAD.CodeValue = 'Asian' THEN 'true' ELSE 'false' END AS VARCHAR)												AS asian
    , CAST(CASE WHEN RAD.CodeValue = 'Black - African American' THEN 'true' ELSE 'false' END AS VARCHAR)							AS "blackOrAfricanAmerican"
    , CAST(CASE WHEN RAD.CodeValue = 'Native Hawaiian - Pacific Islander' THEN 'true' ELSE 'false' END AS VARCHAR)					AS "nativeHawaiianOrOtherPacificIslander"
	, CAST(CASE WHEN RAD.CodeValue = 'White' THEN 'true' ELSE 'false' END AS VARCHAR)												AS white
    , CAST(CASE WHEN RAD.CodeValue = 'Other'  or   RAD.CodeValue is null  or RAD.CodeValue = 'Two or More Races' THEN 'true' ELSE 'false' END AS VARCHAR)						AS "demographicRaceTwoOrMoreRaces"
	,  CAST(CASE WHEN STA.HispanicLatinoEthnicity THEN 'true' ELSE 'false' END AS VARCHAR)  AS "hispanicOrLatinoEthnicity"
	
    
FROM edfi.Staff STA 
LEFT JOIN edfi.StaffSectionAssociation SSA	ON STA.StaffUSI = SSA.StaffUSI
LEFT JOIN edfi.StaffEducationOrganizationAssignmentAssociation SEOAA	ON STA.StaffUSI = SEOAA.StaffUSI
LEFT JOIN edfi.Descriptor SED 	ON STA.SexDescriptorId = SED.DescriptorId
LEFT  JOIN  ( select SRASingle.StaffUSI,SRASingle.RaceDescriptorId  from  edfi.StaffRace SRASingle  ) SRA ON STA.StaffUSI = SRA.StaffUSI
LEFT JOIN edfi.Descriptor RAD 	ON SRA.RaceDescriptorId = RAD.DescriptorId

UNION ALL
-- STUDENTS
SELECT DISTINCT
	CONCAT('s',  CAST(STU.studentusi AS VARCHAR)  )																			AS "sourcedId"
	,CASE WHEN (SELECT COUNT(SSAT.StudentUSI) FROM edfi.StudentSchoolAssociation SSAT 
	WHERE SSAT.StudentUSI = SSA.StudentUSI GROUP BY SSAT.StudentUSI LIMIT 2) = 1 AND SSA.ExitWithdrawDate IS NULL 
	THEN 'active' ELSE 'active' END 																							AS status
    , STU.LastModifiedDate::timestamp with time zone                                                         						AS "dateLastModified"
	, STU.birthDate as  "birthDate"
	, CASE WHEN SED.CodeValue = 'Female' THEN 'female' ELSE 'male' END																AS sex
	, CAST(CASE WHEN SRA.CodeValues like '%American Indian - Alaska Native%' THEN 'true' ELSE 'false' END AS VARCHAR)						AS "americanIndianOrAlaskaNative"
	, CAST(CASE WHEN SRA.CodeValues like '%Asian%' THEN 'true' ELSE 'false' END AS VARCHAR)												AS asian
    , CAST(CASE WHEN SRA.CodeValues like '%Black - African American%' THEN 'true' ELSE 'false' END AS VARCHAR)							AS "blackOrAfricanAmerican"
    , CAST(CASE WHEN SRA.CodeValues like '%Native Hawaiian - Pacific Islander%' THEN 'true' ELSE 'false' END AS VARCHAR)					AS "nativeHawaiianOrOtherPacificIslander"
	, CAST(CASE WHEN SRA.CodeValues like '%White%' THEN 'true' ELSE 'false' END AS VARCHAR)												AS white
    , CAST(CASE WHEN SRA.CodeValues like '%Other%'  or   SRA.CodeValues like '%Two or More Races%'    or   SRA.CodeValues is null or SRA.CodeValues like '%,%' THEN 'true' ELSE 'false' END AS VARCHAR)		AS "demographicRaceTwoOrMoreRaces"
		, CAST(CASE WHEN SEOA.HispanicLatinoEthnicity = true or SRA.CodeValues like '%Hispanic or Latino%' THEN 'true' ELSE 'false' END AS VARCHAR)	AS "hispanicOrLatinoEthnicity"

FROM edfi.Student STU
INNER JOIN edfi.StudentSchoolAssociation SSA 	ON STU.StudentUSI = SSA.StudentUSI
LEFT JOIN edfi.Descriptor SED 	ON STU.BirthSexDescriptorId = SED.DescriptorId
INNER JOIN edfi.StudentEducationOrganizationAssociation SEOA 	ON STU.StudentUSI = SEOA.StudentUSI

left join (SELECT edfi.studenteducationorganizationassociationrace.studentusi
,  string_agg(edfi.Descriptor.codevalue::text, ',')  CodeValues
FROM edfi.studenteducationorganizationassociationrace
LEFT JOIN edfi.Descriptor  	ON edfi.studenteducationorganizationassociationrace.RaceDescriptorId = edfi.Descriptor.DescriptorId
GROUP BY edfi.studenteducationorganizationassociationrace.studentusi
) SRA ON STU.studentusi = SRA.studentusi

join edfi.StudentSchoolAssociation SSAT on SSAT.StudentUSI = stu.StudentUSI
where SSAT.exitwithdrawdate is null