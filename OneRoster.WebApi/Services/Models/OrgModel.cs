using EdFi.OneRoster.Persistence.EntityFrameWork;

namespace EdFi.OneRoster.WebApi.Services.Models
{
    public class OrgModel : orgs
    {
        public new string sourcedId { get; set; }

        public ParentModel parent { get; set; }

        public OrgModel() { 
        }
        public  OrgModel (orgs orgs)
        {
            sourcedId = orgs.sourcedId.ToString();
            status = orgs.status;
            dateLastModified = orgs.dateLastModified;
            name = orgs.name;
            type = orgs.type;
            identifier = orgs.identifier;
            if(orgs.parentId!=null)
            parent = new ParentModel() { sourcedId = orgs.parentId.ToString() };

        }
    }
}
