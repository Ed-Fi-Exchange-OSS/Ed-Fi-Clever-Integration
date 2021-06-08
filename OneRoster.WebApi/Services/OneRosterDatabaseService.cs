using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdFi.OneRoster.Persistence.EntityFrameWork;
using EdFi.OneRoster.WebApi.Services.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EdFi.OneRoster.WebApi.Services
{
    public class OneRosterDatabaseService : IOneRosterService
    {
        EdFiContext _db { get; set; }
        private readonly int? _localEducationAgencyId;
        private readonly int? _schoolId;

        public OneRosterDatabaseService(EdFiContext db, IOptions<ApplicationSettings> appSettings)
        {
            _db = db;

            var config = appSettings.Value;
            _schoolId = config.SchoolId;
            _localEducationAgencyId = config.LocalEducationAgencyId;
        }

        public async Task<ResponseModel<List<AcademicSessionModel>>> GetAcademicSessionsAsync(RequestModel request)
        {
            var query = _db.academicsessions;

            return new ResponseModel<List<AcademicSessionModel>>
            {
                TotalCount = await query.CountAsync(),
                Response = await query.ApplyFilters(request).Select(m => new AcademicSessionModel((academicsessions)m)).ToListAsync()
            };
        }

        public async  Task<ResponseModel<string>> GetCampusVersionAsync(RequestModel request)
        {
            var response = await Task.Run(() => new ResponseModel<string>
            {
                TotalCount = 1,
                Response = "Campus.2016.6"
            });

            return response;
        }

        public async Task<ResponseModel<List<CourseModel>>> GetCoursesAsync(RequestModel request)
        {
            var query = _db.courses.ApplySchoolIdFilter(_schoolId);

            return new ResponseModel<List<CourseModel>>
            {
                TotalCount = await query.CountAsync(),
                Response = await query.ApplyFilters(request).Select(m => new CourseModel((courses)m)).ToListAsync()
            };
        }

        public async Task<ResponseModel<List<DemographicModel>>> GetDemographicsAsync(RequestModel request)
        {
            var query = _db.demographics;

            return new ResponseModel<List<DemographicModel>>
            {
                TotalCount = await query.CountAsync(),
                Response = await query.ApplyFilters(request).Select(m => new DemographicModel((demographics)m)).ToListAsync()
            };
        }

        public async Task<ResponseModel<List<EnrollmentModel>>> GetEnrollmentsAsync(RequestModel request)
        {
            var query = _db.enrollments.ApplySchoolIdFilter(_schoolId);

            return new ResponseModel<List<EnrollmentModel>>
            {
                TotalCount = await query.CountAsync(),
                Response = await query.ApplyFilters(request).Select(m => new EnrollmentModel((enrollments)m)).ToListAsync()
            };
        }

        public async Task<ResponseModel<List<OrgModel>>> GetOrgsAsync(RequestModel request)
        {
            var query = _db.orgs.ApplySchoolIdFilter(_schoolId,_localEducationAgencyId);

            return new ResponseModel<List<OrgModel>>
            {
                TotalCount = await query.CountAsync(),
                Response = await query.ApplyFilters(request).Select(m => new OrgModel((orgs)m)).ToListAsync()
            };
        }

        public async Task<ResponseModel<List<UserModel>>> GetUsersAsync(RequestModel request)
        {
            var query = _db.users.ApplySchoolIdFilter(_schoolId);

            return new ResponseModel<List<UserModel>>
            {
                TotalCount = await query.CountAsync(),
                Response = await query.ApplyFilters(request).Select(m => new UserModel((users)m)).ToListAsync()
            };
        }

        public async Task<ResponseModel<List<ClassModel>>> GetClassesAsync(RequestModel request)
        {
            var query = _db.classes.ApplySchoolIdFilter(_schoolId);

            return new ResponseModel<List<ClassModel>>
            {
                TotalCount = await query.CountAsync(),
                Response = await query.ApplyFilters(request).Select(m => new ClassModel((classes)m)).ToListAsync()
            };
        }
    }
}
