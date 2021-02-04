
drop view if exists onerosterv11.enrollments;

CREATE VIEW onerosterv11.enrollments
AS


(
SELECT DISTINCT
	eo.educationOrganizationId																			AS "SchoolId"
	,eo.nameofinstitution																				AS "NameOfInstitution"
	,SSA.Id																								AS "sourcedId"
	, 'active'																							AS status
	,SEC.LastModifiedDate::timestamp with time zone														AS "dateLastModified"
	, 'teacher'																							as role
	, CAST(CASE WHEN UPPER(CD.CodeValue) = UPPER('Teacher Of Record') THEN true ELSE false END AS boolean)    		AS primary
	, CONCAT('{ "sourcedId": "',(CASE WHEN SSA.ClassroomPositionDescriptorId IS NULL THEN 'a' ELSE 't' END),CAST(sta.StaffUniqueId AS VARCHAR) , '"}')									as user
	, CONCAT('{ "sourcedId":"', SEC.Id, '" }')															AS class
	, CONCAT('{ "sourcedId":"', EO.id, '"  }')															AS school
	, SSA.BeginDate::timestamp with time zone																AS "beginDate"
	, SSA.EndDate::timestamp with time zone	 																AS "endDate"	
	, CONCAT((CASE WHEN SSA.ClassroomPositionDescriptorId IS NULL THEN 'a' ELSE 't' END),CAST(sta.StaffUniqueId AS VARCHAR) ) as id
		, SEC.Id classid
FROM 
edfi.Staff sta
join edfi.StaffSectionAssociation SSA on  sta.staffusi = ssa.staffusi
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
	
	join edfi.staffeducationorganizationassignmentassociation seoaa on sta.staffusi = seoaa.staffusi
	
	
	
where
exists(select 1 from  
edfi.StudentSchoolAssociation SSAT where  SSAT.studentusi = STSA.studentusi 
and SSAT.exitwithdrawdate is null 
order by SSAT.createdate desc  limit 1
)
and seoaa.BeginDate<=CURRENT_DATE and (seoaa.EndDate is null OR seoaa.EndDate >= CURRENT_DATE)

)  
UNION ALL
(
SELECT DISTINCT
	eo.educationOrganizationId																				AS "SchoolId"
	,eo.nameofinstitution																					AS nameofinstitution
	,STSA.Id																								AS "sourcedId"
	, 'active'	AS status
	, SEC.LastModifiedDate::timestamp with time zone														AS "dateLastModified"
	, 'student'																								AS role
	, CAST(0 AS boolean)																					AS primary
	, CONCAT('{ "sourcedId": "s' , CAST(stu.StudentUniqueId AS VARCHAR)	 , '" }')								AS user
	, CONCAT('{ "sourcedId": "',SEC.Id, '" }')																AS class
	, CONCAT('{ "sourcedId": "',EO.id, '"}')																AS school
	, SSA.BeginDate::timestamp with time zone																AS "beginDate"
	, SSA.EndDate::timestamp with time zone	 																AS "endDate"	
	, concat('s' , CAST(stu.StudentUniqueId AS VARCHAR)	) as id
	, SEC.Id classid

FROM  
edfi.student stu 
JOIN edfi.StudentSectionAssociation AS STSA on stu.studentusi = STSA.studentusi
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
AND stsa.begindate <= current_date
AND (current_date <= stsa.enddate OR stsa.enddate IS NULL)
)
