using System;
using System.Text.Json.Serialization;

namespace EdFi.OneRoster.Persistence.EntityFrameWork
{
    public partial class classes : ISchool
    {
        public Guid? sourcedId { get; set; }
        public string status { get; set; }
        public DateTime? dateLastModified { get; set; }
        public string title { get; set; }
        public string classCode { get; set; }
        public string classType { get; set; }
        public string location { get; set; }
        [JsonIgnore]
        public string grades { get; set; }
        [JsonIgnore]
        public string subjects { get; set; }
        [JsonIgnore]
        public string course { get; set; }
        [JsonIgnore]
        public string school { get; set; }
        [JsonIgnore]
        public string terms { get; set; }
        [JsonIgnore]
        public int SchoolId { get; set; }
        [JsonIgnore]
        public string NameOfInstitution { get; set; }
    }
}
