using System.Collections.Generic;
using EdFi.OneRoster.Persistence.EntityFrameWork;
using Newtonsoft.Json;

namespace EdFi.OneRoster.WebApi.Services.Models
{
    public class UserModel : users 
    {

        public new List<ParentModel> orgs { get; set; }

        public new List<string> grades{ get; set; }

        public UserModel()
        {
        }
        public  UserModel (users dbItem)
        {
            sourcedId = dbItem.sourcedId;
            status = dbItem.status;
            dateLastModified = dbItem.dateLastModified;
            enabledUser = dbItem.enabledUser;
            givenName = dbItem.givenName;
            middleName = dbItem.middleName;
            familyName = dbItem.familyName;
            role = dbItem.role;
            identifier = dbItem.identifier;
            email = dbItem.email;
            sms = dbItem.sms;
            phone = dbItem.phone;
            orgs = JsonConvert.DeserializeObject<List<ParentModel>>(dbItem.orgs);
            grades = JsonConvert.DeserializeObject<List<string>>(dbItem.grades);
            username = dbItem.username;

        }
    }

    
}
