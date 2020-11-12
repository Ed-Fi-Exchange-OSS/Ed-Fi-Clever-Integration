using System;
using System.Text.Json.Serialization;

namespace EdFi.OneRoster.Persistence.EntityFrameWork
{
    public partial class users : ISchool
    {
        public string sourcedId { get; set; }
        public string status { get; set; }
        public DateTime? dateLastModified { get; set; }
        public string username { get; set; }
        public bool? enabledUser { get; set; }
        public string givenName { get; set; }
        public string middleName { get; set; }
        public string familyName { get; set; }
        public string role { get; set; }
        public string identifier { get; set; }
        public string email { get; set; }
        public string sms { get; set; }
        public string phone { get; set; }
        [JsonIgnore]
        public string orgs { get; set; }
        [JsonIgnore]
        public string grades { get; set; }
        [JsonIgnore]
        public int SchoolId { get; set; }
        [JsonIgnore]
        public string NameOfInstitution { get; set; }
    }
}
