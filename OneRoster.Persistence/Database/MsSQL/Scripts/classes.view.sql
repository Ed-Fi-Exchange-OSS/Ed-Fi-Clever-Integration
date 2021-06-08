drop view if exists onerosterv11.classes;

CREATE VIEW onerosterv11.classes
AS
SELECT DISTINCT
	EDO.educationorganizationid 																								AS "SchoolId"
	,EDO.nameofinstitution  																									AS "NameOfInstitution"
	,SEC.Id																														AS "sourcedId"
	, 'active'																													AS status
	, SEC.LastModifiedDate::timestamp with time zone																			AS "dateLastModified"
	, CONCAT(COU.courseTitle, ' - ',STA.LastSurname,' - ', SEC.SectionIdentifier)											AS title
	, TRIM(SEC.SectionIdentifier)																								AS "classCode"
	, CASE WHEN STU.HomeroomIndicator = '1' THEN 'homeroom' ELSE 'scheduled' END												AS "classType"
  , CASE WHEN SEC.LocationClassroomIdentificationCode	is null THEN '' ELSE  SEC.LocationClassroomIdentificationCode END		AS location
    , CONCAT('[', (SELECT STRING_AGG(
			CASE 
				WHEN  COGLD.CodeValue = 'Infant/toddler' THEN '"IT"' 
				WHEN  COGLD.CodeValue = 'Preschool/Prekindergarten' THEN '"PR/PK"' 
				WHEN  COGLD.CodeValue = 'Transitional Kindergarten' THEN '"TK"' 
				WHEN  COGLD.CodeValue = 'Kindergarten' THEN '"KG"' 
				WHEN  COGLD.CodeValue = 'First grade' THEN '"01"'
				WHEN  COGLD.CodeValue = 'Second grade' THEN '"02"'
				WHEN  COGLD.CodeValue = 'Third grade' THEN '"03"'
				WHEN  COGLD.CodeValue = 'Fourth grade' THEN '"04"' 
				WHEN  COGLD.CodeValue = 'Fifth grade' THEN '"05"' 
				WHEN  COGLD.CodeValue = 'Sixth grade' THEN '"06"' 
				WHEN  COGLD.CodeValue = 'Seventh grade' THEN '"07"' 
				WHEN  COGLD.CodeValue = 'Eighth grade' THEN '"08"' 
				WHEN  COGLD.CodeValue = 'Ninth grade' THEN '"09"' 
				WHEN  COGLD.CodeValue = 'Tenth grade' THEN '"10"' 
				WHEN  COGLD.CodeValue = 'Eleventh grade' THEN '"11"' 
				WHEN  COGLD.CodeValue = 'Twelfth grade' THEN '"12"'
				WHEN  COGLD.CodeValue = 'Grade 13' THEN '"13"' 
				WHEN  COGLD.CodeValue = 'Postsecondary' THEN '"PS"' 
				WHEN  COGLD.CodeValue = 'Ungraded' THEN '"UG"' 
				WHEN  COGLD.CodeValue = 'Other' THEN '"Other"' 
				ELSE 'NA'
			END
			, ', ')
	  FROM edfi.CourseOfferedGradeLevel AS COG
	  INNER JOIN edfi.Descriptor AS COGLD ON COG.GradeLevelDescriptorId = COGLD.DescriptorId 
		WHERE COG.CourseCode=COU.CourseCode and COG.EducationOrganizationId=COU.EducationOrganizationId) ,']')					AS grades
	, CASE WHEN ASD.CodeValue IS NULL THEN '[""]' ELSE  CONCAT('[','"',ASD.CodeValue,'"',']')  END 								AS subjects
    , CONCAT('{ "sourcedId": "', COU.Id, '"  }')																				AS course
	, CONCAT('{ "sourcedId": "', EDO.Id, '"  }')																				AS school
	, CONCAT('[{ "sourcedId": "', SES.Id, '" }]')																				AS terms
FROM edfi.Section AS SEC
INNER JOIN edfi.EducationOrganization EDO ON SEC.SchoolId = EDO.EducationOrganizationId
INNER JOIN edfi.CourseOffering AS CO 
	ON	SEC.LocalCourseCode=CO.LocalCourseCode 
	AND SEC.SchoolId=CO.SchoolId 
	AND SEC.SchoolYear=CO.SchoolYear 
	AND SEC.SessionName=CO.SessionName
INNER JOIN edfi.Course AS COU 
	ON COU.CourseCode = CO.CourseCode
	AND COU.EducationOrganizationId = CO.EducationOrganizationId
left JOIN edfi.Descriptor AS ASD ON COU.AcademicSubjectDescriptorId = ASD.DescriptorId
 JOIN edfi.StudentSectionAssociation AS STU
	ON SEC.LocalCourseCode = STU.LocalCourseCode
	AND SEC.SchoolId = STU.SchoolId
	AND SEC.SchoolYear = STU.SchoolYear
	AND SEC.SectionIdentifier = STU.SectionIdentifier
	AND SEC.SessionName = STU.SessionName
 JOIN edfi.StaffSectionAssociation AS SSA
	ON SEC.LocalCourseCode = SSA .LocalCourseCode
	AND SEC.SchoolId = SSA.SchoolId
	AND SEC.SchoolYear = SSA.SchoolYear
	AND SEC.SectionIdentifier = SSA.SectionIdentifier
	AND SEC.SessionName = SSA.SessionName
 JOIN edfi.Staff AS STA ON SSA.StaffUSI = STA.StaffUSI
LEFT JOIN edfi.Descriptor AS CPD ON SSA.ClassroomPositionDescriptorId = CPD.DescriptorId
INNER JOIN edfi.Session AS SES 
	ON SEC.SchoolId=SES.SchoolId 
	AND SEC.SchoolYear=SES.SchoolYear 
	AND SEC.SessionName=SES.SessionName

WHERE 
exists(select 1 from  
edfi.StudentSchoolAssociation SSAT where  SSAT.studentusi = STU.studentusi 
and SSAT.exitwithdrawdate is null 
order by SSAT.createdate desc  limit 1
)
and UPPER(CPD.CodeValue) = UPPER('Teacher Of Record') 
GROUP BY	EDO.educationorganizationid ,SEC.Id, SEC.LastModifiedDate, SEC.LocalCourseCode, SEC.SequenceOfCourse, SEC.LocationClassroomIdentificationCode, 
			SEC.SchoolId, SEC.SchoolYear, SEC.SectionIdentifier, SEC.SessionName,
			COU.Id, COU.CourseTitle, COU.CourseCode, COU.EducationOrganizationId, 
			STA.LastSurname, STA.FirstName, STA.MiddleName, 
			STU.HomeroomIndicator, 
			EDO.Id, ASD.CodeValue, SES.id
ORDER BY "sourcedId" OFFSET 0 ROWS;

