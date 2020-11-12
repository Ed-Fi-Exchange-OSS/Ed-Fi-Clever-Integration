using System.Collections.Generic;
using EdFi.OneRoster.Persistence.EntityFrameWork;
using Newtonsoft.Json;

namespace EdFi.OneRoster.WebApi.Services.Models
{
    public class CourseModel :courses
    {
        public new ParentModel schoolYear { get; set; }

        public new List<string> grades { get; set; }
        public new List<string> subjects { get; set; }

        public new ParentModel org { get; set; }

        public CourseModel()
        {
        }
        public CourseModel (courses courses)
        {
            sourcedId = courses.sourcedId;
            status = courses.status;
            dateLastModified = courses.dateLastModified;
            title = courses.title;
            schoolYear = JsonConvert.DeserializeObject<ParentModel>(courses.schoolYear);
            courseCode = courses.courseCode;
            grades = JsonConvert.DeserializeObject<List<string>>(courses.grades);
            subjects = JsonConvert.DeserializeObject<List<string>>(courses.subjects);
            org = JsonConvert.DeserializeObject<ParentModel>(courses.org);
        }
    }

}
