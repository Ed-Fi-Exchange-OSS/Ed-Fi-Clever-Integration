using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdFi.OneRoster.WebApi.Controllers.OAuth2
{
    public class OAuthRequest
    {
        public string Client_id { get; set; }
        public string Client_secret { get; set; }
        public string Grant_type { get; set; }
    }
}
