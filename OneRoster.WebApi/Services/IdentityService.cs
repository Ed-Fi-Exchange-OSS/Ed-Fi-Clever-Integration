using EdFi.OneRoster.WebApi.Controllers.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdFi.OneRoster.WebApi.Services
{
    public interface IIdentityService
    {
        Task<ApiClient> GetIdentity(string key, string secret);
    }
    public class IdentityService : IIdentityService
    {
        public IdentityService() { }
        public async Task<ApiClient> GetIdentity(string key, string secret)
        {
            return new ApiClient();
        
            //var data = await _query.GetAPIClient(key, secret);
            //if (data == null) return null;

            //var model = new ApiClient
            //{
            //    Id = data.Id,
            //    Application = data.Application,
            //    NelticClientId = data.NelticClientId,
            //    Guid = data.Guid
            //};

            //return model;
        }
    }
}
