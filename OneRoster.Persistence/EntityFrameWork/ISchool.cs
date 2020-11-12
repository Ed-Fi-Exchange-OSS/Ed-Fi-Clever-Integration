using System.Text.Json.Serialization;

namespace EdFi.OneRoster.Persistence.EntityFrameWork
{
    public interface ISchool
    {
        [JsonIgnore]
        int SchoolId { get; set; }
        [JsonIgnore]
        string NameOfInstitution { get; set; }
    }
}
