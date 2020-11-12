using System.Collections.Generic;
using EdFi.OneRoster.Persistence.EntityFrameWork;
using Newtonsoft.Json;

namespace EdFi.OneRoster.WebApi.Services.Models
{

    public class ClassModel : classes
    {
        public new List<string> grades { get; set; }
        public new List<string> subjects { get; set; }

        public new ParentModel course { get; set; }
        public new ParentModel school { get; set; }
        public new List<ParentModel> terms{ get; set; }

        public ClassModel()
        {
        }
        public ClassModel (classes dbElement)
        {
            sourcedId = dbElement.sourcedId;
            status = dbElement.status;
            dateLastModified = dbElement.dateLastModified;
            title = dbElement.title;
            classCode = dbElement.classCode;
            classType = dbElement.classType;
            location = dbElement.location;
       
            
            grades = JsonConvert.DeserializeObject<List<string>>(dbElement.grades);
            subjects = JsonConvert.DeserializeObject<List<string>>(dbElement.subjects);

            course = JsonConvert.DeserializeObject<ParentModel>(dbElement.course);
            school = JsonConvert.DeserializeObject<ParentModel>(dbElement.school);
            terms = JsonConvert.DeserializeObject<List<ParentModel>>(dbElement.terms);
        }
    }
}
