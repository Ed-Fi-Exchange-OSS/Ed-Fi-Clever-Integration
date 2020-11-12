//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;


//namespace OneRoster.WebApi.Controllers
//{
//    [Route("dev/campus/oneroster/go/ims/oneroster/v1p1")]
//    [ApiController]
//    public class MockOneRosterController : ControllerBase
//    {
//        public int count;

//        public DateTime curDate = DateTime.Now;

//        private readonly ILogger<OneRosterController> _logger;

//        public MockOneRosterController(ILogger<OneRosterController> logger)
//        {
//            _logger = logger;
//        }

//        [HttpGet]
//        [Route("campusVersion")]
//        public async Task<string> GetCampusVersion([FromQuery] BaseRequest baseRequest)
//        {
//            var service = new EndPointService();
//            var campus = service.GetCampusVersion(baseRequest);
//            this._logger.LogInformation(campus);
//            return campus;
//        }

//        [HttpGet]
//        [Route("orgs")]
//        public object GetOrgs([FromQuery] BaseRequest baseRequest)
//        {
//            Response.Headers.Add("X-Total-Count", "2");


//            return
//                "{\"orgs\":[{\"sourcedId\":\"57537f3e-7008-4620-ac40-5cce8029cf13\",\"status\":\"active\",\"dateLastModified\":\"2020-09-11T10:52:47.769Z\",\"name\":\"Grand Bend ISD\",\"type\":\"district\",\"identifier\":\"255901\"},{\"sourcedId\":\"9823a807-ed10-4c3e-8376-ec7304424e77\",\"status\":\"active\",\"dateLastModified\":\"2020-09-11T10:52:47.769Z\",\"name\":\"Grand Bend Highschool\",\"type\":\"school\",\"identifier\":\"255901\",\"parent\":{\"sourcedId\":\"57537f3e-7008-4620-ac40-5cce8029cf13\"}}]}"
//                ;
//        }


//        [HttpGet]
//        [Route("users")]
//        public object GetUsers([FromQuery] BaseRequest baseRequest)
//        {            
//            Response.Headers.Add("X-Total-Count", "2");

//            return "{\"users\":[{\"sourcedId\":\"s329\",\"status\":\"active\",\"dateLastModified\":\"2020-09-11T10:52:47.769Z\",\"username\":\"jreid\",\"enabledUser\":true,\"givenName\":\"Janet\",\"middleName\":\"Annette\",\"familyName\":\"Reid\",\"role\":\"student\",\"identifier\":\"207232\",\"email\":\"JanetReid@edfi.org\",\"sms\":\"\",\"phone\":\"\",\"orgs\":[{\"sourcedId\":\"9823a807-ed10-4c3e-8376-ec7304424e77\",\"type\":\"org\"}]},{\"sourcedId\":\"t00028\",\"status\":\"active\",\"dateLastModified\":\"2020-09-11T10:52:47.769Z\",\"username\":\"teacher1\",\"enabledUser\":true,\"givenName\":\"Teacher\",\"middleName\":\"Annette\",\"familyName\":\"Reid\",\"role\":\"teacher\",\"identifier\":\"207232\",\"email\":\"JanetReid@edfi.org\",\"sms\":\"\",\"phone\":\"\",\"orgs\":[{\"sourcedId\":\"9823a807-ed10-4c3e-8376-ec7304424e77\"}]}]}";
//        }

//        [HttpGet]
//        [Route("courses")]
//        public object GetCourses([FromQuery] BaseRequest baseRequest)
//        {
            
//            Response.Headers.Add("X-Total-Count", "1");

//            return "{\"courses\":[{\"sourcedId\":\"06bdb94f-9abe-40d7-ac6d-21da5046bd9d\",\"status\":\"active\",\"dateLastModified\":\"2020-09-11T10:52:47.769Z\",\"title\":\"Multimedia\",\"schoolYear\":{\"sourcedId\":\"cac22462-fd27-4af6-9a9a-1abd6ed30e63\"},\"courseCode\":\"TAMULTIM\",\"grades\":[\"09\",\"10\",\"11\"],\"subjects\":[\"Science\"],\"org\":{\"sourcedId\":\"9823a807-ed10-4c3e-8376-ec7304424e77\"}}]}";
//        }

//        [HttpGet]
//        [Route("classes")]
//        public object GetClasses([FromQuery] BaseRequest baseRequest)
//        {
            
//            Response.Headers.Add("X-Total-Count", "1");

//            return "{\"classes\":[{\"sourcedId\":\"009afd73-92cd-4de8-9872-e54f7e8810ac\",\"status\":\"active\",\"dateLastModified\":\"2020-09-11T10:52:47.769Z\",\"title\":\"SCI-03 1 Science, Grade 3\",\"classCode\":\"SCI-03-Cherry, Eric\",\"classType\":\"scheduled\",\"location\":\"room 202\",\"grades\":[\"09\",\"10\",\"11\"],\"subjects\":[\"Science\"],\"course\":{\"sourcedId\":\"06bdb94f-9abe-40d7-ac6d-21da5046bd9d\"},\"school\":{\"sourcedId\":\"9823a807-ed10-4c3e-8376-ec7304424e77\"},\"terms\":[{\"sourcedId\":\"a0000000-0000-0000-0000-000000000002\"}]}]}";
//        }

//        [HttpGet]
//        [Route("enrollments")]
//        public object GetEnrollments([FromQuery] BaseRequest baseRequest)
//        {
//            var service = new EndPointService();
//            var items = service.GetEnrollments(baseRequest, out count);
//            Response.Headers.Add("X-Total-Count", "2");

