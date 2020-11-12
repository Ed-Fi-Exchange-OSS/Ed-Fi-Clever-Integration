using EdFi.OneRoster.Persistence.EntityFrameWork;

namespace EdFi.OneRoster.WebApi.Services.Models
{
    public class DemographicModel : demographics
    {
        public new string birthDate { get; set; }

        public DemographicModel()
        {
        }
        public  DemographicModel (demographics dbElement)
        {

            sourcedId = dbElement.sourcedId;
            status = dbElement.status;
            dateLastModified = dbElement.dateLastModified;            
            birthDate = dbElement.birthDate.Value.ToString("yyyy-MM-dd");
            sex = dbElement.sex;
            americanIndianOrAlaskaNative = dbElement.americanIndianOrAlaskaNative;
            asian = dbElement.asian;
            blackOrAfricanAmerican = dbElement.blackOrAfricanAmerican;
            nativeHawaiianOrOtherPacificIslander = dbElement.nativeHawaiianOrOtherPacificIslander;
            white = dbElement.white;
            demographicRaceTwoOrMoreRaces = dbElement.demographicRaceTwoOrMoreRaces;
            hispanicOrLatinoEthnicity = dbElement.hispanicOrLatinoEthnicity;
        }
    }
}
