CREATE OR ALTER VIEW onerosterv11.orgs
AS
--Types: department, school,district,local,state,national
SELECT 
	EO.educationorganizationid 												AS "SchoolId"
	,EO.nameofinstitution  															AS "NameOfInstitution"
	,EO.Id																AS "sourcedId"
	, CASE WHEN SD.CodeValue = 'Active' THEN 'active'
		   WHEN SD.CodeValue IS NULL THEN 'active' --Usually LEAs are null which means active.
	  ELSE 'tobedeleted' END											AS status
	,  CONVERT(VARCHAR(10),  EO.LastModifiedDate, 107) 					AS "dateLastModified"
	, EO.NameOfInstitution												AS name
	, CASE WHEN SEA.StateEducationAgencyId IS NOT NULL THEN 'state'
		   WHEN LEA.LocalEducationAgencyId IS NOT NULL THEN 'district' 
		   WHEN SCH.SchoolId IS NOT NULL THEN 'school'
	  ELSE 'district' END												AS type
    , CAST(EO.EducationOrganizationId AS VARCHAR)						AS identifier    
	, CASE 
		WHEN SPA.StateEducationAgencyId IS NOT NULL THEN SEP.Id
		WHEN LPA.LocalEducationAgencyId IS NOT NULL THEN SLP.Id
		ELSE NULL END													AS "parentId"
	
FROM edfi.EducationOrganization	AS EO
LEFT JOIN edfi.Descriptor SD ON EO.OperationalStatusDescriptorId = SD.DescriptorId
LEFT JOIN edfi.StateEducationAgency SEA ON EO.EducationOrganizationId = SEA.StateEducationAgencyId
LEFT JOIN edfi.LocalEducationAgency LEA ON EO.EducationOrganizationId = LEA.LocalEducationAgencyId
LEFT JOIN edfi.School SCH ON EO.EducationOrganizationId = SCH.SchoolId
LEFT JOIN edfi.StateEducationAgency SPA ON LEA.StateEducationAgencyId = SPA.StateEducationAgencyId
LEFT JOIN edfi.EducationOrganization SEP ON SPA.StateEducationAgencyId = SEP.EducationOrganizationId
LEFT JOIN edfi.LocalEducationAgency LPA ON SCH.LocalEducationAgencyId = LPA.LocalEducationAgencyId
LEFT JOIN edfi.EducationOrganization SLP ON LPA.LocalEducationAgencyId = SLP.EducationOrganizationId;