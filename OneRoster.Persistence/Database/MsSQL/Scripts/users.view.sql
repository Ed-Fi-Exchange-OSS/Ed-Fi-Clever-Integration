

/****** Object:  View [onerosterv11].[users]    Script Date: 6/10/2021 2:54:50 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO






--drop view if exists onerosterv11.users;


-- STUDENTS AND TEACHERS
CREATE OR ALTER  VIEW [onerosterv11].[users]
AS
--OTHERS AIDE
(select 

		--ssa.schoolid THEREIS NO SSCHOOLID WE USE seoaa.educationorganizationid
		CAST(seoaa.educationorganizationid as int)			as  "SchoolId"
		--seoaa.educationorganizationid							as  "SchoolId"
		,eo.nameofinstitution									as  "NameOfInstitution"
		--, ssa.schoolyear	cant find schoolyear from ssa
		,2021													as schoolyear 
		--,  concat(seoaa.positiontitle, ' - ' ,scd.codevalue ) as TeacherPosition
		, concat('t',sta.staffusi) 								as "sourcedId"		
		, 'active' 												as "status" 
		--, sta.LastModifiedDate/*::timestamp with time zone*/  	as "dateLastModified"
		,CAST(sta.LastModifiedDate AS datetime)			  		as "dateLastModified"
		, case when sta.LoginId is null then concat('t',CAST(STA.staffusi as VARCHAR)) else STA.LoginId end as "username"
		-- Assumption: If a teacher is teaching a course they are enabled.
		, CONVERT(bit,1) 													as "enabledUser"
		, sta.FirstName											as "givenName"
		, case when STA.MiddleName is null then '' else STA.MiddleName end as "middleName"
		, sta.LastSurname										as "familyName"
		, 'aide'												as "role"
		, sta.StaffUniqueId										as "identifier"
		, COALESCE ((select TOP 1 ElectronicMailAddress from edfi.StaffElectronicMail SEMT where SEMT.staffusi=STA.staffusi /*limit 1*/) ,'')	as email
		, 'yep'													as sms
		, 'yep'													as phone
		, CONCAT('[{ "sourcedId": "', eo.id, '" }]')			as orgs
		, '[]'													as grades		

from
edfi.staff sta
join edfi.staffeducationorganizationassignmentassociation seoaa on sta.staffusi = seoaa.staffusi
left join edfi.educationorganization  eo on seoaa.educationorganizationid = eo.educationorganizationid
inner join edfi."descriptor" scd on seoaa.staffclassificationdescriptorid = scd.descriptorid
where not exists(select ssa.staffusi from  edfi.staffsectionassociation ssa where sta.staffusi = ssa.staffusi)

)

UNION ALL

-- TEACHERS // STAFF
-- teachers
(select 
		CAST(ssa.schoolid as int)									as  "SchoolId"
		--ssa.schoolid												as  "SchoolId"
		, eo.nameofinstitution										as  "NameOfInstitution"
		, ssa.schoolyear											as schoolyear
		--, ssa.localcoursecode, ssa.begindate, ssa.enddate 
		--, cpd.codevalue as TeacherPosition
		, concat('t',sta.staffusi) 									as "sourcedId"
		-- Database does not have a begin and enddate for the section so we cant calcualte active.
		, 'active' 													as "status" 
		--, sta.LastModifiedDate/*::timestamp with time zone*/  	as "dateLastModified"
		,CAST(sta.LastModifiedDate AS datetime)						as "dateLastModified"
		, case when sta.LoginId is null then concat('t',CAST(STA.staffusi as VARCHAR)) else STA.LoginId end as "username"
		-- Assumption: If a teacher is teaching a course they are enabled.
		, CONVERT(bit,1) 													as "enabledUser"
		, sta.FirstName												as "givenName"
		, case when STA.MiddleName is null then '' else STA.MiddleName end as "middleName"
		, sta.LastSurname											as "familyName"
		, 'teacher'													as "role"
		, sta.StaffUniqueId											as "identifier"
		, COALESCE ((select TOP 1 ElectronicMailAddress from edfi.StaffElectronicMail SEMT where SEMT.staffusi=STA.staffusi) ,'')	as email
		, 'yep' 														as sms
		, 'yep'														as phone
		, CONCAT('[{ "sourcedId": "', eo.id, '" }]')				as orgs
		, '[]'														as grades
from edfi.staff sta
inner join edfi.staffsectionassociation ssa on sta.staffusi = ssa.staffusi
inner join edfi.educationorganization  eo on ssa.schoolid = eo.educationorganizationid
inner join edfi."descriptor" cpd on ssa.classroompositiondescriptorid = cpd.descriptorid 
inner join edfi.schoolyeartype syt on ssa.schoolyear = syt.schoolyear and ssa.schoolyear = syt.currentschoolyear 
--where ssa.schoolid = 84202
group by ssa.schoolid, eo.id, eo.educationorganizationid, eo.nameofinstitution, ssa.schoolyear, sta.staffusi, sta.firstname, sta.lastsurname, cpd.codevalue,sta.LoginId, sta.MiddleName,sta.StaffUniqueId,sta.LastModifiedDate
--ssa.localcoursecode, ssa.begindate, ssa.enddate
--order by ssa.schoolid, sta.staffusi
)

