using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EdFi.OneRoster.Persistence.EntityFrameWork
{
    public partial class academicsessions : ISchool
    {
        public Guid? sourcedId { get; set; }
        public string status { get; set; }
        public DateTime? dateLastModified { get; set; }
        public string title { get; set; }
        [JsonIgnore]
        public DateTime? startDate { get; set; }
        [JsonIgnore]
        public DateTime? endDate { get; set; }
        public string type { get; set; }
        [JsonIgnore]
        public string parent { get; set; }

        [JsonIgnore]
        public int schoolYear { get; set; }

        [NotMapped]
        [JsonIgnore]
        public int SchoolId { get; set; }

        [JsonIgnore]
        [NotMapped]
        public string NameOfInstitution { get; set; }
    }
}
