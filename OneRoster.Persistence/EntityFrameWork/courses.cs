using System;
using System.Text.Json.Serialization;

namespace EdFi.OneRoster.Persistence.EntityFrameWork
{
    public partial class courses : ISchool
    {
        public Guid? sourcedId { get; set; }
        public string status { get; set; }
        public DateTime? dateLastModified { get; set; }
        public string title { get; set; }
        [JsonIgnore]
        public string schoolYear { get; set; }
        public string courseCode { get; set; }
        [JsonIgnore]
        public string grades { get; set; }
        [JsonIgnore]
        public string subjects { get; set; }
        [JsonIgnore]
        public string org { get; set; }
        [JsonIgnore]
        public int SchoolId { get; set; }
        [JsonIgnore]
        public string NameOfInstitution { get; set; }
    }
}