//            return "{\"enrollments\":[{\"sourcedId\":\"2c87ab8a-2daa-4c05-a239-8d331a2a409e\",\"status\":\"active\",\"dateLastModified\":\"2020-09-11T10:52:47.769Z\",\"role\":\"student\",\"primary\":false,\"user\":{\"sourcedId\":\"s329\"},\"class\":{\"sourcedId\":\"009afd73-92cd-4de8-9872-e54f7e8810ac\"},\"school\":{\"sourcedId\":\"9823a807-ed10-4c3e-8376-ec7304424e77\"},\"beginDate\":\"2020-08-12\",\"endDate\":\"2020-12-18\"},{\"sourcedId\":\"2c87ab8a-2daa-4c05-a239-8d331a2a409c\",\"status\":\"active\",\"dateLastModified\":\"2020-09-11T10:52:47.769Z\",\"role\":\"teacher\",\"primary\":true,\"user\":{\"sourcedId\":\"t00028\"},\"class\":{\"sourcedId\":\"009afd73-92cd-4de8-9872-e54f7e8810ac\"},\"school\":{\"sourcedId\":\"9823a807-ed10-4c3e-8376-ec7304424e77\"},\"beginDate\":\"2020-08-12\",\"endDate\":\"2020-12-18\"}]}";
//        }

//        [HttpGet]
//        [Route("academicsessions")]
//        public object GetAcademicSessions([FromQuery] BaseRequest baseRequest)
//        {
//            var service = new EndPointService();
//            var items = service.GetAcademicSessions(baseRequest, out count);
//            Response.Headers.Add("X-Total-Count", "5");

//            return "{\"academicSessions\":[{\"sourcedId\":\"a0000000-0000-0000-0000-000000000001\",\"status\":\"active\",\"dateLastModified\":\"2020-09-11T10:52:47.769Z\",\"title\":\"2020-2021 School Year\",\"startDate\":\"2020-08-19\",\"endDate\":\"2021-05-21\",\"type\":\"schoolYear\",\"schoolYear\":\"2021\"},{\"sourcedId\":\"a0000000-0000-0000-0000-000000000002\",\"status\":\"active\",\"dateLastModified\":\"2020-09-11T10:52:47.769Z\",\"title\":\"2020-2021 Fall\",\"startDate\":\"2020-08-19\",\"endDate\":\"2020-12-18\",\"type\":\"term\",\"parent\":{\"sourcedId\":\"a0000000-0000-0000-0000-000000000001\"},\"schoolYear\":\"2021\"},{\"sourcedId\":\"a0000000-0000-0000-0000-000000000003\",\"status\":\"active\",\"dateLastModified\":\"2020-09-11T10:52:47.769Z\",\"title\":\"2020-2021 Spring\",\"startDate\":\"2021-01-02\",\"endDate\":\"2021-05-21\",\"type\":\"term\",\"parent\":{\"sourcedId\":\"a0000000-0000-0000-0000-000000000001\"},\"schoolYear\":\"2021\"},{\"sourcedId\":\"a0000000-0000-0000-0000-000000000004\",\"status\":\"active\",\"dateLastModified\":\"2020-09-11T10:52:47.769Z\",\"title\":\"2020-2021 fall-first grading period\",\"startDate\":\"2020-08-19\",\"endDate\":\"2020-12-18\",\"type\":\"gradingPeriod\",\"parent\":{\"sourcedId\":\"a0000000-0000-0000-0000-000000000002\"},\"schoolYear\":\"2021\"},{\"sourcedId\":\"a0000000-0000-0000-0000-000000000005\",\"status\":\"active\",\"dateLastModified\":\"2020-09-11T10:52:47.769Z\",\"title\":\"2020-2021 fall-first grading period\",\"startDate\":\"2020-08-19\",\"endDate\":\"2020-12-18\",\"type\":\"gradingPeriod\",\"parent\":{\"sourcedId\":\"a0000000-0000-0000-0000-000000000003\"},\"schoolYear\":\"2021\"}]}";
//        }

//        [HttpGet]
//        [Route("demographics")]
//        public object GetDemographics([FromQuery] BaseRequest baseRequest)
//        {
//            var service = new EndPointService();
//            var items = service.GetDemographics(baseRequest, out count);
//            Response.Headers.Add("X-Total-Count", "2");

//            return "{\"demographics\":[{\"sourcedId\":\"s329\",\"status\":\"active\",\"dateLastModified\":\"2020-09-11T10:52:47.769Z\",\"birthDate\":\"2000-04-11\",\"sex\":\"male\",\"americanIndianOrAlaskaNative\":\"false\",\"asian\":\"false\",\"blackOrAfricanAmerican\":\"false\",\"nativeHawaiianOrOtherPacificIslander\":\"false\",\"white\":\"true\",\"demographicRaceTwoOrMoreRaces\":\"false\",\"hispanicOrLatinoEthnicity\":\"false\",\"countryOfBirthCode\":\"US\",\"stateOfBirthAbbreviation\":\"NY\",\"cityOfBirth\":\"New York\",\"publicSchoolResidenceStatus\":\"01652\"},{\"sourcedId\":\"t00028\",\"status\":\"active\",\"dateLastModified\":\"2020-09-11T10:52:47.769Z\",\"birthDate\":\"2000-04-11\",\"sex\":\"male\",\"americanIndianOrAlaskaNative\":\"false\",\"asian\":\"false\",\"blackOrAfricanAmerican\":\"false\",\"nativeHawaiianOrOtherPacificIslander\":\"false\",\"white\":\"true\",\"demographicRaceTwoOrMoreRaces\":\"false\",\"hispanicOrLatinoEthnicity\":\"false\",\"countryOfBirthCode\":\"US\",\"stateOfBirthAbbreviation\":\"NY\",\"cityOfBirth\":\"New York\",\"publicSchoolResidenceStatus\":\"01652\"}]}";
//        }




//    }
//}
