using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EdFi.OneRoster.WebApi.Services.Models;

namespace EdFi.OneRoster.WebApi.Services
{
    public class OneRosterStaticDataService : IOneRosterService
    {
        DateTime lastDate = DateTime.Now;

        public async Task<ResponseModel<List<AcademicSessionModel>>> GetAcademicSessionsAsync(RequestModel request)
        {

            var response = await Task.Run(() => new ResponseModel<List<AcademicSessionModel>>
            {
                TotalCount = 5,
                Response = new List<AcademicSessionModel>
                {                     
                    new AcademicSessionModel {
                        sourcedId = new Guid("a0000000-0000-0000-0000-000000000001"),
                        dateLastModified = lastDate,
                        startDate = "2020-08-19",
                        endDate = "2021-05-21",
                        parent = null,
                        schoolYear = "2021",
                        status ="active",
                        title ="2020-2021 School Year",
                        type = "schoolYear",                        
                    },
                    new AcademicSessionModel {
                        sourcedId = new Guid("a0000000-0000-0000-0000-000000000002"),
                        dateLastModified = lastDate,
                        startDate = "2020-08-19",
                        endDate = "2021-12-18",
                        parent = new ParentModel(){sourcedId ="a0000000-0000-0000-0000-000000000001"},
                        schoolYear = "2021",
                        status ="active",
                        title ="2020-2021 Fall",
                        type = "term",
                    },
                    new AcademicSessionModel {
                        sourcedId = new Guid("a0000000-0000-0000-0000-000000000003"),
                        dateLastModified = lastDate,
                        startDate = "2021-01-02",
                        endDate = "2021-05-21",
                        parent = new ParentModel(){sourcedId ="a0000000-0000-0000-0000-000000000001"},
                        schoolYear = "2021",
                        status ="active",
                        title ="2020-2021 Spring",
                        type = "term",
                    },
                    new AcademicSessionModel {
                        sourcedId = new Guid("a0000000-0000-0000-0000-000000000004"),
                        dateLastModified = lastDate,
                        startDate = "2020-08-19",
                        endDate = "2020-12-18",
                        parent = new ParentModel(){sourcedId ="a0000000-0000-0000-0000-000000000002"},
                        schoolYear = "2021",
                        status ="active",
                        title ="2020-2021 fall-first grading period",
                        type = "gradingPeriod",
                    },
                    new AcademicSessionModel {
                        sourcedId = new Guid("a0000000-0000-0000-0000-000000000005"),
                        dateLastModified = lastDate,
                        startDate = "2020-08-19",
                        endDate = "2021-05-21",
                        parent = new ParentModel(){sourcedId ="a0000000-0000-0000-0000-000000000003"},

                        schoolYear = "2021",
                        status ="active",
                        title ="2020-2021 fall-first grading period",
                        type = "gradingPeriod",
                    },


                }
            });

            return response;
        }

        public async Task<ResponseModel<string>> GetCampusVersionAsync(RequestModel request)
        {
            var response = await Task.Run(() => new ResponseModel<string>
            {
                TotalCount = 1,
                Response = "Campus.2016.6"
            });

            return response;
        }

        public async Task<ResponseModel<List<ClassModel>>> GetClassesAsync(RequestModel request)
        {
            var response = await Task.Run(() => new ResponseModel<List<ClassModel>>
            {
                TotalCount = 1,
                Response = new List<ClassModel>
                {
                    new ClassModel
                    {
                        sourcedId = new Guid("009afd73-92cd-4de8-9872-e54f7e8810ac"),
                        status = "active",
                        dateLastModified = lastDate,
                        title = "SCI-03 1 Science, Grade 3",
                        classCode = "SCI-03-Cherry, Eric",
                        classType = "scheduled",
                        location = "room 202",
                        grades = new List<string>{"09","10","11"},
                        subjects = new List<string> {"Science"},
                        course = new ParentModel{sourcedId="06bdb94f-9abe-40d7-ac6d-21da5046bd9d"},
                        school = new ParentModel {sourcedId="14ac6b32-cba5-4aba-900f-a0c54be6907e"},
                        terms = new List<ParentModel>{new ParentModel { sourcedId = "a0000000-0000-0000-0000-000000000002" } }
                    }
                }
            });

            return response;
        }
        public async Task<ResponseModel<List<CourseModel>>> GetCoursesAsync(RequestModel request)
        {
            var response = await Task.Run(() => new ResponseModel<List<CourseModel>>
            {
                TotalCount = 1,
                Response = new List<CourseModel>
                {
                    new CourseModel
                    {
                        sourcedId = new Guid("06bdb94f-9abe-40d7-ac6d-21da5046bd9d"),
                        status = "active",
                        dateLastModified = lastDate,
                        title = "Multimedia",                        
                        grades = new List<string>{"09","10","11"},
                        subjects = new List<string> {"Science"},
                        schoolYear = new ParentModel{sourcedId="cac22462-fd27-4af6-9a9a-1abd6ed30e63"},
                        org = new ParentModel {sourcedId="14ac6b32-cba5-4aba-900f-a0c54be6907e"},
                        courseCode ="TAMULTIM",
                        
                    }
                }
            });

            return response;
        }

