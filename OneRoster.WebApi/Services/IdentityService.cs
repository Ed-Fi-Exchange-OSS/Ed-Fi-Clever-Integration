using EdFi.OneRoster.WebApi.Controllers.OAuth2;
using EdFi.OneRoster.WebApi.Security;
using EdFi.OneRoster.WebApi.Services.Models;
using Microsoft.Extensions.Options;
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
        private readonly ApplicationSettings _settings;

        public IdentityService(
            IOptions<ApplicationSettings> settings
            ) {
            this._settings = settings.Value;

        }
        public async Task<ApiClient> GetIdentity(string key, string secret)
        {
            var clients = this._settings.Clients;

            if (!clients.Any(m => m.Key == key && m.Value == secret))
                return null;

            var client = clients.FirstOrDefault(m => m.Key == key && m.Value == secret);

            var model = new ApiClient
            {
                Id = client.Key,
                //Application = data.Application,
                //NelticClientId = data.NelticClientId,
                //Guid = data.Guid
            };

            return model;

            
            
                   

            //return model;
        }
    }
}
