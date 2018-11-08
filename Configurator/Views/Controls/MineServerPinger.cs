using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;

namespace Configurator.Views.Controls
{
    class MineServerPinger : BaseUriView
    {
        private bool _isPinging;

        public MineServerPinger() 
            : base(null, null)
        {
            Action = async s =>
            {
                try
                {
                    _isPinging = true;

                    var stat = new ServerStat(s);
                    await stat.GetInfoAsync();
                    return stat.ServerUp;
                }
                catch (Exception e)
                {
                    Trace.Write(e);
                    return false;
                }
                finally
                {
                    _isPinging = false;
                }
            };

            CanPing = s => !_isPinging && !string.IsNullOrWhiteSpace(s);

            ButtonHint = "Проверить связь с сервером";
        }
    }
}
