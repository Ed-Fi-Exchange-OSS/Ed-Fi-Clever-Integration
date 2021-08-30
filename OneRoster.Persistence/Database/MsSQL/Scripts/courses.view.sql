CREATE OR ALTER VIEW onerosterv11.courses AS
SELECT DISTINCT
	EDO.educationorganizationid														AS "SchoolId"
	,EDO.nameofinstitution															AS "NameOfInstitution"
	,COU.Id																			AS "sourcedId"
	, 'active'																		AS "status"
	,  COU.LastModifiedDate															AS "dateLastModified"
	, COU.CourseTitle																AS "title"
	, CONCAT('{ "sourcedId": "', SYT.Id,'" }')										AS "schoolYear"
	, COU.CourseCode																AS "courseCode"
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
		WHERE COG.CourseCode=COU.CourseCode and COG.EducationOrganizationId=COU.EducationOrganizationId), ']')	AS "grades"
	, CASE WHEN SUD.CodeValue IS NULL THEN '[]' ELSE  CONCAT('[','"',SUD.CodeValue,'"',']')  END				AS "subjects"
	, CONCAT('{ "sourcedId": "', Edo.Id, '" }')																	AS "org"
FROM edfi.Course AS COU
INNER JOIN  edfi.CourseOffering COO ON COU.CourseCode = COO.CourseCode	AND COu.EducationOrganizationId = COO.EducationOrganizationId
INNER JOIN edfi.Section AS SEC
	ON	SEC.LocalCourseCode=COO.LocalCourseCode 
	AND SEC.SchoolId=COO.SchoolId 
	AND SEC.SchoolYear=COO.SchoolYear 
	AND SEC.SessionName=COO.SessionName
INNER JOIN edfi.StaffSectionAssociation AS SSA
	ON SEC.LocalCourseCode = SSA .LocalCourseCode
	AND SEC.SchoolId = SSA.SchoolId
	AND SEC.SchoolYear = SSA.SchoolYear
	AND SEC.SectionIdentifier = SSA.SectionIdentifier
	AND SEC.SessionName = SSA.SessionName 	
INNER JOIN edfi.StudentSectionAssociation AS STU
	ON SEC.LocalCourseCode = STU.LocalCourseCode
	AND SEC.SchoolId = STU.SchoolId
	AND SEC.SchoolYear = STU.SchoolYear
	AND SEC.SectionIdentifier = STU.SectionIdentifier
	AND SEC.SessionName = STU.SessionName
INNER JOIN edfi.EducationOrganization EDO ON COU.EducationOrganizationId = EDO.EducationOrganizationId
INNER JOIN edfi.SchoolYearType SYT ON COO.SchoolYear = SYT.SchoolYear
LEFT JOIN edfi.AcademicSubjectDescriptor ASD ON COU.AcademicSubjectDescriptorId = ASD.AcademicSubjectDescriptorId
LEFT JOIN edfi.Descriptor SUD ON ASD.AcademicSubjectDescriptorId = SUD.DescriptorId 
where 
exists(select top(1) 1 from  
edfi.StudentSchoolAssociation SSAT where  SSAT.studentusi = STU.studentusi 
and SSAT.exitwithdrawdate is null 
order by SSAT.createdate desc )