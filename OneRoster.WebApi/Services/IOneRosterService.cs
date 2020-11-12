using System.Collections.Generic;
using System.Threading.Tasks;
using EdFi.OneRoster.WebApi.Services.Models;

namespace EdFi.OneRoster.WebApi.Services
{
    public interface IOneRosterService
    {
        Task<ResponseModel<List<AcademicSessionModel>>> GetAcademicSessionsAsync(RequestModel request);
        Task<ResponseModel<string>> GetCampusVersionAsync(RequestModel request);
        Task<ResponseModel<List<CourseModel>>> GetCoursesAsync(RequestModel request);
        Task<ResponseModel<List<OrgModel>>> GetOrgsAsync(RequestModel request);
        Task<ResponseModel<List<UserModel>>> GetUsersAsync(RequestModel request);
        Task<ResponseModel<List<EnrollmentModel>>> GetEnrollmentsAsync(RequestModel request);
        Task<ResponseModel<List<DemographicModel>>> GetDemographicsAsync(RequestModel request);
        Task<ResponseModel<List<ClassModel>>> GetClassesAsync(RequestModel request);
    }
}
