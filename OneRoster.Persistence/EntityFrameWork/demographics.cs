using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EdFi.OneRoster.Persistence.EntityFrameWork
{
    public partial class demographics :ISchool
    {
        public string sourcedId { get; set; }
        public string status { get; set; }
        public DateTime? dateLastModified { get; set; }

        [JsonIgnore]
        public DateTime? birthDate { get; set; }
        public string sex { get; set; }
        public string americanIndianOrAlaskaNative { get; set; }
        public string asian { get; set; }
        public string blackOrAfricanAmerican { get; set; }
        public string nativeHawaiianOrOtherPacificIslander { get; set; }
        public string white { get; set; }
        public string demographicRaceTwoOrMoreRaces { get; set; }
        public string hispanicOrLatinoEthnicity { get; set; }

        [NotMapped]
        [JsonIgnore]
        public int SchoolId { get; set; }

        [JsonIgnore]
        [NotMapped]
        public string NameOfInstitution { get; set; }
    }
}
