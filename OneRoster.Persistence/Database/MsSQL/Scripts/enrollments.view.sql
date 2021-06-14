
CREATE OR ALTER VIEW onerosterv11.enrollments
AS


(
SELECT DISTINCT
	eo.educationOrganizationId																			AS "SchoolId"
	,eo.nameofinstitution																				AS "NameOfInstitution"
	,SSA.Id																								AS "sourcedId"
	, 'active'																							AS status
	, SEC.LastModifiedDate 														AS "dateLastModified"
	, 'teacher'																							as role
	, CAST(CASE WHEN UPPER(CD.CodeValue) = UPPER('Teacher Of Record') THEN 1 ELSE 0 END AS  bit)    		AS 'primary'
	, CONCAT('{ "sourcedId": "',(CASE WHEN SSA.ClassroomPositionDescriptorId IS NULL THEN 'a' ELSE 't' END),CAST(ssa.staffusi AS VARCHAR) , '"}') as 'user'
	, CONCAT('{ "sourcedId":"', SEC.Id, '" }')															AS class
	, CONCAT('{ "sourcedId":"', EO.id, '"  }')															AS school
	,  SSA.BeginDate  															AS "beginDate"
	,  SSA.EndDate																AS "endDate"	
	, CONCAT((CASE WHEN SSA.ClassroomPositionDescriptorId IS NULL THEN 'a' ELSE 't' END),CAST(ssa.staffusi AS VARCHAR) ) as id
		, SEC.Id classid
FROM edfi.StaffSectionAssociation SSA 	
JOIN  edfi.Section SEC
	ON SSA.SectionIdentifier = SEC.SectionIdentifier 
	AND SEC.SessionName = SSA.SessionName
	AND SEC.schoolid = SSA.schoolid
	and sec.schoolyear = ssa.schoolyear
	and sec.localcoursecode = ssa.localcoursecode	
LEFT JOIN edfi.EducationOrganization EO on SEC.SchoolId = EO.educationOrganizationId
LEFT JOIN edfi.Descriptor CD  	ON SSA.ClassroomPositionDescriptorId = CD.DescriptorId
 JOIN edfi.StudentSectionAssociation AS STSA
	ON STSA.SectionIdentifier = SSA.SectionIdentifier 
	and STSA.LocalCourseCode = SSA.LocalCourseCode
	AND STSA.SchoolId = SSA.SchoolId
	AND STSA.SchoolYear = SSA.SchoolYear
	AND STSA.SessionName = SSA.SessionName
	
	
	
where
exists(select TOP(1)1 from  
edfi.StudentSchoolAssociation SSAT where  SSAT.studentusi = STSA.studentusi 
and SSAT.exitwithdrawdate is null 
order by SSAT.createdate desc 
)

)  
UNION ALL
(
SELECT DISTINCT
	eo.educationOrganizationId																				AS "SchoolId"
	,eo.nameofinstitution																					AS nameofinstitution
	,STSA.Id																								AS "sourcedId"
	, 'active'	AS status
	,  SEC.LastModifiedDate													AS "dateLastModified"
	, 'student'																								AS role
	, CAST(0 AS bit)																					AS 'primary'
	, CONCAT('{ "sourcedId": "s' , CAST(STSA.studentusi AS VARCHAR)	 , '" }')								AS 'user'
	, CONCAT('{ "sourcedId": "',SEC.Id, '" }')																AS class
	, CONCAT('{ "sourcedId": "',EO.id, '"}')																AS school
	, CONVERT(VARCHAR(10),SSA.BeginDate, 107) 															AS "beginDate"
	, CONVERT(VARCHAR(10),SSA.EndDate, 107)																AS "endDate"	
	, concat('s' , CAST(STSA.studentusi AS VARCHAR)	) as id
	, SEC.Id classid

FROM  edfi.StudentSectionAssociation AS STSA
 JOIN edfi.StaffSectionAssociation SSA  	 
	ON STSA.SectionIdentifier = SSA.SectionIdentifier 
	and STSA.LocalCourseCode = SSA.LocalCourseCode
	AND STSA.SchoolId = SSA.SchoolId
	AND STSA.SchoolYear = SSA.SchoolYear
	AND STSA.SessionName = SSA.SessionName
-- Section
 JOIN  edfi.Section SEC
	ON SEC.SectionIdentifier = STSA.SectionIdentifier 
	 and SEC.LocalCourseCode = STSA.LocalCourseCode
	AND SEC.SchoolId = STSA.SchoolId
	AND SEC.SchoolYear = STSA.SchoolYear	
	AND SEC.SessionName = STSA.SessionName	
LEFT JOIN edfi.EducationOrganization EO ON SEC.SchoolId = EO.educationOrganizationId
join edfi.StudentSchoolAssociation SSAT on SSAT.StudentUSI = STSA.StudentUSI
where SSAT.exitwithdrawdate is null
AND stsa.begindate <= GETDATE()
AND ( GETDATE() <= stsa.enddate OR stsa.enddate IS NULL)
)
