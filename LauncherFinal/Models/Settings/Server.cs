using System.Collections.Generic;
using LauncherFinal.Models.Settings.Interfases;

namespace LauncherFinal.Models.Settings
{
    public class Server : IServer
    {
        public string Address { get; private set; }
        public string Name { get; private set; }
        public string DownloadLink { get; private set; }
        public IDictionary<string, string> DirHashCheck { get; } = new Dictionary<string, string>();

        #region Equals

        public override bool Equals(object obj)
        {
            if (obj is IServer server)
            {
                return EqualsInner(server);
            }

            return false;

        }

        protected bool EqualsInner(IServer other)
        {
            return string.Equals(Address, other.Address)
                   && string.Equals(Name, other.Name)
                   && string.Equals(DownloadLink, other.DownloadLink)
                   && Equals(DirHashCheck, other.DirHashCheck);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Address?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (Name?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (DownloadLink?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public static bool operator ==(Server left, IServer right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Server left, IServer right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}
