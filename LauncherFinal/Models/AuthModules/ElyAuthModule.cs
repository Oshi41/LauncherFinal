using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;
using LauncherFinal.Helper;
using Newtonsoft.Json.Linq;

namespace LauncherFinal.Models.AuthModules
{
    class ElyAuthModule : IAuthModule
    {
        private readonly HttpClient _client = new HttpClient();
        private static string _authLink = "https://authserver.ely.by/api/users/profiles/minecraft/";
        private static string _signInLink = "https://authserver.ely.by/auth/authenticate";

        public async Task<KeyValuePair<bool, string>> GenerateToken(string login, 
            SecureString password)
        {
            var result = default(KeyValuePair<bool, string>);

            if (string.IsNullOrEmpty(login))
                return result;

            var idToken = await GetID(login);
            if (idToken == Guid.Empty)
                return result;

            var token = await GetAccessToken(login, password, idToken);

            result = new KeyValuePair<bool, string>(string.IsNullOrEmpty(token), token);

            return result;
        }

        /// <summary>
        ///     Уникальный ID пользователя по имени
        /// </summary>
        /// <returns></returns>
        private async Task<Guid> GetID(string login)
        {
            var loginQuerry = _authLink + login;
            try
            {
                var responseRaw = await _client.GetAsync(loginQuerry);
                var json = JObject.Parse(await responseRaw.Content.ReadAsStringAsync());
                if (json.ContainsKey("id"))
                {
                    if (Guid.TryParse(json["id"].ToString(), out var guid))
                        return guid;
                }
            }
            catch (Exception e)
            {
                Trace.Write(e);
            }

            return Guid.Empty;
        }

        /// <summary>
        ///     Своебразный пароль для регистрации
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private async Task<string> GetAccessToken(string login, SecureString password, Guid accessToken)
        {
            var message = $"username={login}&password={password.ConvertToString()}&clientToken={accessToken}";
            var content = new StringContent(message);
            var response = await _client.PostAsync(_signInLink, content);
            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
            return json["accessToken"]?.ToString();
        }
    }
}
