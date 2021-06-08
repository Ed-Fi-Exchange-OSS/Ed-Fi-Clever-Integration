using System.Collections.Generic;

namespace EdFi.OneRoster.WebApi.Services.Models
{
    public class ApplicationSettings
    {
        public string DbMode { get; set; }
        public Dictionary<string,string> Clients { get; set; }

        /// <summary>
        /// OneRosterStaticDataService or OneRosterDatabaseService
        /// </summary>
        public string OneRosterService { get; set; }

        public bool OAuthEnabled { get; set; }                
        public int? SchoolId{ get; set; }
        public int? LocalEducationAgencyId { get; set; }
    }
}
