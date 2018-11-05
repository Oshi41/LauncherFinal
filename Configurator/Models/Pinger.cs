using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Models
{
    public class Pinger
    {
        private readonly Ping _ping = new Ping();

        private bool _isBusy;

        public bool CanPing(string path)
        {
            return !_isBusy && Uri.TryCreate(path, UriKind.Absolute, out var a);
        }

        public async Task<bool> CheckPing(string path)
        {
            if (!Uri.TryCreate(path, UriKind.Absolute, out var uri))
                return false;

            _isBusy = true;

            var result = await Task.Run(() =>
            {
                try
                {
                    return _ping.Send(uri.Host);
                }
                catch (Exception e)
                {
                    Trace.Write(e);
                    return null;
                }
            });

            _isBusy = false;

            return result?.Status == IPStatus.Success;
        }
    }
}