UNION ALL
(

select 
		CAST(ssa.schoolid as int)											as  "SchoolId"
		--ssa.schoolid														as  "SchoolId"
		, eo.nameofinstitution												as  "NameOfInstitution"
		, ssa.schoolyear													as schoolyear
		--, '' as TeacherPosition
		, concat('s',stu.studentusi) 										as "sourcedId"
		-- Database does not have a begin and enddate for the section so we cant calcualte active.
		, 'active' 															as "status" 
		--, stu.LastModifiedDate/*::timestamp with time zone*/  	as "dateLastModified"
		,CAST(stu.LastModifiedDate AS datetime)			  					as "dateLastModified"
		, case when SEOA.LoginId is null then concat('s',CAST(stu.studentusi as VARCHAR)) else SEOA.LoginId end as "username"
		-- Assumption: If a teacher is teaching a course they are enabled.
		, CONVERT(bit,1) 														as "enabledUser"
		, STU.FirstName														as "givenName"
		, case when STU.MiddleName is null then '' else STU.MiddleName end  as "middleName"
		, STU.LastSurname													as "familyName"
		, 'student'															as "role"
		, STU.StudentUniqueId												as "identifier"
		, COALESCE (( 	SELECT TOP 1 ElectronicMailAddress FROM edfi.studenteducationorganizationassociationelectronicmail STUEM where STUEM.StudentUSI=STU.StudentUSI /*limit 1*/) ,'')	AS email
		, 'yep'																as sms
		, 'yep'																as phone
		, CONCAT('[{ "sourcedId": "', eo.id, '" }]')						as orgs
		, CONCAT('['
			, (CASE 
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
				ELSE  'NA' end) ,']')
																									AS grades

from edfi.student STU
join edfi.StudentSchoolAssociation SSAT on SSAT.StudentUSI = STU.StudentUSI
inner join edfi.studentsectionassociation stusa on stu.studentusi = stusa.studentusi
inner join edfi.staffsectionassociation ssa on stusa.sectionidentifier = ssa.sectionidentifier
inner join edfi.educationorganization  eo on ssa.schoolid = eo.educationorganizationid
INNER JOIN edfi.StudentEducationOrganizationAssociation SEOA 	ON STU.StudentUSI = SEOA.StudentUSI
join edfi.Descriptor AS COGLD on SSAT.entrygradeleveldescriptorid = COGLD.descriptorid
where  
SSAT.exitwithdrawdate is null
--and ssa.schoolid = 84202 
group by ssa.schoolid, eo.id, eo.educationorganizationid, eo.nameofinstitution, ssa.schoolyear, stu.studentusi,seoa.loginid, STU.firstname, STU.lastsurname, /*'sourcedId', 'status',*/ COGLD.CodeValue, stu.MiddleName, stu.StudentUniqueId, stu.LastModifiedDate
--ssa.localcoursecode, ssa.begindate, ssa.enddate
--order by ssa.schoolid, stu.studentusi

)
GO