        public async Task<ResponseModel<List<DemographicModel>>> GetDemographicsAsync(RequestModel request)
        {
            var response = await Task.Run(() => new ResponseModel<List<DemographicModel>>
            {
                TotalCount = 2,
                Response = new List<DemographicModel>
                {
                    new DemographicModel
                    {
                        sourcedId = "s10001",
                        status = "active",
                        dateLastModified =lastDate,
                        birthDate= "2000-04-11",
                        sex = "male",
                        americanIndianOrAlaskaNative = "false",
                        asian="false",
                        nativeHawaiianOrOtherPacificIslander = "false",
                        white = "true",
                        demographicRaceTwoOrMoreRaces = "false",
                        hispanicOrLatinoEthnicity = "false",
                        blackOrAfricanAmerican = "false",             
                    },
                        new DemographicModel
                    {
                        sourcedId = "t10001",
                        status = "active",
                        dateLastModified =lastDate,
                        birthDate= "2000-04-11",
                        sex = "male",
                        americanIndianOrAlaskaNative = "false",
                        asian="false",
                        nativeHawaiianOrOtherPacificIslander = "false",
                        white = "true",
                        demographicRaceTwoOrMoreRaces = "false",
                        hispanicOrLatinoEthnicity = "false",
                        blackOrAfricanAmerican = "false",
                    }
                }
            });

            return response;
        }

        public async Task<ResponseModel<List<EnrollmentModel>>> GetEnrollmentsAsync(RequestModel request)
        {
            var response = await Task.Run(() => new ResponseModel<List<EnrollmentModel>>
            {
                TotalCount = 2,
                Response = new List<EnrollmentModel>
                {
                    new EnrollmentModel
                    {
                        sourcedId = new Guid("2c87ab8a-2daa-4c05-a239-8d331a2a409e"),
                        status = "active",
                        dateLastModified = lastDate,
                        role="student",
                        primary =false,
                        user = new ParentModel(){sourcedId = "s10001"},
                        beginDate = "2020-08-12",
                        endDate ="2020-12-18",
                        _class = new ParentModel{sourcedId = "009afd73-92cd-4de8-9872-e54f7e8810ac"},                       
                       
                        school = new ParentModel {sourcedId="14ac6b32-cba5-4aba-900f-a0c54be6907e"},
                    },
                    new EnrollmentModel
                    {
                        sourcedId = new Guid("2c87ab8a-2daa-4c05-a239-8d331a2a409c"),
                        status = "active",
                        dateLastModified = lastDate,
                        role="teacher",
                        primary = true,
                        user = new ParentModel(){sourcedId = "t10001"},
                        beginDate = "2020-08-12",
                        endDate ="2020-12-18",
                        _class = new ParentModel{sourcedId = "009afd73-92cd-4de8-9872-e54f7e8810ac"},

                        school = new ParentModel {sourcedId="14ac6b32-cba5-4aba-900f-a0c54be6907e"},
                    },

                }
            });

            return response;
        }

        public async Task<ResponseModel<List<OrgModel>>> GetOrgsAsync(RequestModel request)
        {
            var response = await Task.Run(() => new ResponseModel<List<OrgModel>>
            {
                TotalCount = 2,
                Response = new List<OrgModel>
                {
                    new OrgModel
                    {
                        sourcedId = "57537f3e-7008-4620-ac40-5cce8029cf13",
                        status = "active",
                        dateLastModified =lastDate,
                        name = "Grand Bend ISD",
                        type = "district",
                        identifier = "255901",                       

                    },
                    new OrgModel
                    {
                        sourcedId = "14ac6b32-cba5-4aba-900f-a0c54be6907e",
                        status = "active",
                        dateLastModified =lastDate,
                        name = "Grand Bend Highschool",
                        type = "school",
                        identifier ="255901",
                        parent = new ParentModel(){sourcedId = "57537f3e-7008-4620-ac40-5cce8029cf13" }

                    }
                }
            });

            return response;
        }

        public async Task<ResponseModel<List<UserModel>>> GetUsersAsync(RequestModel request)
        {
            var response = await Task.Run(() => new ResponseModel<List<UserModel>>
            {
                TotalCount = 2,
                Response = new List<UserModel>
                {
                    new UserModel
                    {
                        sourcedId = "s10001",
                        status = "active",
                        dateLastModified =lastDate,
                        username ="jreid",
                        enabledUser=true,
                        givenName ="Janet",
                        middleName = "Annette",
                        familyName = "Reid",
                        role ="student",
                        identifier = "207232",
                        email = "JanetReid@edfi.org",
                        orgs = new List<ParentModel>{ new ParentModel {sourcedId= "14ac6b32-cba5-4aba-900f-a0c54be6907e"} },
                        phone = "",
                        sms = "",
                        grades = new List<string>{"01"}
                        

                    },
                    new UserModel
                    {
                        sourcedId = "t10001",
                        status = "active",
                        dateLastModified =lastDate,
                        username ="teacher1",
                        enabledUser=true,
                        givenName ="Teacher",
                        middleName = "Annette",
                        familyName = "Reid",
                        role ="teacher",
                        identifier = "207232",
                        email = "JanetReid@edfi.org",
                        orgs = new List<ParentModel>{ new ParentModel {sourcedId= "14ac6b32-cba5-4aba-900f-a0c54be6907e"} },
                        phone = "",
                        sms = "",                        
                    }
                }
            });

            return response;
        }


    }
}
