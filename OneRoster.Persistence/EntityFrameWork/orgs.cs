using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EdFi.OneRoster.Persistence.EntityFrameWork
{
    public partial class orgs : ISchool
    {
        [JsonIgnore]
        public int SchoolId { get; set; }
        [JsonIgnore]
        [NotMapped]
        public string NameOfInstitution { get; set; }
        [JsonIgnore]
        public Guid? sourcedId { get; set; }
        public string status { get; set; }
        public DateTime? dateLastModified { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string identifier { get; set; }
        [JsonIgnore]
        public Guid? parentId { get; set; }
    }
}
