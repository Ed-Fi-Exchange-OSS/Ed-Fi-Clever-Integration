using System.Linq;
using EdFi.OneRoster.Persistence.EntityFrameWork;

namespace EdFi.OneRoster.WebApi.Services
{
    public static class EdOrgFilterHelper
    {
        public static IQueryable<ISchool> ApplySchoolIdFilter(this IQueryable<ISchool> query, int? schoolId ,int? LocalEducationAgencyId=0)
        {
            if (schoolId == 0 || schoolId == null) return query;

            if (LocalEducationAgencyId != 0 && LocalEducationAgencyId != null)
                return query.Where(x => x.SchoolId == schoolId ||  x.SchoolId==LocalEducationAgencyId);
            
            return query.Where(x => x.SchoolId == schoolId);
        }
        public static IQueryable<ISchool> ApplyFilters(this IQueryable<ISchool> query,  RequestModel requestModel)
        {
            return query.Skip(requestModel.offset).Take(requestModel.limit);
        }
    }
}
