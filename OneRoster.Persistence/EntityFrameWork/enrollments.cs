using System;
using System.Text.Json.Serialization;

namespace EdFi.OneRoster.Persistence.EntityFrameWork
{
    public partial class enrollments : ISchool
    {
        public Guid? sourcedId { get; set; }
        public string status { get; set; }
        public DateTime? dateLastModified { get; set; }
        public string role { get; set; }
        public bool? primary { get; set; }

        [JsonIgnore]
        public string user { get; set; }
        [JsonIgnore]
        public string _class { get; set; }
        [JsonIgnore]
        public string school { get; set; }
        [JsonIgnore]
        public DateTime? beginDate { get; set; }
        [JsonIgnore]
        public DateTime? endDate { get; set; }

        [JsonIgnore]
        public int SchoolId { get; set; }
        [JsonIgnore]
        public string NameOfInstitution { get; set; }
    }
}
