using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;

namespace LauncherFinal.Models.AuthModules
{
    public interface IAuthModule
    {
        Task<KeyValuePair<bool, string>> GenerateToken(string login, SecureString password);
    }

    public class EmptyModule : IAuthModule
    {
        public async Task<KeyValuePair<bool, string>> GenerateToken(string login, SecureString password)
        {
            return new KeyValuePair<bool, string>(true, null);
        }
    }
}
