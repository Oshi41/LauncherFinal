using System;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;
using LauncherFinal.Helper;
using MojangSharp.Endpoints;

namespace LauncherFinal.Models.AuthModules
{
    class YggdrasilAuthModule : IAuthModule
    {
        public Uri Uri { get; }

        public YggdrasilAuthModule(string uri)
        {
            Uri = new Uri(uri);
        }

        public async Task<KeyValuePair<bool, string>> GenerateToken(string login, SecureString password)
        {
            var credentials = new Credentials
            {
                Password = password.ConvertToString(),
                Username = login
            };

            var auth = new Authenticate(credentials)
            {
                Address = Uri
            };

            var result = await auth.PerformRequestAsync();

            if (!result.IsSuccess)
                return default(KeyValuePair<bool, string>);

            return new KeyValuePair<bool, string>(true, result.AccessToken);
        }
    }
}
