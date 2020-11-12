using EdFi.OneRoster.Persistence.EntityFrameWork;
using Newtonsoft.Json;

namespace EdFi.OneRoster.WebApi.Services.Models
{
    public class AcademicSessionModel : academicsessions
    {
        public new ParentModel parent { get; set; }
        public new string startDate { get; set; }
        public new string endDate { get; set; }

        public new string schoolYear { get; set; }

        public AcademicSessionModel()
        {
        }
        public  AcademicSessionModel(academicsessions dbElement)
        {
            sourcedId = dbElement.sourcedId;
            status = dbElement.status;
            dateLastModified = dbElement.dateLastModified;
            title = dbElement.title;
            
            type = dbElement.type;
            schoolYear = dbElement.schoolYear.ToString();
            parent = dbElement.parent == null ? null : JsonConvert.DeserializeObject<ParentModel>(dbElement.parent); 

            startDate = dbElement.startDate.Value.ToString("yyyy-MM-dd");
            endDate = dbElement.endDate.Value.ToString("yyyy-MM-dd");
        }
    }
}
