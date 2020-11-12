using System.Text.Json.Serialization;
using EdFi.OneRoster.Persistence.EntityFrameWork;
using Newtonsoft.Json;

namespace EdFi.OneRoster.WebApi.Services.Models
{
    public class EnrollmentModel :enrollments
    {

        public new ParentModel school { get; set; }

        [JsonPropertyName( "class")]
        public new ParentModel _class { get; set; }

        public new ParentModel user { get; set; }

        public new string beginDate { get; set; }
        public new string endDate { get; set; }


        public EnrollmentModel()
        {
        }
        public  EnrollmentModel (enrollments  dbElement)
        {

            sourcedId = dbElement.sourcedId;
            status = dbElement.status;
            dateLastModified = dbElement.dateLastModified;
            role = dbElement.role;
            primary = dbElement.primary;

            if(dbElement.beginDate == null || dbElement.endDate == null)
            {

            }
            else
            {
                beginDate = dbElement.beginDate.Value.ToString("yyyy-MM-dd");
                endDate = dbElement.endDate.Value.ToString("yyyy-MM-dd");
            }
            


            school = JsonConvert.DeserializeObject<ParentModel>(dbElement.school);
            _class = JsonConvert.DeserializeObject<ParentModel>(dbElement._class);
            user= JsonConvert.DeserializeObject<ParentModel>(dbElement.user);

        }

    }
}
